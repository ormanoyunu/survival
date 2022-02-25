using SurvivalTemplatePro.WieldableSystem;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SurvivalTemplatePro.InputSystem
{
    public class PlayerWieldablesInput : CharacterBehaviour
    {
        [SerializeField]
        private bool m_EnableOnStart = true;

        [SerializeField]
        private InputActionReference m_UseInput;

        [SerializeField]
        private InputActionReference m_AimInput;

        [SerializeField]
        private InputActionReference m_ReloadInput;

        [SerializeField]
        private InputActionReference m_DropInput;

        [SerializeField]
        private InputActionReference m_ToggleSurvivalBook;

        private IWieldablesController m_Controller;
        private IItemDropHandler m_InventoryDropHandler;
        private IWieldableSurvivalBookHandler m_SurvivalBookHandler;

        private IAimHandler m_AimHandler;
        private IUseHandler m_UseHandler;
        private IReloadHandler m_ReloadHandler;


        public override void OnInitialized()
        {
            GetModule(out m_Controller);
            GetModule(out m_InventoryDropHandler);
            GetModule(out m_SurvivalBookHandler);

            m_Controller.onWieldableEquipped += OnWieldableChanged;

            if (m_EnableOnStart)
            {
                m_UseInput.action.Enable();
                m_AimInput.action.Enable();
                m_ReloadInput.action.Enable();
                m_DropInput.action.Enable();
                m_ToggleSurvivalBook.action.Enable();
            }
        }

        private void OnWieldableChanged(IWieldable wieldable)
        {
            if (wieldable == null)
            {
                m_AimHandler = null;
                m_UseHandler = null;
                m_ReloadHandler = null;
            }
            else
            {
                m_AimHandler = wieldable.GetComponent<IAimHandler>();
                m_UseHandler = wieldable.GetComponent<IUseHandler>();
                m_ReloadHandler = wieldable.GetComponent<IReloadHandler>();
            }
        }

        private void OnEnable()
        {
            m_DropInput.action.started += OnDropActionPerformed;
            m_ToggleSurvivalBook.action.started += OnSurvivalBookTogglePerfomed;
            m_ReloadInput.action.started += TryStartReload;
        }

        private void OnDisable()
        {
            m_DropInput.action.started -= OnDropActionPerformed;
            m_ToggleSurvivalBook.action.started -= OnSurvivalBookTogglePerfomed;
            m_ReloadInput.action.started -= TryStartReload;
        }

        private void OnSurvivalBookTogglePerfomed(InputAction.CallbackContext obj) => m_SurvivalBookHandler.ToggleInspection();

        private void OnDropActionPerformed(InputAction.CallbackContext obj)
        {
            if (m_Controller.ActiveWieldable != null && m_Controller.ActiveWieldable.AttachedItem != null)
                m_InventoryDropHandler.DropItem(m_Controller.ActiveWieldable.AttachedItem, 0.35f);
        }

        private void TryStartReload(InputAction.CallbackContext obj)
        {
            if (m_ReloadHandler != null)
                m_ReloadHandler.StartReloading();
        }

        private void Update()
        {
            if (m_UseHandler != null)
                HandleUseInput();

            if (m_AimHandler != null)
                HandleAimInput();
        }

        private void HandleUseInput() 
        {
            if (m_UseInput.action.triggered)
                m_UseHandler.Use(UsePhase.Start);
            else if (m_UseInput.action.ReadValue<float>() > 0.001f)
                m_UseHandler.Use(UsePhase.Hold);
            else if (m_UseInput.action.WasReleasedThisFrame() || !m_UseInput.action.enabled)
                m_UseHandler.Use(UsePhase.End);
        }

        private void HandleAimInput() 
        {
            if (m_AimInput.action.ReadValue<float>() > 0.001f)
            {
                if (!m_AimHandler.IsAiming)
                    m_AimHandler.StartAiming();
            }
            else if (m_AimHandler.IsAiming)
                m_AimHandler.EndAiming();
        }
    }
}