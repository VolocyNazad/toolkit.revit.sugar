using Autodesk.Revit.DB;
using Nice3point.TUnit.Revit;
using Revit.Sugar;

namespace Revit.Sugar.RevitTests;

public sealed class UpdaterBuilderTests : RevitApiTest
{
    [Test]
    public async Task Build_AppliesAllConfiguredValues()
    {
        var updaterId = new UpdaterId(
            new AddInId(new Guid("22222222-2222-2222-2222-222222222222")),
            new Guid("11111111-1111-1111-1111-111111111111"));
        var executed = false;

        IUpdater updater = new UpdaterBuilder()
            .SetUpdaterId(updaterId)
            .SetUpdaterName("Test Updater")
            .SetAdditionalInformation("Additional info")
            .SetChangePriority(ChangePriority.FloorsRoofsStructuralWalls)
            .SetAction(_ => executed = true)
            .Build();

        updater.Execute(null!);

        await Assert.That(updater.GetUpdaterId()).IsEqualTo(updaterId);
        await Assert.That(updater.GetUpdaterName()).IsEqualTo("Test Updater");
        await Assert.That(updater.GetAdditionalInformation()).IsEqualTo("Additional info");
        await Assert.That(updater.GetChangePriority()).IsEqualTo(ChangePriority.FloorsRoofsStructuralWalls);
        await Assert.That(executed).IsTrue();
    }

    [Test]
    public async Task Build_WithoutConfiguration_UsesDefaults()
    {
        IUpdater updater = new UpdaterBuilder().Build();

        await Assert.That(updater.GetUpdaterId()).IsNull();
        await Assert.That(updater.GetUpdaterName()).IsEqualTo(string.Empty);
        await Assert.That(updater.GetAdditionalInformation()).IsEqualTo(string.Empty);
        await Assert.That(updater.GetChangePriority()).IsEqualTo(ChangePriority.Structure);
    }
}
