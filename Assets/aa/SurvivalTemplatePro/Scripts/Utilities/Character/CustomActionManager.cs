using UnityEngine;
using UnityEngine.Events;

namespace SurvivalTemplatePro
{
    public class CustomActionManager : CharacterBehaviour, ICustomActionManager
    {
        public bool CustomActionActive => m_ActionActive;

        public event UnityAction<CustomActionParams> onActionStart;
        public event UnityAction onActionEnd;

        private IPauseHandler m_PauseHandler;
        private CustomActionParams m_CustomAction;
        private float m_ActionEndTime;
        private bool m_ActionActive;


        public override void OnInitialized()
        {
            TryGetModule(out m_PauseHandler);
        }

        public void StartAction(CustomActionParams actionParams)
        {
            if (CustomActionActive)
                EndCurrentAction(true);

            m_ActionActive = true;

            m_PauseHandler.RegisterLocker(this, new PlayerPauseParams(false, true, true, true));

            m_CustomAction = actionParams;
            m_ActionEndTime = actionParams.EndTime;
            onActionStart?.Invoke(actionParams);
        }

        public void TryCancelAction()
        {
            if (CustomActionActive && m_CustomAction.CanCancel)
                EndCurrentAction(true);
        }

        private void Update()
        {
            if (CustomActionActive && Time.time > m_ActionEndTime)
                EndCurrentAction(false);
        }

        private void EndCurrentAction(bool cancel)
        {
            m_PauseHandler.UnregisterLocker(this);

            if (!cancel)
                m_CustomAction.OnCompleteCallbacks?.Invoke();
            else
                m_CustomAction.OnCanceledCallbacks?.Invoke();

            onActionEnd?.Invoke();
            m_ActionActive = false;
        }
    }
}