using Autodesk.Revit.DB;

namespace Revit.Sugar;

/// <summary>
/// A fluent builder for <see cref="Autodesk.Revit.DB.IUpdater"/> that lets a caller define an
/// updater's behavior with a lambda instead of writing a dedicated class implementing the
/// interface.
/// </summary>
public sealed class UpdaterBuilder
{
    /// <summary>
    /// An <see cref="Autodesk.Revit.DB.IUpdater"/> implementation whose behavior and metadata
    /// are supplied entirely through settable properties, configured by the enclosing
    /// <see cref="UpdaterBuilder"/>.
    /// </summary>
    private sealed class LambdaUpdater : IUpdater
    {
        public string AdditionalInformation { get; set; } = string.Empty;
        public ChangePriority ChangePriority { get; set; } = ChangePriority.Structure;
        public UpdaterId? UpdaterId { get; set; }
        public string UpdaterName { get; set; } = string.Empty;
        public Action<UpdaterData> Action { get; set; } = delegate { };

        public void Execute(UpdaterData data) => Action(data);
        public string GetAdditionalInformation() => AdditionalInformation;
        public ChangePriority GetChangePriority() => ChangePriority;
        public UpdaterId? GetUpdaterId() => UpdaterId;
        public string GetUpdaterName() => UpdaterName;
    }

    private readonly LambdaUpdater _updater = new();

    /// <summary>
    /// Builds the configured <see cref="Autodesk.Revit.DB.IUpdater"/>.
    /// </summary>
    /// <returns>An <see cref="Autodesk.Revit.DB.IUpdater"/> reflecting everything set on this builder so far.</returns>
    public IUpdater Build() => _updater;

    /// <summary>
    /// Sets the updater's <see cref="Autodesk.Revit.DB.IUpdater.GetAdditionalInformation"/> value.
    /// </summary>
    /// <param name="additionalInformation">The additional information text to report.</param>
    /// <returns>This builder, for chaining.</returns>
    public UpdaterBuilder SetAdditionalInformation(string additionalInformation)
    {
        _updater.AdditionalInformation = additionalInformation;
        return this;
    }

    /// <summary>
    /// Sets the updater's <see cref="Autodesk.Revit.DB.IUpdater.GetChangePriority"/> value.
    /// </summary>
    /// <param name="changePriority">The change priority to report.</param>
    /// <returns>This builder, for chaining.</returns>
    public UpdaterBuilder SetChangePriority(ChangePriority changePriority)
    {
        _updater.ChangePriority = changePriority;
        return this;
    }

    /// <summary>
    /// Sets the updater's <see cref="Autodesk.Revit.DB.IUpdater.GetUpdaterId"/> value.
    /// </summary>
    /// <param name="updaterId">The unique id to report; must be registered before use.</param>
    /// <returns>This builder, for chaining.</returns>
    public UpdaterBuilder SetUpdaterId(UpdaterId updaterId)
    {
        _updater.UpdaterId = updaterId;
        return this;
    }

    /// <summary>
    /// Sets the updater's <see cref="Autodesk.Revit.DB.IUpdater.GetUpdaterName"/> value.
    /// </summary>
    /// <param name="name">The display name to report.</param>
    /// <returns>This builder, for chaining.</returns>
    public UpdaterBuilder SetUpdaterName(string name)
    {
        _updater.UpdaterName = name;
        return this;
    }

    /// <summary>
    /// Sets the action that runs when Revit invokes <see cref="Autodesk.Revit.DB.IUpdater.Execute"/>
    /// on the built updater.
    /// </summary>
    /// <param name="action">The logic to run, receiving the <see cref="Autodesk.Revit.DB.UpdaterData"/> Revit passes in.</param>
    /// <returns>This builder, for chaining.</returns>
    public UpdaterBuilder SetAction(Action<UpdaterData> action)
    {
        _updater.Action = action;
        return this;
    }
}
