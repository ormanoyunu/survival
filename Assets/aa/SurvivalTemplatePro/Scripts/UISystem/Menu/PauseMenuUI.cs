using UnityEngine;
using UnityEngine.InputSystem;

namespace SurvivalTemplatePro.UISystem
{
    public class PauseMenuUI : PlayerUIBehaviour
    {
        [SerializeField]
        private InputActionReference m_PauseInput;

        [Space]

        [SerializeField]
        private PanelUI m_Panel;

        [SerializeField]
        private SceneField m_MainMenuScene;

        private float m_PanelToggleTimer;
        private bool m_IsActive = false;

        private IPauseHandler m_PauseHandler;


        public void QuitToMenu() => LevelManager.LoadScene(m_MainMenuScene);
        public void QuitToDesktop() => Application.Quit();

        public void TogglePauseMenu()
        {
            if (Time.time > m_PanelToggleTimer && ((m_PauseHandler.PauseActive && m_IsActive) || (!m_PauseHandler.PauseActive && !m_IsActive)))
            {
                m_IsActive = !m_IsActive;
                m_Panel.Show(m_IsActive);

                if (m_IsActive)
                    m_PauseHandler.RegisterLocker(this, new PlayerPauseParams(true, true, true, true));
                else
                    m_PauseHandler.UnregisterLocker(this);

                m_PanelToggleTimer = Time.time + 0.3f;
            }
        }

        private void TogglePauseMenu(InputAction.CallbackContext context) => TogglePauseMenu();

        public override void OnAttachment()
        {
            GetModule(out m_PauseHandler);

            m_PauseInput.action.Enable();
            m_PauseInput.action.performed += TogglePauseMenu;
        }

        public override void OnDetachment()
        {
            m_PauseInput.action.Disable();
            m_PauseInput.action.performed -= TogglePauseMenu;
        }
    }
}