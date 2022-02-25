using System;
using UnityEngine;
using UnityEngine.UI;

namespace SurvivalTemplatePro.UISystem
{
    [Serializable]
    public class BuildMaterialUI : MonoBehaviour
    {
        [SerializeField]
        private Image m_Icon;

        [SerializeField]
        private Text m_Amount;


        public void Display(Sprite icon, string amount)
        {
            m_Icon.sprite = icon;
            m_Amount.text = amount;
        }
    }
}