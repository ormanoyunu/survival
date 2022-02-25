using UnityEngine;
using UnityEngine.InputSystem;

namespace SurvivalTemplatePro.InputSystem
{
    public class PlayerLookInput : CharacterBehaviour
    {
        [SerializeField]
        private bool m_EnableOnStart = true;

        [SerializeField]
        private InputActionReference m_LookInput;

        private ILookHandler m_LookHandler;


        public override void OnInitialized()
        {
            GetModule(out m_LookHandler);

            if (m_EnableOnStart)
                m_LookInput.action.Enable();
        }

        private void LateUpdate()
        {
            if (!IsInitialized)
                return;

            m_LookHandler.UpdateLook(m_LookInput.action.ReadValue<Vector2>());
        }
    }
}