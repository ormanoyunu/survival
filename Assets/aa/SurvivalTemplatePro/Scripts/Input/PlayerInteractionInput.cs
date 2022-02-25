using UnityEngine;
using UnityEngine.InputSystem;

namespace SurvivalTemplatePro.InputSystem
{
    public class PlayerInteractionInput : CharacterBehaviour
    {
        [SerializeField]
        private bool m_EnableOnStart = true;

        [SerializeField]
        private InputActionReference m_InteractInput;

        private IInteractionHandler m_InteractionHandler;


        public override void OnInitialized()
        {
            GetModule(out m_InteractionHandler);

            if (m_EnableOnStart)
                m_InteractInput.action.Enable();
        }

        private void OnEnable()
        {
            m_InteractInput.action.started += OnInteractStart;
            m_InteractInput.action.canceled += OnInteractEnd;
        }

        private void OnDisable()
        {
            m_InteractInput.action.started -= OnInteractStart;
            m_InteractInput.action.canceled -= OnInteractEnd;
        }

        private void OnInteractStart(InputAction.CallbackContext context)
        {
            m_InteractionHandler.StartInteraction();
        }

        private void OnInteractEnd(InputAction.CallbackContext obj) => m_InteractionHandler.EndInteraction();
    }
}
