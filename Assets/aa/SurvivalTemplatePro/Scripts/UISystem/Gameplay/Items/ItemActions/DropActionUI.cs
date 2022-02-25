using SurvivalTemplatePro.InventorySystem;

namespace SurvivalTemplatePro.UISystem
{
    public class DropActionUI : ItemActionUI
    {
        public override bool IsViableForItem(ItemSlot itemSlot) => true;
        public override bool CanPerformAction(ItemSlot itemSlot) => true;

        protected override void PerformAction(ItemSlot itemSlot)
        {
            if (Player.TryGetModule(out IItemDropHandler itemDropHandler))
                itemDropHandler.DropItem(itemSlot);
        }
    }
}