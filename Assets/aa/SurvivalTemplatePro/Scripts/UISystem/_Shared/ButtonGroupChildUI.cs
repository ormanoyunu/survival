using System;
using UnityEngine;
using UnityEngine.UI;

namespace SurvivalTemplatePro.UISystem
{
    [RequireComponent(typeof(Button))]
    public class ButtonGroupChildUI : MonoBehaviour
    {
        public event Action<ButtonGroupChildUI> OnClick;

        [SerializeField]
        private GameObject m_SelectedGraphic;

        private Button m_Button;


        public void SelectButton(bool select)
        {
            m_SelectedGraphic.SetActive(select);
        }

        private void Awake()
        {
            m_Button = GetComponent<Button>();
            m_Button.onClick.AddListener(()=> OnClick.Invoke(this));
        }
    }
}