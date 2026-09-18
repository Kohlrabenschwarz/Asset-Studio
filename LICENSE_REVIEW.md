# AssetRipper license review — 2026-09-19

## Decision

Keep the existing Studio MIT license for now. This review did not establish that this update incorporated GPL-licensed AssetRipper implementation code. This is a scoped technical provenance review, not a certification that every inherited dependency is license-compatible.

## Evidence

- Compared all 250 shipped C# files against [RazTools/Studio at 6ba787de8e6cc03631f99db7690e3933b60b6eb3](https://github.com/RazTools/Studio/tree/6ba787de8e6cc03631f99db7690e3933b60b6eb3), normalizing CRLF/LF. 239 were unchanged, 8 changed, and 3 were new. The changed files are AnimationClip.cs, Renderer.cs, Shader.cs, SkinnedMeshRenderer.cs, Texture.cs, Texture2D.cs, MainForm.cs, and DllLoader.cs. The new files are AssetReplacementService.cs and the two audit programs. This identifies the immediate baseline; it does not prove the complete provenance of inherited code.
- Reviewed the added parsing logic: it adjusts existing version branches and reads fields according to input TypeTree nodes. The replacement writer calls AssetsTools.NET APIs. There is no AssetRipper package/project reference in the shipped projects and no newly added AssetRipper implementation module identified in this comparison. Absence of a package reference alone would not rule out copied code.
- **The bundled classdata.tpk is byte-for-byte identical to [UABEA ReleaseFiles/classdata.tpk at 057e2f6ae67ebc94a38faca9f946f8577162fa99](https://github.com/nesrak1/UABEA/blob/057e2f6ae67ebc94a38faca9f946f8577162fa99/ReleaseFiles/classdata.tpk).** Size: 289,605 bytes. SHA-256: `129E1F80F930415DB6779FE6089AFA75280CB51462BCEE812BEAB6CD81A764C6`. [UABEA's repository license at that revision](https://github.com/nesrak1/UABEA/blob/057e2f6ae67ebc94a38faca9f946f8577162fa99/license) is MIT, copyright 2021 nesrak1. This establishes an exact matching supplier revision, not the original download date or all database generation inputs.
- [AssetRipper/Tpk at 94549d10ee0fbb90392e45e555861031ff560359](https://github.com/AssetRipper/Tpk/tree/94549d10ee0fbb90392e45e555861031ff560359) is a separate **MIT** project, copyright 2022 ds5678. Its type-tree workflow packages [AssetRipper/TypeTreeDumps](https://github.com/AssetRipper/TypeTreeDumps). No Tpk C# implementation is compiled into Studio; Tpk is acknowledged as related database tooling. The exact build inputs used for UABEA's matching binary were not independently reconstructed.
- [AssetRipper/AssetRipper](https://github.com/AssetRipper/AssetRipper/blob/master/LICENSE.md), the main application, carries **GPLv3**. This license must not be generalized to every repository in the AssetRipper organization. Conversely, Tpk's MIT license must not be generalized to the main application.
- The inherited YAML and AnimationClipConverter files are unchanged from the RazTools baseline. RazTools credits mafaca/uTinyRipper; [uTinyRipper's license](https://github.com/mafaca/UtinyRipper/blob/master/LICENSE) is MIT, copyright 2020 mafaca. Those are inherited upstream credits, not evidence of a new GPL AssetRipper port.

## Remaining issues

The source headers in Unity.CecilTools and Unity.SerializationLogic name Unity's reference-only license; FMOD and Autodesk FBX SDK dependencies have separate terms. Several inherited native DLLs do not have a complete source/revision record in this checkout. Changing root LICENSE to GPL would not grant missing rights or establish compatibility with these terms. These components require their own redistribution review before claiming comprehensive compliance.

If a specific GPL AssetRipper code fragment is later identified, record its exact file and revision. Then remove/replace the fragment, obtain compatible permission, or distribute the combined work under compatible GPL terms with the required notices and corresponding source. Do not erase MIT author notices when addressing that case.

## Changes from this review

Recorded the exact UABEA binary match, corrected the earlier statement that no matching source revision was known, retained separate MIT notices, and documented the distinction between the GPL application and MIT tooling. No application behavior or root license changed. Existing release v1.36.1 remains unchanged; its older embedded attribution predates this provenance clarification.