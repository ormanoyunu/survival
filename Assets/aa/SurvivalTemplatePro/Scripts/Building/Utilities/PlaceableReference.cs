using SurvivalTemplatePro.BuildingSystem;
using System;
using UnityEngine;

namespace SurvivalTemplatePro
{
    [Serializable]
    public struct PlaceableReference
	{
		public int Value => m_Value;

		[SerializeField]
		private int m_Value;


		public PlaceableReference(int value)
		{
			this.m_Value = value;
		}

		public static bool operator ==(PlaceableReference x, PlaceableReference y) => x.m_Value == y.m_Value;
		public static bool operator ==(PlaceableReference x, int y) => x.m_Value == y;
		public static bool operator !=(PlaceableReference x, PlaceableReference y) => x.m_Value != y.m_Value;
		public static bool operator !=(PlaceableReference x, int y) => x.m_Value != y;

		public override bool Equals(object obj)
		{
			if (obj is PlaceableReference)
				return m_Value == ((PlaceableReference)obj).m_Value;
			else if (obj is int)
				return m_Value == (int)obj;

			return false;
		}

		public static implicit operator PlaceableReference(int value) => new PlaceableReference(value);
		public static implicit operator int(PlaceableReference item) => item.Value;

		public Placeable GetPlaceable() => PlaceableDatabase.GetPlaceableById(m_Value);

		public override string ToString() => PlaceableDatabase.GetPlaceableById(m_Value).PlaceableName;
		public override int GetHashCode() => m_Value.GetHashCode();
	}
}