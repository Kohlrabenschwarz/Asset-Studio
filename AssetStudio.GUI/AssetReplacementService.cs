using AssetsTools.NET;
using AssetsTools.NET.Extra;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using StudioObject = AssetStudio.Object;
using WritingManager = AssetsTools.NET.Extra.AssetsManager;

namespace AssetStudio.GUI
{
    /// <summary>Writes a modified copy of one asset file or its containing UnityFS bundle.</summary>
    public static class AssetReplacementService
    {
        public static bool CanImport(ClassIDType type) =>
            type == ClassIDType.Texture2D || type == ClassIDType.TextAsset || type == ClassIDType.Font;

        public static string InputFilter(ClassIDType type) => type switch
        {
            ClassIDType.Texture2D => "Images|*.png;*.jpg;*.jpeg;*.bmp;*.tga;*.tif;*.tiff|All files|*.*",
            ClassIDType.Font => "Fonts|*.ttf;*.otf|All files|*.*",
            _ => "All files|*.*"
        };

        public static void Replace(AssetItem item, string inputPath, string outputPath, bool raw)
        {
            if (item == null || item.Asset == null)
                throw new InvalidOperationException("Select one asset first.");
            string sourcePath = item.SourceFile.originalPath ?? item.SourceFile.fullName;
            if (!File.Exists(sourcePath))
                throw new FileNotFoundException("The source file is missing.", sourcePath);
            if (string.Equals(Path.GetFullPath(sourcePath), Path.GetFullPath(outputPath), StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Choose an output path different from the open source file.");
            if (!raw && !CanImport(item.Type))
                throw new NotSupportedException($"Automatic replacement is not available for {item.Type}. Use Replace raw for serialized object bytes.");

            var manager = new WritingManager();
            bool outputCreated = false;
            try
            {
                using (Stream tpk = Assembly.GetExecutingAssembly().GetManifestResourceStream("AssetStudio.GUI.Resources.classdata.tpk")
                    ?? throw new InvalidOperationException("Unity class database is missing from the application."))
                    manager.LoadClassPackage(tpk);

                byte[] replacement;
                if (item.SourceFile.originalPath != null)
                {
                    var bundle = manager.LoadBundleFile(sourcePath, true);
                    if (bundle.file.DataIsCompressed)
                        throw new NotSupportedException("This bundle compression could not be unpacked for editing.");
                    int index = bundle.file.GetFileIndex(item.SourceFile.fileName);
                    if (index < 0)
                    {
                        index = bundle.file.BlockAndDirInfo.DirectoryInfos.FindIndex(
                            d => string.Equals(Path.GetFileName(d.Name), item.SourceFile.fileName, StringComparison.OrdinalIgnoreCase));
                    }
                    if (index < 0)
                        throw new InvalidDataException($"Cannot locate {item.SourceFile.fileName} inside the bundle.");
                    var file = manager.LoadAssetsFileFromBundle(bundle, index, false);
                    replacement = RewriteAssets(manager, file, item.Asset, inputPath, raw);
                    bundle.file.BlockAndDirInfo.DirectoryInfos[index].SetNewData(replacement);
                    using var output = new FileStream(outputPath, FileMode.CreateNew, FileAccess.Write);
                    outputCreated = true;
                    using var writer = new AssetsFileWriter(output);
                    bundle.file.Write(writer, 0);
                }
                else
                {
                    var file = manager.LoadAssetsFile(sourcePath, false);
                    replacement = RewriteAssets(manager, file, item.Asset, inputPath, raw);
                    using var output = new FileStream(outputPath, FileMode.CreateNew, FileAccess.Write);
                    outputCreated = true;
                    output.Write(replacement);
                }
            }
            catch
            {
                if (outputCreated && File.Exists(outputPath))
                    File.Delete(outputPath);
                throw;
            }
            finally
            {
                manager.UnloadAll(true);
            }
        }

        private static byte[] RewriteAssets(WritingManager manager, AssetsFileInstance file, StudioObject asset, string inputPath, bool raw)
        {
            var info = file.file.GetAssetInfo(asset.m_PathID)
                ?? throw new InvalidDataException($"PathID {asset.m_PathID} does not exist in the source file.");
            if (info.GetTypeId(file.file) != (int)asset.type)
                throw new InvalidDataException("Selected asset type does not match the source file.");

            if (raw)
            {
                info.SetNewData(File.ReadAllBytes(inputPath));
            }
            else
            {
                manager.LoadClassDatabaseFromPackage(file.file.Metadata.UnityVersion);
                var field = manager.GetBaseField(file, info, AssetReadFlags.None);
                if (field == null || field.IsDummy)
                    throw new InvalidDataException("This object's type tree could not be read.");
                switch (asset.type)
                {
                    case ClassIDType.TextAsset:
                        Require(field, "m_Script").AsByteArray = File.ReadAllBytes(inputPath);
                        break;
                    case ClassIDType.Font:
                        Require(field, "m_FontData")["Array"].AsByteArray = File.ReadAllBytes(inputPath);
                        break;
                    case ClassIDType.Texture2D:
                        ImportTexture(field, inputPath);
                        break;
                    default:
                        throw new NotSupportedException($"Automatic import is not available for {asset.type}.");
                }
                info.SetNewData(field);
            }
            using var memory = new MemoryStream();
            using (var writer = new AssetsFileWriter(memory))
                file.file.Write(writer, 0);
            return memory.ToArray();
        }

        private static AssetTypeValueField Require(AssetTypeValueField field, string name)
        {
            var child = field[name];
            if (child == null || child.IsDummy)
                throw new InvalidDataException($"The required field {name} is missing in this Unity version.");
            return child;
        }

        private static void ImportTexture(AssetTypeValueField field, string inputPath)
        {
            using var image = new Bitmap(inputPath);
            if (image.Width <= 0 || image.Height <= 0 || (long)image.Width * image.Height > 268435456)
                throw new InvalidDataException("Invalid or excessively large image dimensions.");
            using var rgba = new Bitmap(image.Width, image.Height, PixelFormat.Format32bppArgb);
            using (var graphics = Graphics.FromImage(rgba))
                graphics.DrawImage(image, 0, 0, image.Width, image.Height);
            var bounds = new Rectangle(0, 0, image.Width, image.Height);
            var bits = rgba.LockBits(bounds, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            byte[] data = new byte[checked(image.Width * image.Height * 4)];
            try
            {
                byte[] row = new byte[image.Width * 4];
                for (int y = 0; y < image.Height; y++)
                {
                    Marshal.Copy(IntPtr.Add(bits.Scan0, y * bits.Stride), row, 0, row.Length);
                    int output = (image.Height - 1 - y) * row.Length;
                    for (int x = 0; x < row.Length; x += 4)
                    {
                        data[output + x] = row[x + 2];
                        data[output + x + 1] = row[x + 1];
                        data[output + x + 2] = row[x];
                        data[output + x + 3] = row[x + 3];
                    }
                }
            }
            finally { rgba.UnlockBits(bits); }

            Require(field, "m_Width").AsInt = image.Width;
            Require(field, "m_Height").AsInt = image.Height;
            Require(field, "m_TextureFormat").AsInt = 4; // Unity TextureFormat.RGBA32
            Require(field, "m_CompleteImageSize").AsInt = data.Length;
            if (!field["m_MipCount"].IsDummy) field["m_MipCount"].AsInt = 1;
            if (!field["m_MipMap"].IsDummy) field["m_MipMap"].AsBool = false;
            var stream = field["m_StreamData"];
            if (!stream.IsDummy)
            {
                stream["offset"].AsLong = 0;
                stream["size"].AsLong = 0;
                stream["path"].AsString = "";
            }
            var imageData = Require(field, "image data");
            imageData.AsByteArray = data;
        }
    }
}
