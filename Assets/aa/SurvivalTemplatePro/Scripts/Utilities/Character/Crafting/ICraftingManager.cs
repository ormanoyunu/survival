using SurvivalTemplatePro.InventorySystem;
using UnityEngine.Events;

namespace SurvivalTemplatePro
{
    public interface ICraftingManager : ICharacterModule
    {
        bool IsCrafting { get; }

        event UnityAction<ItemInfo> onCraftingStart;
        event UnityAction onCraftingEnd;

        void Craft(ItemInfo itemInfo);
        void CancelCrafting();
    }
}