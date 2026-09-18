# Asset Studio

Unity asset and bundle viewer based on [RazTools/Studio](https://github.com/RazTools/Studio), which builds on [Perfare/AssetStudio](https://github.com/Perfare/AssetStudio).

This update improves Unity 6 Texture2D, Shader, Renderer, and AnimationClip parsing, supports asset replacement, and provides a single Windows x64 executable.

## Download and use

Download `AssetStudio.GUI.exe` from [Releases](https://github.com/Kohlrabenschwarz/Asset-Studio/releases). Open bundles with **File → Load file/folder**. To replace an asset, select it and use **Replace**; save to a new output file and reload it to inspect the result.

## Build

On Windows with the .NET 8 SDK:

```powershell
dotnet publish AssetStudio.GUI/AssetStudio.GUI.csproj -c Release -f net8.0-windows -r win-x64 --self-contained true -m:1 -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:IncludeAllContentForSelfExtract=true -o artifacts/gui
```

See [BUILDING.md](BUILDING.md) for CLI builds and audit tools.

## Credits

Code changes and this README were prepared with **OpenAI Codex (GPT-5.6 SOL)**. This is an independent downstream update. Original authorship and licenses are retained; credits and dependency provenance are in [THIRD_PARTY_NOTICES.md](THIRD_PARTY_NOTICES.md).
