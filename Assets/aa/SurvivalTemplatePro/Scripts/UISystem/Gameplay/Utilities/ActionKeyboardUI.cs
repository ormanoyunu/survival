using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace SurvivalTemplatePro.UISystem
{
    public class ActionKeyboardUI : MonoBehaviour
    {
        [SerializeField]
        private InputActionReference m_InputAction;

        [SerializeField]
        private Text m_Text;

        [SerializeField]
        private string m_TextTemplate = "{}";

        [SerializeField]
        private Color m_TextInputColor = Color.white;


        private void Start()
        {
            if (m_InputAction != null)
                SetText();

            Destroy(this);
        }

        private void SetText()
        {
            if (m_Text != null)
                m_Text.text = m_TextTemplate.Replace("{}", $"<color={ColorToHex(m_TextInputColor)}>{m_InputAction.action.bindings[0].ToDisplayString()}</color>");
        }

        private void OnValidate() 
        {
            m_Text = GetComponent<Text>();

            if (!m_TextTemplate.Contains("{}")) m_TextTemplate += "{}";
            if (m_InputAction != null) SetText();
        }

        private string ColorToHex(Color32 color)
        {
            string hex = "#" + color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");

            return hex;
        }
    }
}