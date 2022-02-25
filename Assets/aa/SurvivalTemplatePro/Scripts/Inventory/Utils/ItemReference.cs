using SurvivalTemplatePro.InventorySystem;
using System;
using UnityEngine;

namespace SurvivalTemplatePro
{
    [Serializable]
    public struct ItemReference
    {
		public int Value => m_Value;

		[SerializeField]
		private int m_Value;


		public ItemReference(int value)
		{
			this.m_Value = value;
		}

		public static bool operator ==(ItemReference x, ItemReference y) => x.m_Value == y.m_Value;
		public static bool operator ==(ItemReference x, int y) => x.m_Value == y;
		public static bool operator !=(ItemReference x, ItemReference y) => x.m_Value != y.m_Value;
		public static bool operator !=(ItemReference x, int y) => x.m_Value != y;

		public override bool Equals(object obj)
		{
			if (obj is ItemReference)
				return m_Value == ((ItemReference)obj).m_Value;
			else if (obj is int)
				return m_Value == (int)obj;

			return false;
		}

		public static explicit operator ItemReference(string itemName) => new ItemReference(ItemDatabase.GetItemByName(itemName).Id);

		public static implicit operator ItemReference(int value) => new ItemReference(value);
		public static implicit operator int(ItemReference reference) => reference.Value;

		public ItemInfo GetItem() => ItemDatabase.GetItemById(m_Value);

		public override string ToString() => ItemDatabase.GetItemById(m_Value).Name;
		public override int GetHashCode() => m_Value.GetHashCode();
	}
}