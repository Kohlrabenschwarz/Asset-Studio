using AssetStudio;
using AssetStudio.GUI;
using System.Drawing;
using System.IO;
using System.Linq;
using System;

if (args.Length < 3 || !new[] { "scan", "texture", "text", "raw" }.Contains(args[2]))
{
    Console.Error.WriteLine("Usage: ReplacementAudit <source-file-or-folder> <new-output-file> <scan|texture|text|raw>");
    Environment.ExitCode = 2;
    return;
}
string source = args[0];
string output = args[1];
string mode = args[2];
Logger.Default = new ConsoleLogger();
Logger.Flags = LoggerEvent.Error;
if (mode == "scan")
{
    int scanned = 0;
    foreach (string path in Directory.EnumerateFiles(source).OrderBy(p => p))
    {
        if (++scanned > 350) break;
        var scan = new AssetsManager { Game = GameManager.GetGame(GameType.Normal) };
        try
        {
            scan.LoadFiles(path);
            var kinds = scan.assetsFileList.SelectMany(f => f.Objects).Select(o => o.type).Distinct().ToArray();
            if (kinds.Contains(ClassIDType.TextAsset) || kinds.Contains(ClassIDType.Font))
                Console.WriteLine($"{path} {string.Join(',', kinds.Where(k => k == ClassIDType.TextAsset || k == ClassIDType.Font))}");
        }
        finally { scan.Clear(); }
    }
    return;
}
var manager = new AssetsManager { Game = GameManager.GetGame(GameType.Normal) };
manager.LoadFiles(source);
AssetStudio.Object asset = mode == "texture"
    ? manager.assetsFileList.SelectMany(f => f.Objects).OfType<Texture2D>().FirstOrDefault(t => t.m_Width > 0 && t.m_Height > 0)
    : mode == "text" ? manager.assetsFileList.SelectMany(f => f.Objects).OfType<TextAsset>().FirstOrDefault()
    : manager.assetsFileList.SelectMany(f => f.Objects).FirstOrDefault(o => o.type != ClassIDType.TextAsset);
if (asset == null) throw new System.Exception("No matching asset in sample.");
Console.WriteLine($"SOURCE {asset.type} {asset.Name} {asset.m_PathID} {asset.assetsFile.unityVersion}");
var item = new AssetItem(asset);
string input = Path.Combine(Path.GetDirectoryName(output)!, Path.GetFileName(output) + (mode == "texture" ? ".png" : ".dat"));
Directory.CreateDirectory(Path.GetDirectoryName(output)!);
if (mode == "texture")
{
    using var bitmap = new Bitmap(4, 4);
    using (var g = Graphics.FromImage(bitmap)) g.Clear(System.Drawing.Color.FromArgb(255, 37, 85, 204));
    bitmap.Save(input, System.Drawing.Imaging.ImageFormat.Png);
}
else File.WriteAllText(input, "AssetStudio replacement verification");
if (mode == "raw") File.WriteAllBytes(input, asset.GetRawData());
AssetReplacementService.Replace(item, input, output, mode == "raw");
manager.Clear();
var check = new AssetsManager { Game = GameManager.GetGame(GameType.Normal) };
check.LoadFiles(output);
var found = check.assetsFileList.SelectMany(f => f.Objects).FirstOrDefault(o => o.m_PathID == item.m_PathID && o.type == item.Type);
if (found == null) throw new System.Exception("Replaced object missing in output.");
if (mode == "texture")
{
    var texture = (Texture2D)found;
    using var image = texture.ConvertToImage(false);
    if (image == null || image.Width != 4 || image.Height != 4) throw new System.Exception("Texture preview failed.");
    Console.WriteLine($"PASS texture {texture.m_Width}x{texture.m_Height}, {texture.m_TextureFormat}");
}
else if (mode == "text")
{
    byte[] expected = File.ReadAllBytes(input);
    byte[] actual = ((TextAsset)found).m_Script;
    if (!actual.SequenceEqual(expected))
        throw new Exception($"Text content mismatch: expected {expected.Length} bytes, actual {actual.Length} bytes");
    Console.WriteLine($"PASS text {actual.Length} bytes");
}
else Console.WriteLine($"PASS raw {found.type} {found.GetRawData().Length} bytes");
check.Clear();
