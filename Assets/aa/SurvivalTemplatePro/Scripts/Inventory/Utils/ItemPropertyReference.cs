using SurvivalTemplatePro.InventorySystem;
using System;
using UnityEngine;

namespace SurvivalTemplatePro
{
    [Serializable]
	public struct ItemPropertyReference
	{
		public int Value => m_Value;

		[SerializeField]
		private int m_Value;


		public ItemPropertyReference(int value)
		{
			this.m_Value = value;
		}

		public ItemPropertyReference(string name)
		{
			this.m_Value = ItemDatabase.GetPropertyByName(name).Id;
		}

		public static bool operator ==(ItemPropertyReference x, ItemPropertyReference y) => x.m_Value == y.m_Value;
		public static bool operator ==(ItemPropertyReference x, int y) => x.m_Value == y;
		public static bool operator !=(ItemPropertyReference x, ItemPropertyReference y) => x.m_Value != y.m_Value;
		public static bool operator !=(ItemPropertyReference x, int y) => x.m_Value != y;

		public override bool Equals(object obj)
		{
			if (obj is ItemPropertyReference)
				return m_Value == ((ItemPropertyReference)obj).m_Value;
			else if (obj is int)
				return m_Value == (int)obj;

			return false;
		}

		public static implicit operator ItemPropertyReference(int value) => new ItemPropertyReference(value);
		public static implicit operator int(ItemPropertyReference reference) => reference.Value;

		public ItemPropertyDefinition GetProperty() => ItemDatabase.GetPropertyById(m_Value);

		public override string ToString() => ItemDatabase.GetItemById(m_Value).Name;
		public override int GetHashCode() => m_Value.GetHashCode();
	}
}