# Build and verification

Build the managed projects directly on Windows with the .NET 8 SDK. The solution includes the optional C++ FBX project; building the entire solution requires its native build dependencies. GUI and CLI builds use the prebuilt libraries already in `AssetStudio.GUI/Libraries`.

The README publish command uses the current runtime patch available to the selected SDK. The earlier offline executable used .NET 8.0.10 because it was the locally cached runtime; that old runtime is not pinned in this source package.

## Bundle audit

`BundleAudit <file-or-folder> [maximum-files] [report.json] [unity-version]` loads each selected input independently and attempts every Texture2D preview up to 16 million pixels. Use a maximum larger than the input count for a full scan. Smaller maxima select a sample by header and file size. An exit code of zero only means the scan finished; inspect `Errors`, `PreviewFailed`, `PreviewSkipped`, `Fatal`, and `Declared` versus `Loaded` in the JSON report.

`BundleAudit <file> inspect <class-name> [unity-version]` prints the first matching object's TypeTree. Supplying a version overrides the input's version; use it only when the real version is known. There is no automatic majority-version guessing for stripped files.

## Replacement audit

Run these checks with your own input data and a new output path:

```powershell
dotnet run --project ReplacementAudit -c Release -- <texture-bundle> <new-output-bundle> texture
dotnet run --project ReplacementAudit -c Release -- <text-bundle> <new-output-bundle> text
dotnet run --project ReplacementAudit -c Release -- <bundle> <new-output-bundle> raw
```

The tool creates its replacement input beside the output, writes a new bundle, reloads it, and checks the selected object. Texture checks use a generated 4x4 image; text checks compare exact bytes. Font import exists but has no dedicated audit mode. Replacement checks do not establish that modified bundles will be accepted by a particular game.

## Publishing

The GitHub workflow builds CLI and both audit tools, then publishes the GUI as one Windows x64 EXE. It does not run game-data audits on GitHub because private sample files are not included. Original copyright notices, `LICENSE`, and the UABEA attribution resource are preserved.
