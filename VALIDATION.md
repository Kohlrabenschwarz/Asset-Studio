# Local validation — 2026-09-18

Checked the prepared source copy on Windows with .NET SDK 10.0.300 targeting .NET 8.

| Check | Result |
| --- | --- |
| GUI, CLI, BundleAudit, ReplacementAudit builds | Passed; existing unused-variable and YAML TODO warnings remain |
| Single-file Windows x64 GUI publish | Exactly one EXE; startup passed |
| Full input scan | 3,528 input files; 3,493 contained loaded serialized assets |
| Object records | 512,225 declared and loaded; this includes raw Object fallbacks |
| Texture previews | 13,024 passed, 0 failed, 33 skipped over 16 million pixels |
| Load errors | One input with Unity versions stripped to 0.0.0 |
| Texture replacement round trip | Generated 4x4 RGBA32 texture was written to a new bundle and reloaded successfully |

The input set included Unity 6000.2.14f1 and several Unity 2022.3 patch versions. The stripped-version input was not assigned a guessed version. No private game data, path-bearing JSON reports, or local executables are included in this source folder.

Text, font, and raw replacement round trips were not rerun in this preparation pass. Model export, audio playback, every shader export target, and no-TypeTree Unity 6 layouts were not exhaustively tested. The GitHub workflow has been prepared but has not yet run remotely.

The local single-file publish used the cached .NET 8.0.10 runtime. Repository commands and CI intentionally resolve the current patch available to their SDK instead of pinning that runtime.
