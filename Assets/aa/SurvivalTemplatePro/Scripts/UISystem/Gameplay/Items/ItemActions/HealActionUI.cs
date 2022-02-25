using SurvivalTemplatePro.InventorySystem;
using UnityEngine;

namespace SurvivalTemplatePro.UISystem
{
    public class HealActionUI : ItemActionUI
    {
        [Space]

        [SerializeField]
        private ItemPropertyReference m_HealthRestoreProperty;


        public override bool IsViableForItem(ItemSlot itemSlot) => itemSlot.Item.HasProperty(m_HealthRestoreProperty);
        public override bool CanPerformAction(ItemSlot itemSlot) => (Player.HealthManager.MaxHealth - Player.HealthManager.Health) < 0.01f;

        protected override void PerformAction(ItemSlot itemSlot)
        {
            float healthRestore = itemSlot.Item.GetProperty(m_HealthRestoreProperty).Float;
            Player.HealthManager.RestoreHealth(healthRestore);

            itemSlot.RemoveFromStack(1);
        }
    }
}