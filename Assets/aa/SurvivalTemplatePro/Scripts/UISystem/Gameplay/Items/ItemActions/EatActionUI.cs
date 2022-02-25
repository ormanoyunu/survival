using SurvivalTemplatePro.InventorySystem;
using UnityEngine;

namespace SurvivalTemplatePro.UISystem
{
    public class EatActionUI : ItemActionUI
    {
        [Space]

        [SerializeField]
        private ItemPropertyReference m_HungerRestoreProperty;

        [SerializeField]
        private ItemPropertyReference m_ThirstRestoreProperty;

        private IHungerManager m_HungerManager;
        private IThirstManager m_ThirstManager;


        public override void OnAttachment()
        {
            GetModule(out m_HungerManager);
            GetModule(out m_ThirstManager);
        }

        public override bool IsViableForItem(ItemSlot itemSlot) => itemSlot.Item.HasProperty(m_HungerRestoreProperty) || itemSlot.Item.HasProperty(m_ThirstRestoreProperty);
        public override bool CanPerformAction(ItemSlot itemSlot) => !m_HungerManager.HasMaxHunger || !m_ThirstManager.HasMaxThirst;

        protected override void PerformAction(ItemSlot itemSlot)
        {
            if (itemSlot.Item.TryGetProperty(m_HungerRestoreProperty, out var hungerRestore))
                m_HungerManager.Hunger += hungerRestore.Float;

            if (itemSlot.Item.TryGetProperty(m_ThirstRestoreProperty, out var thirstRestore))
                m_ThirstManager.Thirst += thirstRestore.Float;

            itemSlot.RemoveFromStack(1);
        }
    }
}