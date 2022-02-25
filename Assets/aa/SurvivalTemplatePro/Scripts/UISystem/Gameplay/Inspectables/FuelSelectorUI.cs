using SurvivalTemplatePro.InventorySystem;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SurvivalTemplatePro.UISystem
{
    public class FuelSelectorUI : PlayerUIBehaviour
    {
        #region Internal
        public class FuelItem
        {
            public int Id;
            public int Count;
            public int Duration;


            public FuelItem(int id, int count, int duration)
            {
                Id = id;
                Count = count;
                Duration = duration;
            }
        }
        #endregion

        public FuelItem SelectedFuel { get; private set; }

        [SerializeField]
        private ItemPropertyReference m_FuelProperty;

        [SerializeField]
        private Button m_NextBtn;

        [SerializeField]
        private Button m_PreviousBtn;

        [SerializeField]
        private Image m_IconImg;

        private FuelItem[] m_FuelItems = new FuelItem[0];
        private int m_SelectedFuelIdx;

        private IInventory m_Inventory;


        public void AttachToInventory(IInventory inventory) 
        {
            m_Inventory = inventory;
            inventory.onContainerChanged += OnItemContainerChanged;
            OnItemContainerChanged(null);
        }

        public void DetachFromInventory() 
        {
            m_Inventory.onContainerChanged -= OnItemContainerChanged;
            m_Inventory = null;
        }

        private void Awake()
        {
            CacheFuelItems();
            m_SelectedFuelIdx = 0;

            m_NextBtn.onClick.AddListener(()=> SelectNextFuel(true));
            m_PreviousBtn.onClick.AddListener(()=> SelectNextFuel(false));

            SelectFuelAtIndex(0);
        }

        private void OnItemContainerChanged(ItemSlot slot)
        {
            RefreshFuelList();

            if (m_SelectedFuelIdx == -1 || m_FuelItems[m_SelectedFuelIdx].Count == 0)
                SelectNextFuel(true);
        }

        private void SelectNextFuel(bool selectNext)
        {
            RefreshFuelList();

            bool foundValidFuel = false;
            int iterations = 0;
            int i = m_SelectedFuelIdx;

            do
            {
                i = (int)Mathf.Repeat(i + (selectNext ? 1 : -1), m_FuelItems.Length);
                iterations++;

                if (m_FuelItems[i].Count > 0)
                {
                    foundValidFuel = true;
                    m_SelectedFuelIdx = i;
                }
            }
            while(!foundValidFuel && iterations < m_FuelItems.Length);

            m_SelectedFuelIdx = foundValidFuel ? i : -1;
            SelectFuelAtIndex(m_SelectedFuelIdx);
        }

        private void SelectFuelAtIndex(int index)
        {
            if (m_FuelItems == null || m_FuelItems.Length < 1)
                return;

            m_IconImg.enabled = index > -1;

            if (index > -1)
            {
                SelectedFuel = m_FuelItems[index];

                if (ItemDatabase.TryGetItemById(SelectedFuel.Id, out ItemInfo itemInfo))
                    m_IconImg.sprite = itemInfo.Icon;
            }
        }

        private void CacheFuelItems()
        {
            List<FuelItem> fuelItems = new List<FuelItem>();
            int i = 0;

            foreach (var itemCategory in ItemDatabase.GetAllCategories())
            {
                foreach (var itemInfo in itemCategory.Items)
                {
                    foreach (var property in itemInfo.Properties)
                    {
                        if (property.Id == m_FuelProperty)
                        {
                            fuelItems.Add(new FuelItem(itemInfo.Id, 0, property.GetAsInteger()));
                            i++;

                            break;
                        }
                    }
                }
            }

            m_FuelItems = fuelItems.ToArray();
        }

        private void RefreshFuelList()
        {
            for (int i = 0; i < m_FuelItems.Length; i++)
                m_FuelItems[i].Count = 0;

            for (int i = 0; i < PlayerInventory.Containers.Count; i++)
            {
                for (int j = 0; j < PlayerInventory.Containers[i].Slots.Length; j++)
                {
                    Item item = PlayerInventory.Containers[i].Slots[j].Item;

                    if (item != null && item.HasProperty(m_FuelProperty) && TryGetFuelItem(item.Id, out FuelItem fuelItem))
                        fuelItem.Count += item.CurrentStackSize;
                }
            }
        }

        private bool TryGetFuelItem(int itemId, out FuelItem fuelItem)
        {
            for (int i = 0;i < m_FuelItems.Length;i++)
            {
                if(m_FuelItems[i].Id == itemId)
                {
                    fuelItem = m_FuelItems[i];
                    return true;
                }
            }

            fuelItem = null;
            return false;
        }
    }
}