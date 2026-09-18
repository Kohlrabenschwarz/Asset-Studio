# Unreleased

Unity 6000.2.14f1 bundles now load with the correct Texture2D, Shader pass, and Renderer layouts when TypeTree metadata is available. Old texture fallback fields and Shader editor hashes/platform arrays are skipped when absent; new renderer flags and LOD fields are consumed when present. Later Unity releases also enter the shader program, stage count, animation flag, and texture mipmap-group branches added during Unity 2022.

SkinnedMeshRenderer root bone and bounds are read when declared by the schema. Empty TypeTree node lists use the version fallback rather than being treated as a complete schema. Layout fallbacks without TypeTree have not been verified against every Unity 6 release.

The GUI supports writing replacement Texture2D, TextAsset, Font, and raw object data into a new output file. The writer uses AssetsTools.NET and the embedded UABEA class database; attribution is retained.

Native dependencies can be extracted from embedded resources for single-EXE distribution. Extraction publishes completed files atomically to avoid exposing partially written DLLs during simultaneous starts.

The source package includes build instructions, audit-tool usage, and a GitHub workflow for managed project builds and single-file GUI publishing. Build caches, private game files, local settings, and executable distributions are excluded.
