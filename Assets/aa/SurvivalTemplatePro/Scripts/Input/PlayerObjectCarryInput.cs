using UnityEngine;
using UnityEngine.InputSystem;

namespace SurvivalTemplatePro.InputSystem
{
    public class PlayerObjectCarryInput : CharacterBehaviour
    {
        [SerializeField]
        private bool m_EnableOnStart = true;

        [SerializeField]
        private InputActionReference m_AltInteractInput;

        [SerializeField]
        private InputActionReference m_DropInput;

        private IObjectCarryController m_ObjectCarryController;


        public override void OnInitialized()
        {
            GetModule(out m_ObjectCarryController);

            if (m_EnableOnStart)
            {
                m_AltInteractInput.action.Enable();
                m_DropInput.action.Enable();
            }
        }

        private void OnEnable()
        {
            m_AltInteractInput.action.started += OnUseCarriedObjectPerformed;
            m_DropInput.action.started += OnDropCarriedObjectsPerformed;
        }

        private void OnDisable()
        {
            m_AltInteractInput.action.started -= OnUseCarriedObjectPerformed;
            m_DropInput.action.started -= OnDropCarriedObjectsPerformed;
        }

        private void OnDropCarriedObjectsPerformed(InputAction.CallbackContext obj) => m_ObjectCarryController.DropCarriedObjects(1);
        private void OnUseCarriedObjectPerformed(InputAction.CallbackContext obj) => m_ObjectCarryController.UseCarriedObject();
    }
}