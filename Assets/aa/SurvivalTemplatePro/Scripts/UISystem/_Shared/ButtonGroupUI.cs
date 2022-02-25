using UnityEngine;

namespace SurvivalTemplatePro.UISystem
{
    public class ButtonGroupUI : MonoBehaviour
    {
        [SerializeField]
        private SoundPlayer m_SelectAudio;

        private ButtonGroupChildUI[] m_Buttons;
        private ButtonGroupChildUI m_SelectedButton;


        public void SelectBtnAtIndex(int index)
        {
            if (m_Buttons.Length > 0)
                SelectButton(m_Buttons[Mathf.Clamp(index, 0, m_Buttons.Length)]);
        }

        private void Awake()
        {
            m_Buttons = GetComponentsInChildren<ButtonGroupChildUI>();

            if (m_Buttons != null && m_Buttons.Length > 0)
            {
                m_SelectedButton = m_Buttons[0];
                m_SelectedButton.SelectButton(true);
            }

            foreach(var btn in m_Buttons)
                btn.OnClick += SelectButton;
        }

        private void SelectButton(ButtonGroupChildUI button)
        {
            m_SelectedButton.SelectButton(false);

            button.SelectButton(true);
            m_SelectedButton = button;

            m_SelectAudio.Play2D();
        }
    }
}