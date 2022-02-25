using UnityEngine;
using UnityEngine.UI;

namespace SurvivalTemplatePro.UISystem
{
    public class WheelSlotUI : ItemSlotUI
    {
        public enum SelectionGraphicState { Normal, Highlighted }

        [BHeader("Item Wheel Slot")]

        [SerializeField]
        private Image m_SelectionGraphic;

        [SerializeField]
        private Color m_SelectionGraphicColor;

        [SerializeField]
        private Color m_SelectionGraphicHighlightedColor;


        public void SetSlotHighlights(SelectionGraphicState state)
        {
            if (state == SelectionGraphicState.Normal)
            {
                m_SelectionGraphic.enabled = m_Selected;
                m_SelectionGraphic.color = m_SelectionGraphicColor;
            }
            else if (state == SelectionGraphicState.Highlighted)
            {
                m_SelectionGraphic.enabled = true;
                m_SelectionGraphic.color = m_SelectionGraphicHighlightedColor;
            }
        }

        protected override void Awake()
        {
            base.Awake();

            m_SelectionGraphic.enabled = false;
        }
    }
}