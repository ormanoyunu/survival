using UnityEngine;
using UnityEngine.UI;

namespace SurvivalTemplatePro.UISystem
{
    public class RequirementUI : MonoBehaviour
    {
        [SerializeField]
        private Image m_Icon;

        [SerializeField]
        private Text m_Amount;


        public void Display(Sprite icon, string amount, Color textColor)
        {
            m_Icon.sprite = icon;
            m_Amount.text = amount;
            m_Amount.color = textColor;
        }
    }
}