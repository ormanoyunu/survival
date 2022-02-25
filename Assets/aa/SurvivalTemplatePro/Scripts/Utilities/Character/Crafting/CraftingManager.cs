using SurvivalTemplatePro.InventorySystem;
using UnityEngine.Events;
using UnityEngine;

namespace SurvivalTemplatePro
{
    public class CraftingManager : CharacterBehaviour, ICraftingManager
    {
        public bool IsCrafting => m_CurrentItemToCraft != null;

        public event UnityAction<ItemInfo> onCraftingStart;
        public event UnityAction onCraftingEnd;

        [SerializeField]
        [Tooltip("Craft Sound: Sound that will be played after crafting an item.")]
        private StandardSound m_CraftSound;

        private ItemInfo m_CurrentItemToCraft;

        private IInventory m_Inventory;
        private IItemDropHandler m_ItemDropHandler;
        private ICustomActionManager m_ActionManager;


        public override void OnInitialized()
        {
            TryGetModule(out m_ActionManager);
            TryGetModule(out m_ItemDropHandler);
            TryGetModule(out m_Inventory);
        }

        public void Craft(ItemInfo itemInfo)
        {
            if (IsCrafting)
                return;

            var blueprint = itemInfo.Crafting.Blueprint;

            // Verify if all of the blueprint crafting materials exist in the inventory 
            for (int i = 0; i < blueprint.Length; i++)
            {
                int itemCount = m_Inventory.GetItemCount(blueprint[i].Item, ItemContainerFlags.Storage);

                if (itemCount < blueprint[i].Amount)
                    return;
            }

            // Get craft duration
            float craftDuration = itemInfo.Crafting.CraftDuration;

            // Start crafting
            m_CurrentItemToCraft = itemInfo;
            CustomActionParams craftingParams = new CustomActionParams("Crafting", "Crafting <b>" + itemInfo.Name + "</b>...", craftDuration, true, OnCraftItemEnd, OnCraftCancel);
            m_ActionManager.StartAction(craftingParams);

            onCraftingStart?.Invoke(itemInfo);

            Character.AudioPlayer.PlaySound(m_CraftSound);
        }

        public void CancelCrafting()
        {
            if (IsCrafting)
                m_ActionManager.TryCancelAction();
        }

        private void OnCraftItemEnd()
        {
            if (!IsCrafting)
                return;

            var blueprint = m_CurrentItemToCraft.Crafting.Blueprint;

            // Remove the blueprint items from the inventory
            for (int i = 0; i < blueprint.Length; i++)
            {
                int removedCount = m_Inventory.RemoveItems(blueprint[i].Item, blueprint[i].Amount, ItemContainerFlags.Storage);

                if (removedCount < blueprint[i].Amount)
                    return;
            }

            // if the crafted item couldn't be added to the inventory, spawn the world prefab.
            if (m_Inventory.AddItems(m_CurrentItemToCraft.Id, 1, ItemContainerFlags.Storage) == 0)
                m_ItemDropHandler.DropItem(new Item(m_CurrentItemToCraft));

            m_CurrentItemToCraft = null;
            onCraftingEnd?.Invoke();
        }

        private void OnCraftCancel()
        {
            m_CurrentItemToCraft = null;
            onCraftingEnd?.Invoke();
        }
    }
}