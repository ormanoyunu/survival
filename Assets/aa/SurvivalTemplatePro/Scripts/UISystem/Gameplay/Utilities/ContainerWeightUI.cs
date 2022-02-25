using SurvivalTemplatePro.InventorySystem;
using UnityEngine;
using UnityEngine.UI;

namespace SurvivalTemplatePro.UISystem
{
    public class ContainerWeightUI : PlayerUIBehaviour
    {
        [SerializeField]
        private string m_ContainerName;

        [SerializeField]
        private int m_Decimals = 1;

        [SerializeField]
        private Text m_WeightText;

        [SerializeField]
        private Image m_WeightBar;

        private ItemContainer m_Container;


        public override void OnAttachment()
        {
            m_Container = Player.Inventory.GetContainerWithName(m_ContainerName);

            if (m_Container != null)
            {
                m_Container.onChanged += OnContainerChanged;
                OnContainerChanged(null);
            }
        }    

        private void OnContainerChanged(ItemSlot slot)
        {
            float weight = 0f;

            for (int i = 0;i < m_Container.Count;i++)
            {
                if (m_Container[i].HasItem)
                    weight += m_Container[i].Item.Info.Weight * m_Container[i].Item.CurrentStackSize;
            }

            m_WeightText.text = string.Format("{0} / {1} KG", (float)System.Math.Round(weight, m_Decimals), m_Container.MaxWeight);
            m_WeightBar.fillAmount = weight / m_Container.MaxWeight;
        }
    }
}