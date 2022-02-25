using SurvivalTemplatePro.InventorySystem;

namespace SurvivalTemplatePro
{
    public interface IItemDropHandler : ICharacterModule
    {
        void DropItem(Item itemToDrop, float dropDelay = 0f);
        void DropItem(ItemSlot itemSlot, float dropDelay = 0f);
    }
}