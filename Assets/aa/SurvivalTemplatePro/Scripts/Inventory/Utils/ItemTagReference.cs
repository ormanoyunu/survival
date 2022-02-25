using System;
using UnityEngine;

namespace SurvivalTemplatePro
{
    [Serializable]
	public struct ItemTagReference
    {
		public string Value => m_Value;

		[SerializeField]
		private string m_Value;


		public ItemTagReference(string value)
		{
			this.m_Value = value;
		}

		public static bool operator ==(ItemTagReference x, ItemTagReference y) => x.m_Value == y.m_Value;
		public static bool operator ==(ItemTagReference x, string y) => x.m_Value == y;
		public static bool operator !=(ItemTagReference x, ItemTagReference y) => x.m_Value != y.m_Value;
		public static bool operator !=(ItemTagReference x, string y) => x.m_Value != y;

		public override bool Equals(object obj)
		{
			if (obj is ItemTagReference)
				return m_Value == ((ItemTagReference)obj).m_Value;
			else if (obj is string)
				return m_Value == (string)obj;

			return false;
		}

		public static implicit operator ItemTagReference(string value) => new ItemTagReference(value);
		public static implicit operator string(ItemTagReference reference) => reference.Value;

		public override string ToString() => m_Value;
		public override int GetHashCode() => m_Value.GetHashCode();
	}
}