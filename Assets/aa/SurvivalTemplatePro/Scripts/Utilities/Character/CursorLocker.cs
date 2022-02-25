using UnityEngine;

namespace SurvivalTemplatePro
{
    public class CursorLocker : CharacterBehaviour
    {
        [SerializeField, Tooltip("If checked, a button will show up which can lock the cursor.")]
        private bool m_ShowLockButton = true;

        [SerializeField, Tooltip("If checked, you can unlock the cursor by pressing the Escape / Esc key.")]
        private bool m_CanUnlock = true;

        private bool m_EditorPause = false;


        public void ChangeCursorVisibility(PlayerPauseParams pauseParams)
        {
            if (!enabled)
                return;

            ChangeCursorVisibility(pauseParams.UnlockCursor);
        }

        public void ChangeCursorVisibility(bool enable)
        {
            if (!enabled)
                return;

            Cursor.visible = enable;
            Cursor.lockState = enable ? CursorLockMode.None : CursorLockMode.Locked;
        }

        private void Start()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void OnGUI()
        {
            if (!m_ShowLockButton)
                return;

            Vector2 buttonSize = new Vector2(256f, 24f);

            // Note: While in the editor, pressing Escape will always unlock the cursor.
            if (Event.current.type == EventType.KeyDown && (m_CanUnlock || Application.isEditor))
            {
                if (Event.current.keyCode == KeyCode.Escape)
                {
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                    m_EditorPause = false;
                }
            }

            if (Cursor.lockState == CursorLockMode.None && !m_EditorPause)
            {
                if (GUI.Button(new Rect(Screen.width * 0.5f - buttonSize.x / 2f, 16f, buttonSize.x, buttonSize.y), "Lock Cursor (Hit 'Esc' to unlock)"))
                {
                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.Locked;
                    m_EditorPause = true;
                }
            }
        }
    }
}