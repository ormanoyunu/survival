using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SurvivalTemplatePro.InventorySystem
{
    [DisallowMultipleComponent()]
    public class Inventory : CharacterBehaviour, IInventory
    {
        public List<ItemContainer> Containers
        {
            get
            {
                if (m_Containers == null)
                {
                    m_Containers = new List<ItemContainer>();

                    for (int i = 0; i < m_InitialContainers.Length; i++)
                    {
                        ItemContainer container = m_InitialContainers[i].GenerateContainer();
                        m_Containers.Add(container);

                        AddListeners(container, true);
                    }
                }

                return m_Containers;
            }
        }

        public ContainerGeneratorList StartupContainers => m_InitialContainers;

        public event UnityAction<ItemSlot> onContainerChanged;
        public event UnityAction<ItemSlot, SlotChangeType> onSlotChanged;

        [SerializeField, Reorderable]
        [Tooltip("The initial item containers")]
        private ContainerGeneratorList m_InitialContainers = new ContainerGeneratorList();

        private List<ItemContainer> m_Containers;


        #region Save&Load
        public void OnLoad()
        {
            foreach (ItemContainer container in Containers)
            {
                container.OnDeserialization(this);

                foreach (ItemSlot slot in container.Slots)
                {
                    slot.OnDeserialization(this);

                    if (slot.HasItem)
                        slot.Item.OnDeserialization(this);
                }
            }
        }
        #endregion

        public void AddContainer(ItemContainer itemContainer, bool add)
        {
            if (add && !Containers.Contains(itemContainer))
            {
                Containers.Add(itemContainer);
                AddListeners(itemContainer, true);
            }
            else if (!add && Containers.Contains(itemContainer))
            {
                Containers.Remove(itemContainer);
                AddListeners(itemContainer, false);
            }
        }

        public bool HasContainerWithFlags(ItemContainerFlags flags)
        {
            for (int i = 0; i < Containers.Count; i++)
            {
                if (flags.Has(Containers[i].Flag))
                    return true;
            }

            return false;
        }

        public ItemContainer GetContainerWithFlags(ItemContainerFlags flags)
        {
            for (int i = 0; i < Containers.Count; i++)
            {
                if (flags.Has(Containers[i].Flag))
                    return Containers[i];
            }

            return null;
        }

        public ItemContainer GetContainerWithName(string name)
        {
            for (int i = 0; i < Containers.Count; i++)
            {
                if (Containers[i].Name == name)
                    return Containers[i];
            }

            return null;
        }

        public int AddItem(Item item, ItemContainerFlags flags = ItemContainerFlags.Everything)
        {
            int addedInTotal = 0;

            for (int i = 0; i < Containers.Count; i++)
            {
                if (flags.Has(Containers[i].Flag))
                {
                    addedInTotal += Containers[i].AddItem(item);

                    if (addedInTotal >= item.CurrentStackSize)
                        break;
                }
            }

            return addedInTotal;
        }

        public bool AddOrSwap(ItemContainer slotParent, ItemSlot slot, ItemContainerFlags flags)
        {
            for (int i = 0; i < Containers.Count; i++)
            {
                if (flags.Has(Containers[i].Flag))
                {
                    bool addedOrSwapped = Containers[i].AddOrSwap(slotParent, slot);

                    if (addedOrSwapped)
                        return true;
                }
            }

            return false;
        }

        public int AddItems(string itemName, int amountToAdd, ItemContainerFlags flags)
        {
            int addedInTotal = 0;

            if (amountToAdd > 0)
            {
                for (int i = 0; i < Containers.Count; i++)
                {
                    if (flags.Has(Containers[i].Flag))
                    {
                        int added = Containers[i].AddItem(itemName, amountToAdd);
                        addedInTotal += added;

                        if (added == addedInTotal)
                            return addedInTotal;
                    }
                }
            }

            return addedInTotal;
        }

        public int AddItems(int itemId, int amountToAdd, ItemContainerFlags flags)
        {
            int addedInTotal = 0;

            if (amountToAdd > 0)
            {
                for (int i = 0; i < Containers.Count; i++)
                {
                    if (flags.Has(Containers[i].Flag))
                    {
                        int added = Containers[i].AddItem(itemId, amountToAdd);
                        addedInTotal += added;

                        if (added == addedInTotal)
                            return addedInTotal;
                    }
                }
            }

            return addedInTotal;
        }

        public bool RemoveItem(Item item, ItemContainerFlags flags)
        {
            for (int i = 0; i < Containers.Count; i++)
            {
                if (flags.Has(Containers[i].Flag))
                {
                    if (Containers[i].RemoveItem(item))
                        return true;
                }
            }

            return false;
        }

        public int RemoveItems(string itemName, int amountToRemove, ItemContainerFlags flags)
        {
            int removedInTotal = 0;

            if (amountToRemove > 0)
            {
                for (int i = 0; i < Containers.Count; i++)
                {
                    if (flags.Has(Containers[i].Flag))
                    {
                        int removedNow = Containers[i].RemoveItem(itemName, amountToRemove);
                        removedInTotal += removedNow;

                        if (removedNow == removedInTotal)
                            return removedInTotal;
                    }
                }
            }

            return removedInTotal;
        }

        public int RemoveItems(int itemId, int amountToRemove, ItemContainerFlags flags)
        {
            int removedInTotal = 0;

            if (amountToRemove > 0)
            {
                for (int i = 0; i < Containers.Count; i++)
                {
                    if (flags.Has(Containers[i].Flag))
                    {
                        int removedNow = Containers[i].RemoveItem(itemId, amountToRemove);
                        removedInTotal += removedNow;

                        if (removedNow == removedInTotal)
                            return removedInTotal;
                    }
                }
            }

            return removedInTotal;
        }

        public int GetItemCount(string itemName, ItemContainerFlags flags)
        {
            int count = 0;

            for (int i = 0; i < Containers.Count; i++)
            {
                if (flags.Has(Containers[i].Flag))
                    count += Containers[i].GetItemCount(itemName);
            }

            return count;
        }

        public int GetItemCount(int itemId, ItemContainerFlags flags)
        {
            int count = 0;

            for (int i = 0; i < Containers.Count; i++)
            {
                if (flags.Has(Containers[i].Flag))
                    count += Containers[i].GetItemCount(itemId);
            }

            return count;
        }

        private void AddListeners(ItemContainer itemContainer, bool addListener)
        {
            if (addListener)
                itemContainer.onChanged += OnContainerChanged;
            else
                itemContainer.onChanged -= OnContainerChanged;

            for (int i = 0; i < itemContainer.Slots.Length; i++)
            {
                if (addListener)
                    itemContainer.Slots[i].onChanged += OnSlotChanged;
                else
                    itemContainer.Slots[i].onChanged -= OnSlotChanged;
            }
        }

        private void OnContainerChanged(ItemSlot slot) => onContainerChanged?.Invoke(slot);
        private void OnSlotChanged(ItemSlot slot, SlotChangeType changeType) => onSlotChanged?.Invoke(slot, changeType);

        public bool ContainsItem(Item item, ItemContainerFlags flags)
        {
            for (int i = 0; i < Containers.Count; i++)
            {
                if (flags.Has(Containers[i].Flag) && Containers[i].ContainsItem(item))
                    return true;
            }

            return false;
        }
    }
}