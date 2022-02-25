using SurvivalTemplatePro.InventorySystem;
using UnityEngine;

namespace SurvivalTemplatePro.UISystem
{
    public class ItemAutoMover : PlayerUIBehaviour
    {
        [SerializeField]
        [Tooltip("All of the item tag(s) that correspond to the wieldables.")]
        private ItemTagReference[] m_WieldableTags;

        [SerializeField]
        [Tooltip("All of the clothing tag(s) that correspond to clothing items.")]
        private ItemTagReference[] m_ClothingTags;

        private IInventoryInspectManager m_InventoryInspectManager;


        public override void OnAttachment()
        {
            TryGetModule(out m_InventoryInspectManager);
        }

        /// <summary>
        /// Called from the inspector using a Unity Event
        /// </summary>
        public void TryAutoMove(ItemSlotUI itemSlotUI)
        {
            if (itemSlotUI.Item == null)
                return;

			ItemSlot slot = itemSlotUI.ItemSlot;
			ItemContainer container = itemSlotUI.Parent.ItemContainer;
            ItemContainerFlags containerFlag = itemSlotUI.Parent.ItemContainer.Flag;

            ItemContainerFlags primaryFlag = 0;
			ItemContainerFlags secondaryFlag = 0;
			ItemContainerFlags tertiaryFlag = 0;

            bool externalContainerActive = m_InventoryInspectManager.ExternalContainer != null;
            bool moveToExternalContainer = false;

            if (externalContainerActive)
            {
                // Move from external 
                if (containerFlag == ItemContainerFlags.External)
                {
                    primaryFlag = ItemContainerFlags.Storage;

                    if (IsWieldable(slot))
                        secondaryFlag = ItemContainerFlags.Holster;
                }
                else
                {
                    // Move to external
                    primaryFlag = ItemContainerFlags.External;
                    moveToExternalContainer = true;
                }
            }
            else
            {
                // Move from storage 
                if (containerFlag == ItemContainerFlags.Storage)
                {
                    if (IsClothing(slot))
                    {
                        primaryFlag = ItemContainerFlags.Equipment;
                        secondaryFlag = ItemContainerFlags.Holster;
                    }
                    else if (IsWieldable(slot))
                        primaryFlag = ItemContainerFlags.Holster;
                }
                // Move from holster 
                else if (containerFlag == ItemContainerFlags.Holster)
                {
                    primaryFlag = ItemContainerFlags.Storage;
                    secondaryFlag = ItemContainerFlags.External;
                }
                // Move from equipment 
                else if (containerFlag == ItemContainerFlags.Equipment)
                {
                    primaryFlag = ItemContainerFlags.Storage;
                    secondaryFlag = ItemContainerFlags.External;
                }
            }

            // Add to target container
            if (!moveToExternalContainer)
            {
                bool added = PlayerInventory.AddOrSwap(container, slot, primaryFlag);

                if (!added)
                    added = PlayerInventory.AddOrSwap(container, slot, secondaryFlag);

                if (!added)
                    PlayerInventory.AddOrSwap(container, slot, tertiaryFlag);
            }
            else
            {
                var externalContainer = m_InventoryInspectManager.ExternalContainer.ItemContainer;

                bool added = externalContainer.AddOrSwap(container, slot);

                if (!added)
                    added = externalContainer.AddOrSwap(container, slot);

                if (!added)
                    externalContainer.AddOrSwap(container, slot);
            }
		}

        private bool IsClothing(ItemSlot slot)
		{
            ItemInfo info = slot.Item.Info;

            for (int i = 0; i < m_ClothingTags.Length; i++)
            {
				if (info.CompareTag(m_ClothingTags[i]))
					return true;
            }

			return false;
		}

        private bool IsWieldable(ItemSlot slot)
        {
            ItemInfo info = slot.Item.Info;

            for (int i = 0; i < m_WieldableTags.Length; i++)
            {
                if (info.CompareTag(m_WieldableTags[i]))
                    return true;
            }

            return false;
        }
    }
}