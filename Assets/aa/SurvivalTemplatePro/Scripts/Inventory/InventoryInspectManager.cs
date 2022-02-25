using UnityEngine;
using UnityEngine.Events;

namespace SurvivalTemplatePro
{
    /// <summary>
    /// Handles any type of inventory inspection (e.g. Backpack, external containers etc.)
    /// </summary>
    public class InventoryInspectManager : CharacterBehaviour, IInventoryInspectManager
    {
        public InventoryInspectState InspectState { get; private set; }
        public IExternalContainer ExternalContainer { get; private set; }
        public float LastInspectionTime => m_NextAllowedToggleTime - m_ToggleThreshold;

        public event UnityAction<InventoryInspectState> onInspectStarted;
        public event UnityAction onInspectEnded;

        [SerializeField, Range(0f, 10f)]
        [Tooltip("How often can the inventory inspection be toggled (e.g. open/close backpack).")]
        private float m_ToggleThreshold = 0.35f;

        private float m_NextAllowedToggleTime;
        private IPauseHandler m_PauseHandler;


        public override void OnInitialized() => GetModule(out m_PauseHandler);

        public bool TryInspect(InventoryInspectState inspectState, IExternalContainer container)
        {
            if (m_PauseHandler.PauseActive || Time.time < m_NextAllowedToggleTime || InspectState == inspectState)
                return false;

            m_NextAllowedToggleTime = Time.time + m_ToggleThreshold;

            InspectState = inspectState;
            ExternalContainer = container;

            m_PauseHandler.RegisterLocker(this, new PlayerPauseParams(true, true, true, true));
            onInspectStarted?.Invoke(inspectState);

            return true;
        }

        public bool TryStopInspecting(bool forceStop = false)
        {
            if (!m_PauseHandler.PauseActive || Time.time < m_NextAllowedToggleTime && !forceStop)
                return false;

            m_NextAllowedToggleTime = Time.time + m_ToggleThreshold;

            InspectState = InventoryInspectState.None;
            ExternalContainer = null;

            m_PauseHandler.UnregisterLocker(this);
            onInspectEnded?.Invoke();

            return true;
        }
    }
}