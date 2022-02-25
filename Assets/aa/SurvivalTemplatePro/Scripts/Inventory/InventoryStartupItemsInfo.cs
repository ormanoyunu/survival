using System;
using UnityEngine;

namespace SurvivalTemplatePro.InventorySystem
{
    [CreateAssetMenu(menuName = "Survival Template Pro/Inventory/Startup Items")]
    public class InventoryStartupItemsInfo : ScriptableObject
    {
        #region Internal
        [Serializable]
        public class ItemContainerStartupItems
        {
            public string Name;
            public ItemGenerator[] StartupItems;
        }
        #endregion

        [SerializeField]
        private ItemContainerStartupItems[] m_ItemContainersStartupItems;


        public void AddItemsToInventory(IInventory inventory)
        {
            foreach (var container in m_ItemContainersStartupItems)
            {
                ItemContainer itemContainer = inventory.GetContainerWithName(container.Name);

                if (itemContainer != null)
                {
                    foreach (var item in container.StartupItems)
                    {
                        if (item.Method == ItemGenerationMethod.Specific)
                            itemContainer.AddItem(item.SpecificItem, item.Count);
                        else if (item.Method == ItemGenerationMethod.RandomFromCategory)
                        {
                            ItemInfo itemInfo = ItemDatabase.GetRandomItemFromCategory(item.Category);

                            if (itemInfo != null)
                                itemContainer.AddItem(itemInfo.Id, item.Count);
                        }
                        else if (item.Method == ItemGenerationMethod.Random)
                        {
                            var category = ItemDatabase.GetRandomCategory();

                            if (category != null)
                            {
                                ItemInfo itemInfo = ItemDatabase.GetRandomItemFromCategory(category.Name);

                                if (itemInfo != null)
                                    itemContainer.AddItem(itemInfo.Id, item.Count);
                            }
                        }
                    }
                }
            }
        }
    }
}