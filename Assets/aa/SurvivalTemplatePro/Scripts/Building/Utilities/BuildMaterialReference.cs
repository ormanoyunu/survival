using SurvivalTemplatePro.BuildingSystem;
using UnityEngine;
using System;

namespace SurvivalTemplatePro
{
	[Serializable]
	public struct BuildMaterialReference
    {
		public int Value => m_Value;

		[SerializeField]
		private int m_Value;


		public BuildMaterialReference(int value)
		{
			this.m_Value = value;
		}

		public static bool operator ==(BuildMaterialReference x, BuildMaterialReference y) => x.m_Value == y.m_Value;
		public static bool operator ==(BuildMaterialReference x, int y) => x.m_Value == y;
		public static bool operator !=(BuildMaterialReference x, BuildMaterialReference y) => x.m_Value != y.m_Value;
		public static bool operator !=(BuildMaterialReference x, int y) => x.m_Value != y;

		public override bool Equals(object obj)
		{
			if (obj is BuildMaterialReference)
				return m_Value == ((BuildMaterialReference)obj).m_Value;
			else if (obj is int)
				return m_Value == (int)obj;

			return false;
		}

		public static implicit operator BuildMaterialReference(int value) => new BuildMaterialReference(value);
		public static implicit operator BuildingMaterialInfo(BuildMaterialReference reference) => BuildMaterialsDatabase.GetBuildingMaterialById(reference.m_Value);
		public static implicit operator int(BuildMaterialReference item) => item.Value;

		public override string ToString() => BuildMaterialsDatabase.GetBuildingMaterialById(m_Value).Name;
		public override int GetHashCode() => m_Value.GetHashCode();
	}
}