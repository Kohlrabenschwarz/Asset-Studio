# Studio — Unity 6 compatibility update

## AI disclosure and authorship

**The changes in this compatibility update were developed with AI assistance through OpenAI Codex. This README's update sections, build instructions, and review notes were also written and revised by AI.** Retained upstream documentation and the original application were written by their respective authors, not by the AI used for this update.

**Model information available in the current editing session: GPT-5-based OpenAI Codex.** The exact deployed model identifier/version is not exposed to this session, and model identifiers for earlier sessions cannot be verified from the available record. No more specific model name is claimed.

The user supplied requirements, sample files, and feedback; Codex generated and revised code and documentation and ran local checks. This is an independent downstream update, not an official release or endorsement by RazTools, Perfare, or the other credited maintainers. AI involvement does not replace their authorship, copyright notices, or licenses. The checks described below are limited to the documented local dataset and do not establish comprehensive correctness or independent human code review.

See [THIRD_PARTY_NOTICES.md](THIRD_PARTY_NOTICES.md) for upstream origins, dependencies, retained credits, and remaining provenance limits.


Source based on RazTools/Studio, which extends Perfare/AssetStudio. This update preserves the original license and credits.

## Changes

- Reads Texture fallback fields, Shader pass metadata, and Renderer additions according to the available TypeTree.
- Extends Shader, AnimationClip, and Texture2D version thresholds beyond Unity 2022.
- Produces a Windows x64 single EXE with its runtime and native libraries included.
- Supports replacing Texture2D, TextAsset, Font, or complete raw serialized object data in a new output file.
- Includes BundleAudit and ReplacementAudit for repeatable verification.

## Build and publish

Use Windows and the .NET 8 SDK. Bundled native DLLs are used; rebuilding the C++ FBX project separately requires its original native dependencies.

```powershell
dotnet build AssetStudio.GUI/AssetStudio.GUI.csproj -c Release -f net8.0-windows -m:1
dotnet build AssetStudio.CLI/AssetStudio.CLI.csproj -c Release -f net8.0-windows -m:1
dotnet publish AssetStudio.GUI/AssetStudio.GUI.csproj -c Release -f net8.0-windows -r win-x64 --self-contained true -m:1 -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:IncludeAllContentForSelfExtract=true -o artifacts/gui
```

The distributable is `artifacts/gui/AssetStudio.GUI.exe`. Runtime libraries are extracted to the temporary directory when needed. Build outputs belong in release artifacts, not the source repository.

## Verification and limits

The previous local full scan covered 3,528 input files, 512,225 loaded object records, and 13,024 successful texture previews. It skipped 33 textures over the audit's 16-million-pixel limit. One bundle had both engine versions stripped to `0.0.0` and requires an explicit version. These results concern that dataset, not all Unity versions or every export feature. Unknown classes retained as raw Object instances are not fully parsed classes.

```powershell
dotnet run --project BundleAudit -c Release -- <bundle-folder> 4000 report.json
dotnet run --project BundleAudit -c Release -- <bundle-file> inspect Shader
```

Reports contain input paths and error details; keep private game data and local reports out of GitHub. No sample game bundles are included. See [BUILDING.md](BUILDING.md) for replacement checks and [CHANGELOG.md](CHANGELOG.md) for review notes.

## Existing features and upstream notes

Asset replacement: select exactly one asset, then use **Replace → Replace selected asset...** for Texture2D (PNG/JPEG/BMP/TGA/TIFF), TextAsset, or Font (TTF/OTF). Texture imports are stored as Unity RGBA32 with one mip level, so the resulting bundle can be larger. **Replace → Replace raw serialized bytes...** accepts a complete serialized object for any asset type. Choose a new output file; the open source bundle is not overwritten. The modified bundle must be reloaded to inspect the saved change.

The replacement writer uses [AssetsTools.NET 3.0.0](https://github.com/nesrak1/AssetsTools.NET) by nesrak1 (MIT) and classdata.tpk previously obtained from [UABEA](https://github.com/nesrak1/UABEA). Separate license and attribution resources are included and embedded in the GUI build. The database's exact upstream revision was not recorded; see [THIRD_PARTY_NOTICES.md](THIRD_PARTY_NOTICES.md) for this provenance limitation.

## Upstream notice (RazTools archive)

Check out the [original AssetStudio project](https://github.com/Perfare/AssetStudio) for more information.

Note: Requires Internet connection to fetch asset_index jsons.
_____________________________________________________________________________________________________________________________
How to use:

Check the tutorial [here](https://gist.github.com/Modder4869/0f5371f8879607eb95b8e63badca227e) (Thanks to Modder4869 for the tutorial)
_____________________________________________________________________________________________________________________________
CLI Version:
```
Description:

Usage:
  AssetStudioCLI <input_path> <output_path> [options]

Arguments:
  <input_path>   Input file/folder.
  <output_path>  Output folder.

Options:
  --silent                                                Hide log messages.
  --type <Texture2D|Sprite|etc..>                         Specify unity class type(s)
  --filter <filter>                                       Specify regex filter(s).
  --game <BH3|CB1|CB2|CB3|GI|SR|TOT|ZZZ> (REQUIRED)       Specify Game.
  --map_op <AssetMap|Both|CABMap|None>                    Specify which map to build. [default: None]
  --map_type <JSON|XML>                                   AssetMap output type. [default: XML]
  --map_name <map_name>                                   Specify AssetMap file name.
  --group_assets_type <ByContainer|BySource|ByType|None>  Specify how exported assets should be grouped. [default: 0]
  --no_asset_bundle                                       Exclude AssetBundle from AssetMap/Export.
  --no_index_object                                       Exclude IndexObject/MiHoYoBinData from AssetMap/Export.
  --xor_key <xor_key>                                     XOR key to decrypt MiHoYoBinData.
  --ai_file <ai_file>                                     Specify asset_index json file path (to recover GI containers).
  --version                                               Show version information
  -?, -h, --help                                          Show help and usage information
```
_____________________________________________________________________________________________________________________________
NOTES:
```
- in case of any "MeshRenderer/SkinnedMeshRenderer" errors, make sure to enable "Disable Renderer" option in "Export Options" before loading assets.
- in case of need to export models/animators without fetching all animations, make sure to enable "Ignore Controller Anim" option in "Options -> Export Options" before loading assets.
```
_____________________________________________________________________________________________________________________________
Special Thank to:
- Perfare: Original author.
- Khang06: [Project](https://github.com/khang06/genshinblkstuff) for extraction.
- Radioegor146: [Asset-indexes](https://github.com/radioegor146/gi-asset-indexes) for recovered/updated asset_index's.
- Ds5678: [AssetRipper](https://github.com/AssetRipper/AssetRipper)[[discord](https://discord.gg/XqXa53W2Yh)] for information about Asset Formats & Parsing.
- mafaca: [uTinyRipper](https://github.com/mafaca/UtinyRipper) for `YAML` and `AnimationClipConverter`. 
