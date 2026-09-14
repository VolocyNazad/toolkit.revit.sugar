# Revit.Sugar

[![Revit 2021.1.9, 2023, 2025](https://img.shields.io/badge/Revit-2021.1.9%20%7C%202023%20%7C%202025-green.svg)](https://autodesk.com/revit)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](https://opensource.org/licenses/MIT)
[![VolocyNazad](https://img.shields.io/badge/VolocyNazad-blue.svg)](https://github.com/VolocyNazad)

> Syntactic sugar for working with the Revit API: extension methods and small helpers with no DI and no state.

Revit.Sugar — a set of extension methods and static helpers that simplify common operations on Revit API objects: bounding box intersection, `ElementId` handling that doesn't depend on the API version, wrappers over `FilteredElementCollector`, `ElementFilter` factories, and a fluent `IUpdater` builder.

## Features

- `BoundingBoxXYZ.Overlaps(other, tolerance)` — a fast check for whether two bounding boxes' world bounds intersect (accounting for `Transform`), suitable as a broad-phase check before exact geometric intersection.
- `ElementId.IsCategory(category)`, `.ToBuiltInCategory()`, `.Value()` — hide the difference between `IntegerValue` (before Revit 2024) and `Value` (2024+).
- `Category.BuiltInCategory` — the same for `Category` (versions before 2024 only).
- `FilteredElementCollector.OfClass<T>()`, `.WhereElementIs<T>()` — generic wrappers over `OfClass`/`WhereElementIsElementType`/`WhereElementIsNotElementType`.
- `ElementFilterUtils.AllFilter()`, `.EmptyFilter()`, `.CategoryTypeFilter(doc, categoryType, inverse)` — ready-made `ElementFilter` factories.
- `UpdaterBuilder` — a fluent `IUpdater` builder that avoids implementing the interface by hand.

## Installation

```
dotnet add package VolocyNazad.Revit.Sugar
```

## Usage

```csharp
using Revit.Sugar;

// Bounding box intersection
bool overlaps = boxA.Overlaps(boxB);

// ElementId, independent of the Revit version
BuiltInCategory category = elementId.ToBuiltInCategory();
bool isWalls = elementId.IsCategory(BuiltInCategory.OST_Walls);

// FilteredElementCollector
var walls = new FilteredElementCollector(doc).WhereElementIs<Wall>().ToElements();

// Filter factories
ElementFilter all = ElementFilterUtils.AllFilter();
ElementFilter modelCategories = ElementFilterUtils.CategoryTypeFilter(doc, CategoryType.Model);

// IUpdater without implementing the interface by hand
IUpdater updater = new UpdaterBuilder()
    .SetUpdaterId(new UpdaterId(addinId, new Guid("...")))
    .SetUpdaterName("My Updater")
    .SetChangePriority(ChangePriority.Structure)
    .SetAction(data => { /* ... */ })
    .Build();
```

## Supported Revit versions

The package is built and tested for Revit 2021.1.9, 2023.0.0, and 2025.0.0. Revit 2021 and 2023 target net48; Revit 2025 targets net8.0-windows.

## Requirements

- .NET SDK 10.0.103+ (see `global.json`)
- Revit API (the `Revit_All_Main_Versions_API_x64` package)

## License

MIT, see [LICENSE](LICENSE).

## Development documentation

- [Development policy](docs/policies/development.md)
- [Repository guide and technology stack](docs/repository.md)

## Contributing

Read [CONTRIBUTING.md](CONTRIBUTING.md) before submitting changes.
