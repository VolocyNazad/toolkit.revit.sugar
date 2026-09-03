using Autodesk.Revit.DB;
using Nice3point.TUnit.Revit;
using Nice3point.TUnit.Revit.Executors;
using Revit.Sugar;
using TUnit.Core.Executors;

namespace Revit.Sugar.RevitTests;

public sealed class ElementFilterUtilsTests : RevitApiTest
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
        ElementFilter filter = ElementFilterUtils.AllFilter();

        ICollection<ElementId> matched = new FilteredElementCollector(_document!)
            .WherePasses(filter)
            .ToElementIds();

        await Assert.That(matched).Contains(_wall!.Id);
    }

    [Test]
    public async Task EmptyFilter_MatchesNothing()
    {
        ElementFilter filter = ElementFilterUtils.EmptyFilter();

        ICollection<ElementId> matched = new FilteredElementCollector(_document!)
            .WherePasses(filter)
            .ToElementIds();

        await Assert.That(matched).IsEmpty();
    }

    [Test]
    public async Task CategoryTypeFilter_MatchesOnlyRequestedCategoryType()
    {
        ElementFilter modelFilter = ElementFilterUtils.CategoryTypeFilter(_document!, CategoryType.Model);

        ICollection<ElementId> matched = new FilteredElementCollector(_document!)
            .WherePasses(modelFilter)
            .WhereElementIsNotElementType()
            .ToElementIds();

        await Assert.That(matched).Contains(_wall!.Id);
    }

    [Test]
    public async Task CategoryTypeFilter_Inverse_ExcludesRequestedCategoryType()
    {
        ElementFilter inverseModelFilter = ElementFilterUtils.CategoryTypeFilter(_document!, CategoryType.Model, inverse: true);

        ICollection<ElementId> matched = new FilteredElementCollector(_document!)
            .WherePasses(inverseModelFilter)
            .WhereElementIsNotElementType()
            .ToElementIds();

        await Assert.That(matched).DoesNotContain(_wall!.Id);
    }
}
