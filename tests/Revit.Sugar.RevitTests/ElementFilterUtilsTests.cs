using Autodesk.Revit.DB;
using Nice3point.TUnit.Revit;
using Nice3point.TUnit.Revit.Executors;
using Revit.Sugar;
using TUnit.Core.Executors;

namespace Revit.Sugar.RevitTests;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "Test names use underscores to separate scenario and expected result.")]
internal sealed class ElementFilterUtilsTests : RevitApiTest
{
    private Document? _document;
    private Level? _level;
    private Wall? _wall;

    [Before(Test)]
    [HookExecutor<RevitThreadExecutor>]
    public void CreateModel()
    {
        _document = Application.NewProjectDocument(UnitSystem.Metric);

        using Transaction transaction = new(_document, "Seed ElementFilterUtils tests");
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
    public async Task AllFilter_MatchesEveryElement()
    {
        using ElementFilter filter = ElementFilterUtils.AllFilter();
        using FilteredElementCollector collector = new(_document!);

        ICollection<ElementId> matched = collector
            .WherePasses(filter)
            .ToElementIds();

        await Assert.That(matched).Contains(_wall!.Id);
    }

    [Test]
    public async Task EmptyFilter_MatchesNothing()
    {
        using ElementFilter filter = ElementFilterUtils.EmptyFilter();
        using FilteredElementCollector collector = new(_document!);

        ICollection<ElementId> matched = collector
            .WherePasses(filter)
            .ToElementIds();

        await Assert.That(matched).IsEmpty();
    }

    [Test]
    public async Task CategoryTypeFilter_MatchesOnlyRequestedCategoryType()
    {
        using ElementFilter modelFilter = ElementFilterUtils.CategoryTypeFilter(_document!, CategoryType.Model);
        using FilteredElementCollector collector = new(_document!);

        ICollection<ElementId> matched = collector
            .WherePasses(modelFilter)
            .WhereElementIsNotElementType()
            .ToElementIds();

        await Assert.That(matched).Contains(_wall!.Id);
    }

    [Test]
    public async Task CategoryTypeFilter_Inverse_ExcludesRequestedCategoryType()
    {
        using ElementFilter inverseModelFilter = ElementFilterUtils.CategoryTypeFilter(_document!, CategoryType.Model, inverse: true);
        using FilteredElementCollector collector = new(_document!);

        ICollection<ElementId> matched = collector
            .WherePasses(inverseModelFilter)
            .WhereElementIsNotElementType()
            .ToElementIds();

        await Assert.That(matched).DoesNotContain(_wall!.Id);
    }
}
