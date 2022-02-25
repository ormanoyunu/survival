using UnityEngine.Events;

namespace SurvivalTemplatePro
{
    public interface IInventoryInspectManager : ICharacterModule
    {
        InventoryInspectState InspectState { get; }
        IExternalContainer ExternalContainer { get; }
        float LastInspectionTime { get; }

        event UnityAction<InventoryInspectState> onInspectStarted;
        event UnityAction onInspectEnded;

        bool TryInspect(InventoryInspectState inspectState, IExternalContainer container = null);
        bool TryStopInspecting(bool forceStop = false);
    }

    public enum InventoryInspectState { None, Default, External }
}