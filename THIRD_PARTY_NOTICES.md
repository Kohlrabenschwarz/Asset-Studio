# Third-party notices and provenance

This downstream update is based on **RazTools/Studio**, not directly on the original Perfare checkout. Upstream code, documentation, contributors, and bundled dependencies retain their original authorship. The AI disclosure in README applies to this update, not to those projects.

## Direct project lineage and added dependency

| Project / author | Role in this source package | Notice |
| --- | --- | --- |
| [RazTools/Studio](https://github.com/RazTools/Studio), Razmoth and contributors | Immediate upstream; extended AssetStudio implementation | MIT; root LICENSE preserved |
| [Perfare/AssetStudio](https://github.com/Perfare/AssetStudio), Perfare and contributors | Original application underlying RazTools/Studio | MIT; original notices preserved, including Radu and hozuki where present |
| [nesrak1/AssetsTools.NET](https://github.com/nesrak1/AssetsTools.NET) | NuGet 3.0.0 used by the replacement writer | MIT; Resources/AssetsTools.NET-LICENSE.txt; package metadata identifies nesrak1 |
| [nesrak1/UABEA](https://github.com/nesrak1/UABEA) | Recorded source of the bundled classdata.tpk | MIT notice retained; binary matched to UABEA revision 057e2f6; details below |
| [SeriousCache/UABE](https://github.com/SeriousCache/UABE) | Historical basis credited by AssetsTools.NET/UABEA | Credit for that lineage, not a claim that its entire source is included here |

## Retained upstream credits and research references

- [khang06/genshinblkstuff](https://github.com/khang06/genshinblkstuff): RazTools credits its extraction work. The upstream link could not be retrieved during this review; credit is retained.
- [radioegor146/gi-asset-indexes](https://github.com/radioegor146/gi-asset-indexes): asset-index work credited by RazTools. Private game files and downloaded indexes are not included in this source package.
- [AssetRipper/AssetRipper](https://github.com/AssetRipper/AssetRipper), ds5678 and contributors: upstream credit for asset format and parsing information. Its current repository is GPL-3.0; that code must not be assumed covered by Studio's MIT license. This review does not establish a file-by-file provenance history for inherited code.
- [mafaca/UtinyRipper](https://github.com/mafaca/UtinyRipper): retained RazTools credit for YAML and AnimationClipConverter.
- [K0lb3/UnityPy](https://github.com/K0lb3/UnityPy): related Unity asset research reference. No UnityPy package or Python runtime is shipped in this project; this reference does not assert a verified history of copied code.
- [Modder4869's tutorial](https://gist.github.com/Modder4869/0f5371f8879607eb95b8e63badca227e): original tutorial credit and link retained.
- [AssetRipper/Tpk](https://github.com/AssetRipper/Tpk) and its contributors: related class database tooling linked by AssetsTools.NET. This separate repository is MIT, not GPL. Its notice (2022 ds5678) is retained; the bundled database matches UABEA as recorded below.

## Bundled components and separate terms

The root MIT license must not be read as licensing every dependency under MIT. Retained source headers identify Unity Technologies' reference-only terms for Unity.CecilTools and Unity.SerializationLogic, Firelight Technologies for FMOD bindings, and Matthäus G. Chajdas for CSspv (its own LICENSE is retained). The original FBX wrapper credits Perfare and hozuki; its native build uses Autodesk FBX SDK with separate terms.

The Perfare README also credits [Ishotihadus/mikunyan](https://github.com/Ishotihadus/mikunyan), [BinomialLLC/crunch](https://github.com/BinomialLLC/crunch), and [Unity-Technologies/crunch](https://github.com/Unity-Technologies/crunch/tree/unity) for texture decoding. These acknowledgments are retained here; they do not identify every binary's source revision.

Native DLLs inherited from the supplied checkout (including FMOD, FBXNative, HLSLDecompiler, and ACL variants), OpenTK.WinForms, and NuGet packages retain their own terms. This review preserved existing notices but did not independently reconstruct all inherited binary provenance or certify redistribution compliance for every bundled component.

## Database provenance record and limits

The bundled classdata.tpk exactly matches `ReleaseFiles/classdata.tpk` at [UABEA revision 057e2f6ae67ebc94a38faca9f946f8577162fa99](https://github.com/nesrak1/UABEA/blob/057e2f6ae67ebc94a38faca9f946f8577162fa99/ReleaseFiles/classdata.tpk): 289,605 bytes, SHA-256 `129E1F80F930415DB6779FE6089AFA75280CB51462BCEE812BEAB6CD81A764C6`. UABEA is MIT licensed. The original download date and underlying generation inputs remain unverified; the exact byte match now establishes a matching supplier revision. See [LICENSE_REVIEW.md](LICENSE_REVIEW.md) for the code comparison, license distinction, and remaining dependency limits.

This notice distinguishes direct dependencies, inherited credits, and research references. It does not claim that all listed repositories contributed newly copied code or that a comprehensive legal/provenance audit has been completed. Existing LICENSE files and copyright headers have not been removed or reassigned to the downstream user or AI.