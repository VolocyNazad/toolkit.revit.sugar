using Autodesk.Revit.DB;
using Nice3point.TUnit.Revit;
using Nice3point.TUnit.Revit.Executors;
using Revit.Sugar;
using TUnit.Core.Executors;

namespace Revit.Sugar.RevitTests;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "Test names use underscores to separate scenario and expected result.")]
internal sealed class FilteredElementCollectorExtensionsTests : RevitApiTest
{
    private Document? _document;
    private Level? _level;
    private Wall? _wall;

    [Before(Test)]
    [HookExecutor<RevitThreadExecutor>]
    public void CreateModel()
    {
        _document = Application.NewProjectDocument(UnitSystem.Metric);

        using Transaction transaction = new(_document, "Seed FilteredElementCollector tests");
        transaction.Start();
        _level = Level.Create(_document, 0);
        _wall = Wall.Create(
            _document,
            Line.CreateBound(XYZ.Zero, new XYZ(10, 0, 0)),
            _level.Id,
            false);
        transaction.Commit();
    }

    [After(Test)]
    [HookExecutor<RevitThreadExecutor>]
    public void CloseModel()
    {
        _document?.Close(false);
    }

    [Test]
    public async Task OfClassGeneric_MatchesSameElementsAsOfClassWithType()
    {
        using FilteredElementCollector genericCollector = new(_document!);
        ICollection<ElementId> viaGeneric = genericCollector
            .OfClass<Wall>()
            .ToElementIds();
        using FilteredElementCollector typeCollector = new(_document!);
        ICollection<ElementId> viaType = typeCollector
            .OfClass(typeof(Wall))
            .ToElementIds();

        await Assert.That(viaGeneric).IsEquivalentTo(viaType);
        await Assert.That(viaGeneric).Contains(_wall!.Id);
    }

    [Test]
    public async Task WhereElementIs_ForInstanceType_ReturnsOnlyInstances()
    {
        using FilteredElementCollector collector = new(_document!);

        ICollection<ElementId> walls = collector
            .WhereElementIs<Wall>()
            .ToElementIds();

        await Assert.That(walls).Contains(_wall!.Id);
        await Assert.That(walls).DoesNotContain(_wall!.GetTypeId());
    }

    [Test]
    public async Task WhereElementIs_ForElementTypeSubtype_ReturnsOnlyTypes()
    {
        using FilteredElementCollector collector = new(_document!);

        ICollection<ElementId> wallTypes = collector
            .WhereElementIs<WallType>()
            .ToElementIds();

        await Assert.That(wallTypes).Contains(_wall!.GetTypeId());
        await Assert.That(wallTypes).DoesNotContain(_wall!.Id);
    }
}
