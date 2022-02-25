using SurvivalTemplatePro.InventorySystem;
using System;
using UnityEngine;

namespace SurvivalTemplatePro
{
    [Serializable]
    public struct ItemCategoryReference
    {
		public string Value => m_Value;

		[SerializeField]
		private string m_Value;


		public ItemCategoryReference(string value)
		{
			this.m_Value = value;
		}

		public static bool operator ==(ItemCategoryReference x, ItemCategoryReference y) => x.m_Value == y.m_Value;
		public static bool operator ==(ItemCategoryReference x, string y) => x.m_Value == y;
		public static bool operator !=(ItemCategoryReference x, ItemCategoryReference y) => x.m_Value != y.m_Value;
		public static bool operator !=(ItemCategoryReference x, string y) => x.m_Value != y;

		public override bool Equals(object obj)
		{
			if (obj is ItemCategoryReference)
				return m_Value == ((ItemCategoryReference)obj).m_Value;
			else if (obj is string)
				return m_Value == (string)obj;

			return false;
		}

		public static implicit operator ItemCategoryReference(string value) => new ItemCategoryReference(value);
		public static implicit operator string(ItemCategoryReference reference) => reference.Value;

		public ItemCategory GetItemCategory() => ItemDatabase.GetCategoryByName(m_Value);

		public override string ToString() => m_Value;
		public override int GetHashCode() => m_Value.GetHashCode();
	}
}
