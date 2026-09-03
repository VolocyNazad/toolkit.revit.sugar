# AGENTS.md

## Policy

The stack documented below is the default and takes priority over
whatever an agent might otherwise reach for. Prefer what's already in use
over introducing an alternative. If a deviation seems necessary, say so
explicitly to the user and get confirmation before adding it.

If a change affects the folder structure or the tech stack (a new/removed project, a new dependency, a version bump worth recording, a new convention), update this file accordingly as part of the same change - don't leave it to a later pass.

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
- No central package management (`Directory.Packages.props` is empty) -
  package versions are set per-`<PackageReference>`. `Nice3point.TUnit.Revit`
  is versioned per Revit major version (e.g. `2025.x.y` for Revit 2025) -
  pinned inline as `Version="$(MajorRevitVersion).*"` rather than via
  `Directory.Packages.props`, since `revit.linter` centralizes that same
  convention there but this repo doesn't use central package management.

## Testing note

Everything here is a thin wrapper around Revit API types (`XYZ`,
`Transform`, `BoundingBoxXYZ`, `ElementFilter` subclasses, `IUpdater`,
...). Merely *constructing* those types loads native modules bundled with
a Revit installation, so a plain `dotnet test` outside a running Revit
process fails with `FileNotFoundException` on `RevitAPI.dll` - even for
plain-looking geometry math. Because of that, unlike
`toolkit.revit.context` / `toolkit.revit.events` / `toolkit.revit.mediatR`
/ `toolkit.revit.transaction-cache` (plain `xunit.v3`, runnable in CI),
this repo's tests live in `tests/Revit.Sugar.RevitTests` and run via
`Nice3point.TUnit.Revit`'s `RevitThreadExecutor`, which launches/attaches
a real Revit process - same mechanism as `revit.linter`'s `*.RevitTests`
projects (e.g. `tests/Revit.Linter.Languages.RevitTests`).

Requirements to run these tests locally:
- A licensed Revit installed on the machine, matching the selected build
  configuration's version (e.g. `Debug_2025.0.0` needs Revit 2025).
- `dotnet test -c "Debug_2025.0.0"` (or run/debug via Rider/VS with
  "Testing Platform support" enabled).

CI (`ci.yml`) therefore only runs `dotnet build`, not `dotnet test` -
same as `revit.linter`, which has no automated test job at all for this
reason. Don't add a `dotnet test` step to CI without first solving the
"no licensed Revit on a hosted runner" problem.
