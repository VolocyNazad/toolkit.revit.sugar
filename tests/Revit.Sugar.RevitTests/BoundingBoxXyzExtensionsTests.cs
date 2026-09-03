using Autodesk.Revit.DB;
using Nice3point.TUnit.Revit;
using Revit.Sugar;

namespace Revit.Sugar.RevitTests;

public sealed class BoundingBoxXyzExtensionsTests : RevitApiTest
{
    private static BoundingBoxXYZ CreateBox(XYZ min, XYZ max, Transform? transform = null)
    {
        var box = new BoundingBoxXYZ
        {
            Min = min,
            Max = max
        };

        if (transform is not null)
        {
            box.Transform = transform;
        }

        return box;
    }

    [Test]
    public async Task Overlaps_WhenBoxesIntersect_ReturnsTrue()
    {
        BoundingBoxXYZ a = CreateBox(new XYZ(0, 0, 0), new XYZ(2, 2, 2));
        BoundingBoxXYZ b = CreateBox(new XYZ(1, 1, 1), new XYZ(3, 3, 3));

        await Assert.That(a.Overlaps(b)).IsTrue();
        await Assert.That(b.Overlaps(a)).IsTrue();
    }

    [Test]
    public async Task Overlaps_WhenBoxesAreDisjoint_ReturnsFalse()
    {
        BoundingBoxXYZ a = CreateBox(new XYZ(0, 0, 0), new XYZ(1, 1, 1));
        BoundingBoxXYZ b = CreateBox(new XYZ(2, 2, 2), new XYZ(3, 3, 3));

        await Assert.That(a.Overlaps(b)).IsFalse();
    }

    [Test]
    public async Task Overlaps_WhenBoxesTouchAtBoundary_ReturnsTrue()
    {
        BoundingBoxXYZ a = CreateBox(new XYZ(0, 0, 0), new XYZ(1, 1, 1));
        BoundingBoxXYZ b = CreateBox(new XYZ(1, 1, 1), new XYZ(2, 2, 2));

        await Assert.That(a.Overlaps(b)).IsTrue();
    }

    [Test]
    public async Task Overlaps_JustOutsideTolerance_ReturnsFalse()
    {
        BoundingBoxXYZ a = CreateBox(new XYZ(0, 0, 0), new XYZ(1, 1, 1));
        BoundingBoxXYZ b = CreateBox(new XYZ(1.1, 1.1, 1.1), new XYZ(2, 2, 2));

        await Assert.That(a.Overlaps(b, tolerance: 1e-9)).IsFalse();
    }

    [Test]
    public async Task Overlaps_WithinExplicitTolerance_ReturnsTrue()
    {
        BoundingBoxXYZ a = CreateBox(new XYZ(0, 0, 0), new XYZ(1, 1, 1));
        BoundingBoxXYZ b = CreateBox(new XYZ(1.05, 1.05, 1.05), new XYZ(2, 2, 2));

        await Assert.That(a.Overlaps(b, tolerance: 0.1)).IsTrue();
    }

    [Test]
    public async Task Overlaps_WhenOneBoxIsTransformed_AccountsForWorldPosition()
    {
        // Box translated by (10, 0, 0) via its Transform - local coords look disjoint from `b`,
        // but the transformed (world) box overlaps it.
        var translated = Transform.CreateTranslation(new XYZ(10, 0, 0));
        BoundingBoxXYZ a = CreateBox(new XYZ(0, 0, 0), new XYZ(1, 1, 1), translated);
        BoundingBoxXYZ b = CreateBox(new XYZ(10.5, 0.5, 0.5), new XYZ(11, 1, 1));

        await Assert.That(a.Overlaps(b)).IsTrue();
    }
}
