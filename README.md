# Revit.Sugar

[![Revit 2021-2027](https://img.shields.io/badge/Revit-2021–2027-green.svg)](https://autodesk.com/revit)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](https://opensource.org/licenses/MIT)
[![VolocyNazad](https://img.shields.io/badge/VolocyNazad-blue.svg)](https://github.com/VolocyNazad)

> Синтаксический сахар для работы с Revit API: extension-методы и небольшие хелперы без DI и без состояния.

Revit.Sugar — набор extension-методов и статических хелперов, упрощающих типовые операции с объектами Revit API: пересечение bounding box'ов, работа с `ElementId` без оглядки на версию API, обёртки над `FilteredElementCollector`, фабрики `ElementFilter`, fluent-билдер `IUpdater`.

## Возможности

- `BoundingBoxXYZ.Overlaps(other, tolerance)` — быстрая проверка пересечения мировых границ двух bounding box'ов (с учётом `Transform`), подходит как broad-phase проверка перед точным геометрическим пересечением.
- `ElementId.IsCategory(category)`, `.ToBuiltInCategory()`, `.Value()` — скрывают разницу между `IntegerValue` (до Revit 2024) и `Value` (2024+).
- `Category.BuiltInCategory` — то же самое для `Category` (только для версий до 2024).
- `FilteredElementCollector.OfClass<T>()`, `.WhereElementIs<T>()` — дженерик-обёртки над `OfClass`/`WhereElementIsElementType`/`WhereElementIsNotElementType`.
- `ElementFilterUtils.AllFilter()`, `.EmptyFilter()`, `.CategoryTypeFilter(doc, categoryType, inverse)` — готовые фабрики `ElementFilter`.
- `UpdaterBuilder` — fluent-билдер `IUpdater` без ручной реализации интерфейса.

## Установка

```
dotnet add package VolocyNazad.Revit.Sugar
```

## Использование

```csharp
using Revit.Sugar;

// Пересечение bounding box'ов
bool overlaps = boxA.Overlaps(boxB);

// ElementId, независимо от версии Revit
BuiltInCategory category = elementId.ToBuiltInCategory();
bool isWalls = elementId.IsCategory(BuiltInCategory.OST_Walls);

// FilteredElementCollector
var walls = new FilteredElementCollector(doc).WhereElementIs<Wall>().ToElements();

// Фабрики фильтров
ElementFilter all = ElementFilterUtils.AllFilter();
ElementFilter modelCategories = ElementFilterUtils.CategoryTypeFilter(doc, CategoryType.Model);

// IUpdater без ручной реализации интерфейса
IUpdater updater = new UpdaterBuilder()
    .SetUpdaterId(new UpdaterId(addinId, new Guid("...")))
    .SetUpdaterName("My Updater")
    .SetChangePriority(ChangePriority.Structure)
    .SetAction(data => { /* ... */ })
    .Build();
```

## Поддерживаемые версии Revit

Пакет собирается под версии Revit 2021–2027 (см. конфигурации в `Revit.Sugar.csproj`), таргетируя `net48` для версий до 2025 и `net8.0-windows` для 2025+.

## Требования

- .NET SDK 10.0.103+ (см. `global.json`)
- Revit API (пакет `Revit_All_Main_Versions_API_x64`)

## Лицензия

MIT, см. [LICENSE](LICENSE).
