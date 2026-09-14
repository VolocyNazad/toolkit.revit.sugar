# Repository guide

Paths in this document are relative to the repository root.
Read and follow the [development policy](policies/development.md) alongside this guide.

## About

Source for `VolocyNazad.Revit.Sugar`: a small set of extension methods and
static helpers for the Revit API (bounding box overlap checks, `ElementId`
version-agnostic accessors, `FilteredElementCollector` generic shortcuts,
`ElementFilter` factories, a fluent `IUpdater` builder). No DI, no state -
this library is syntactic sugar over the raw Revit API, not a service
layer. Extracted from `revit.linter` (`src/Toolkit.Revit.Extensions`).
Consumed by `revit.linter`.

Unlike `toolkit.revit.context` / `toolkit.revit.events` /
`toolkit.revit.mediatR` / `toolkit.revit.transaction-cache`, there is
**no** `DI/ServiceCollectionExtensions.cs` and **no**
`Abstractions/Services` here - do not add them out of habit when copying
structure from those repos.

## Repository structure

```
.
├── src/
│   └── Revit.Sugar/               the library
└── tests/
    └── Revit.Sugar.RevitTests/    TUnit tests hosted inside a real Revit process
```

## Tech stack

- .NET, built against `Revit_All_Main_Versions_API_x64` (Nice3point's
  multi-version Revit API reference package)
- MinVer (git-tag-based versioning)
- Tests: **TUnit + `Nice3point.TUnit.Revit`** (`RevitThreadExecutor`),
  matching `revit.linter`'s `*.RevitTests` convention - **not**
  `xunit.v3`, unlike the other repos in this family
- Central package management is not used; package versions are set per
  `<PackageReference>`. `Nice3point.TUnit.Revit`
  is versioned per Revit major version (e.g. `2025.x.y` for Revit 2025) -
  pinned inline as `Version="$(MajorRevitVersion).*"` rather than via
  `Directory.Packages.props`, since `revit.linter` centralizes that same
  convention there but this repo doesn't use central package management.

## Documentation layout

- `AGENTS.md` links to the required repository guidance.
- `docs/policies/development.md` contains the development policy.
- `docs/repository.md` describes the project, repository structure, and technology stack.

The root solution exposes the documentation files under a `docs` solution folder in Visual Studio, preserving their subfolder structure. When adding documentation files, also add them as solution items; solution folders do not automatically include new files.

CI builds the full solution for each supported Revit configuration, so the live Revit test project is compiled but not executed on hosted runners. Artifact names include the build configuration so each matrix job uploads a distinct artifact. NuGet publishing builds its own packages and does not download these CI artifacts.

The root `global.json` selects stable .NET SDK 10.0 (minimum `10.0.103`, `rollForward: latestFeature`). CI and publishing install the SDK from this file. Additional SDK installations may provide older test runtimes. See the [SDK selection policy](policies/development.md#net-sdk-selection).

## Solution items

The root solution exposes repository-level documents and configuration under `solutionItems`, GitHub files and maintenance scripts in matching subfolders, and documentation under `docs/`. The list is explicit, not a filesystem glob; keep links up to date when files change. See the [solution items policy](policies/development.md#solution-items).
## Repository validation

`scripts/Validate-Repository.ps1` enforces the required repository documents,
their navigation links, and complete, valid Solution Items. The
`.github/workflows/repository-policy.yml` workflow runs it for pushes and pull
requests. See the [development policy](policies/development.md#repository-validation).

## Formatting

The root `.editorconfig` defines the portable formatting baseline. Existing repositories may add stricter C# or analyzer-specific settings. See the [development policy](policies/development.md#formatting-baseline).

## Versioning and release tags

The library uses MinVer 8 with stable tags in the `0.0.PATCH` format and no `v` prefix. A tagged commit is built separately for Revit 2021.1.9, 2023.0.0, and 2025.0.0; tag `0.0.8`, for example, produces package versions `2021.0.8`, `2023.0.8`, and `2025.0.8`. Manual publishing accepts exactly one matching tag at HEAD and verifies the package ID and Revit-specific version before pushing to NuGet.
