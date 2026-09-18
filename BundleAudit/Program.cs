using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using AssetStudio;

if (args.Length == 0 || (args.Length > 1 && args[1] == "inspect" && args.Length < 3))
{
    Console.Error.WriteLine("Usage: BundleAudit <file-or-folder> [maximum-files] [report.json] [unity-version]");
    Console.Error.WriteLine("       BundleAudit <file> inspect <class-name> [unity-version]");
    Environment.ExitCode = 2;
    return;
}
var root = args[0];
if (args.Length > 1 && args[1] == "inspect")
{
    Logger.Default = new ConsoleLogger();
    Logger.Flags = LoggerEvent.Error;
    var manager = new AssetsManager { Game = GameManager.GetGame(GameType.Normal), SpecifyUnityVersion = args.Length > 3 ? args[3] : null };
    manager.LoadFiles(root);
    var classId = (int)Enum.Parse<ClassIDType>(args[2]);
    foreach (var assets in manager.assetsFileList)
    {
        foreach (var info in assets.m_Objects.Where(x => x.classID == classId).Take(1))
        {
            Console.WriteLine($"OBJECT {info} version={assets.unityVersion} typeHash={Convert.ToHexString(info.serializedType.m_OldTypeHash ?? Array.Empty<byte>())}");
            foreach (var node in info.serializedType.m_Type?.m_Nodes ?? new())
                Console.WriteLine($"{new string(' ', node.m_Level * 2)}{node.m_Type} {node.m_Name} size={node.m_ByteSize} align={(node.m_MetaFlag & 0x4000) != 0}");
        }
    }
    manager.Clear();
    return;
}
var maximum = args.Length > 1 ? int.Parse(args[1]) : 250;
var files = (File.Exists(root) ? new[] { root } : Directory.EnumerateFiles(root, "*", SearchOption.AllDirectories))
    .Select(path => new FileInfo(path))
    .Where(file => file.Length > 0)
    .Select(file => new Candidate(file.FullName, file.Length, Header(file.FullName)))
    .ToList();
var sample = maximum >= files.Count
    ? files.OrderBy(file => file.Length).ToList()
    : files.GroupBy(file => (file.Header, Bucket(file.Length)))
        .SelectMany(group => group.OrderBy(file => file.Path, StringComparer.Ordinal).Take(Math.Max(4, maximum / 15)))
        .OrderBy(file => file.Length)
        .Take(maximum)
        .ToList();
var sink = new AuditLogger();
Logger.Default = sink;
Logger.Flags = LoggerEvent.Error;
var rows = new List<object>();
var index = 0;
foreach (var file in sample)
{
    sink.Messages.Clear();
    var manager = new AssetsManager { Game = GameManager.GetGame(GameType.Normal), SpecifyUnityVersion = args.Length > 3 ? args[3] : null };
    var versions = new List<string>();
    var typeCounts = new Dictionary<string, int>();
    var previewOk = 0;
    var previewFailed = 0;
    var previewSkipped = 0;
    var previewErrors = new List<string>();
    var declared = 0;
    var loaded = 0;
    string fatal = null;
    try
    {
        manager.LoadFiles(file.Path);
        foreach (var assets in manager.assetsFileList)
        {
            versions.Add(assets.unityVersion);
            declared += assets.m_Objects.Count;
            loaded += assets.Objects.Count;
            foreach (var obj in assets.Objects)
            {
                var type = obj.GetType().Name;
                typeCounts[type] = typeCounts.GetValueOrDefault(type) + 1;
            }
            foreach (var texture in assets.Objects.OfType<Texture2D>())
            {
                try
                {
                    if (texture.m_Width <= 0 || texture.m_Height <= 0 ||
                        (long)texture.m_Width * texture.m_Height > 16_000_000)
                    {
                        previewSkipped++;
                        continue;
                    }
                    using var image = texture.ConvertToImage(false);
                    if (image == null)
                    {
                        previewFailed++;
                        previewErrors.Add($"{texture.Name}: no image ({texture.m_TextureFormat}, {texture.m_Width}x{texture.m_Height}, data={texture.image_data.Size})");
                    }
                    else previewOk++;
                }
                catch (Exception ex)
                {
                    previewFailed++;
                    previewErrors.Add($"{texture.Name}: {ex.GetType().Name}: {ex.Message}");
                }
            }
        }
    }
    catch (Exception ex) { fatal = $"{ex.GetType().Name}: {ex.Message}"; }
    finally { manager.Clear(); }
    rows.Add(new { file.Path, file.Length, file.Header, Versions = versions.Distinct().ToArray(),
        Declared = declared, Loaded = loaded, Types = typeCounts,
        Errors = sink.Messages.Count, FirstError = sink.Messages.FirstOrDefault(),
        PreviewOk = previewOk, PreviewFailed = previewFailed, PreviewSkipped = previewSkipped, PreviewErrors = previewErrors,
        Fatal = fatal });
    Console.WriteLine($"{++index}/{sample.Count} {file.Header} {file.Length} objects={loaded}/{declared} errors={sink.Messages.Count} previews={previewOk}/{previewFailed} {Path.GetFileName(file.Path)}");
}
File.WriteAllText(args.Length > 2 ? args[2] : "bundle-audit.json", JsonSerializer.Serialize(rows, new JsonSerializerOptions { WriteIndented = true }));

static string Header(string path)
{
    using var stream = File.OpenRead(path);
    Span<byte> bytes = stackalloc byte[8];
    bytes.Clear();
    stream.Read(bytes);
    return System.Text.Encoding.ASCII.GetString(bytes).TrimEnd('\0');
}

static int Bucket(long length) => (int)Math.Log2(Math.Max(length, 1));

record Candidate(string Path, long Length, string Header);

sealed class AuditLogger : ILogger
{
    public List<string> Messages { get; } = new();
    public void Log(LoggerEvent level, string message)
    {
        if (level == LoggerEvent.Error) Messages.Add(message);
    }
}
