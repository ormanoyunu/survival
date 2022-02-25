using System;
using UnityEngine;
using UnityEngine.Events;

namespace SurvivalTemplatePro.InventorySystem
{
    #region Internal
    public enum SlotChangeType
	{
		ItemChanged,
		StackChanged,
		PropertyChanged
	}
    #endregion

    [Serializable]
	public class ItemSlot
	{
		public bool HasItem => Item != null;
		public Item Item => m_Item;

		/// <summary>Sent when this slot has changed (e.g. when the item has changed).</summary>
		public event UnityAction<ItemSlot, SlotChangeType> onChanged;

		[SerializeField]
		private Item m_Item;


		#region Save&Load
		public void OnDeserialization(object sender)
		{
			if (Item != null)
			{
				Item.onPropertyChanged += OnPropertyChanged;
				Item.onStackChanged += OnStackChanged;
			}
		}
        #endregion

        public void SetItem(Item item)
		{
			if (m_Item != item)
			{
				if (Item != null)
				{
					Item.onPropertyChanged -= OnPropertyChanged;
					Item.onStackChanged -= OnStackChanged;
				}

				m_Item = item;

				if (m_Item != null)
				{
					m_Item.onPropertyChanged += OnPropertyChanged;
					m_Item.onStackChanged += OnStackChanged;
				}

				onChanged?.Invoke(this, SlotChangeType.ItemChanged);
			}
		}

		public int RemoveFromStack(int amount)
		{
			if (!HasItem)
				return 0;

			if (amount >= Item.CurrentStackSize)
			{
				int stackSize = Item.CurrentStackSize;
				SetItem(null);

				return stackSize;
			}

			int oldStack = Item.CurrentStackSize;
			Item.CurrentStackSize = Mathf.Max(Item.CurrentStackSize - amount, 0);

			if (oldStack != Item.CurrentStackSize)
				onChanged?.Invoke(this, SlotChangeType.StackChanged);

			return oldStack - Item.CurrentStackSize;
		}

		public int AddToStack(int amount)
		{
			if (!HasItem || Item.Info.StackSize <= 1)
				return 0;

			int oldStackCount = Item.CurrentStackSize;
			int surplus = amount + oldStackCount - Item.Info.StackSize;
			int currentStackCount = oldStackCount;

			if (surplus <= 0)
				currentStackCount += amount;
			else
				currentStackCount = Item.Info.StackSize;

			Item.CurrentStackSize = currentStackCount;

			return currentStackCount - oldStackCount;
		}

		private void OnPropertyChanged(ItemProperty itemProperty) => onChanged?.Invoke(this, SlotChangeType.PropertyChanged);
		private void OnStackChanged() => onChanged?.Invoke(this, SlotChangeType.StackChanged);

		public static implicit operator bool(ItemSlot slot) => slot != null;
	}
}
