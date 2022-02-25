using SurvivalTemplatePro.InventorySystem;
using System.Collections.Generic;
using UnityEngine;

namespace SurvivalTemplatePro.UISystem
{
    public class BookCraftingUI : BookCategoryUI
    {
        [SerializeField]
        private CraftingSlotUI m_Template;

        [SerializeField]
        private RectTransform m_TemplateSpawnRect;

        [SerializeField, Range(5, 20)]
        private int m_TemplateInstanceCount = 10;

        private List<CraftingSlotUI> m_CachedSlots = new List<CraftingSlotUI>();
        private ICraftingManager m_CraftingManager;


        public override void AttachToPlayer(ICharacter player)
        {
            base.AttachToPlayer(player);

            player.TryGetModule(out m_CraftingManager);
        }

        public override void Select()
        {
            base.Select();

            Player.Inventory.onContainerChanged += UpdateCraftRequirments;
            UpdateCraftRequirments(null);
        }

        public override void Deselect()
        {
            base.Deselect();

            if (Player != null)
            {
                Player.Inventory.onContainerChanged -= UpdateCraftRequirments;
                m_CraftingManager.CancelCrafting();
            }
        }

        protected override void Awake()
        {
            base.Awake();

            int remainingSlots = m_TemplateInstanceCount;

            foreach (var category in ItemDatabase.GetAllCategories())
            {
                foreach (var item in category.Items)
                {
                    if (remainingSlots == 0)
                        break;

                    if (item.Crafting.IsCraftable)
                    {
                        m_CachedSlots.Add(Instantiate(m_Template.gameObject, m_TemplateSpawnRect).GetComponent<CraftingSlotUI>());

                        int currentIndex = m_TemplateInstanceCount - remainingSlots;
                        m_CachedSlots[currentIndex].onClick += StartCrafting;
                        m_CachedSlots[currentIndex].DisplayItem(item);

                        remainingSlots--;
                    }
                }
            }
        }

        private void UpdateCraftRequirments(ItemSlot slot)
        {
            for (int i = 0; i < m_CachedSlots.Count; i++)
                m_CachedSlots[i].UpdateRequirementsUI(Player.Inventory);
        }

        private void StartCrafting(ItemInfo itemInfo) => m_CraftingManager.Craft(itemInfo);
    }
}