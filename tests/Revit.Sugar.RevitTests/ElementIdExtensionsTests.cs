using Autodesk.Revit.DB;
using Nice3point.TUnit.Revit;
using Revit.Sugar;

namespace Revit.Sugar.RevitTests;

public sealed class ElementIdExtensionsTests : RevitApiTest
{
    [Test]
    public async Task IsCategory_WhenIdMatchesBuiltInCategory_ReturnsTrue()
    {
        ElementId wallsId = new(BuiltInCategory.OST_Walls);

        await Assert.That(wallsId.IsCategory(BuiltInCategory.OST_Walls)).IsTrue();
        await Assert.That(wallsId.IsCategory(BuiltInCategory.OST_Floors)).IsFalse();
    }

    [Test]
    public async Task ToBuiltInCategory_RoundTripsTheOriginalCategory()
    {
        ElementId floorsId = new(BuiltInCategory.OST_Floors);

        BuiltInCategory category = floorsId.ToBuiltInCategory();

        await Assert.That(category).IsEqualTo(BuiltInCategory.OST_Floors);
    }

    [Test]
    public async Task Value_MatchesTheUnderlyingCategoryId()
    {
        ElementId wallsId = new(BuiltInCategory.OST_Walls);

#if BEFORE2024
        await Assert.That(wallsId.Value()).IsEqualTo(wallsId.IntegerValue);
#else
        await Assert.That(wallsId.Value()).IsEqualTo(wallsId.Value);
#endif
    }
}
