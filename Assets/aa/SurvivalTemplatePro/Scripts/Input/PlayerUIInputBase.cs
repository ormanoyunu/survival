using SurvivalTemplatePro.UISystem;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SurvivalTemplatePro.InputSystem
{
    public abstract class PlayerUIInputBase : PlayerUIBehaviour
    {
        protected bool m_ActionsEnabled;
        private int m_BlockedCount;


        public override void OnAttachment() => OnEnable();

        private void OnEnable()
        {
            if (!IsInitialized)
                return;

            m_ActionsEnabled = true;

            AddActionListeners();
        }

        private void OnDisable()
        {
            if (!IsInitialized)
                return;

            EnableActions(false);
            m_ActionsEnabled = false;

            RemoveActionListeners();
        }

        protected void RemoveInputBlocker()
        {
            m_BlockedCount = Mathf.Max(m_BlockedCount - 1, 0);

            if (!m_ActionsEnabled && m_BlockedCount == 0)
            {
                EnableActions(true);
                m_ActionsEnabled = true;
            }
        }

        protected void AddInputBlocker()
        {
            m_BlockedCount = Mathf.Max(m_BlockedCount + 1, 0);

            if (m_ActionsEnabled)
            {
                EnableActions(false);
                m_ActionsEnabled = false;
            }
        }

        protected abstract void EnableActions(bool enable);
        protected abstract void AddActionListeners();
        protected abstract void RemoveActionListeners();

        protected void EnableAction(InputAction action, bool enable)
        {
            if (enable)
                action.Enable();
            else
                action.Disable();
        }
    }
}