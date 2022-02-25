using System;
using UnityEngine;

namespace SurvivalTemplatePro.InventorySystem
{
    [Serializable]
	public class ContainerGenerator
	{
		public string Name => m_Name;
		public int MaxWeight => m_MaxWeight;
		public int Size => m_Size;

		[SerializeField]
		private string m_Name;

		[SerializeField, Range(0, 100)]
		private int m_MaxWeight = 30;

		[SerializeField]
		private ItemContainerFlags m_Flag;

		[SerializeField, Range(1, 100)]
		private int m_Size = 1;

		[BHeader("Item Filtering")]

		[SerializeField]
		private ItemCategoryReference[] m_ValidCategories;

		[SerializeField]
		private ItemPropertyReference[] m_RequiredProperties;

		[SerializeField]
		private ItemTagReference m_RequiredTag;


		public ItemContainer GenerateContainer()
		{
			var container = new ItemContainer(
				m_Name,
				m_MaxWeight,
				m_Size,
				m_Flag,
				m_ValidCategories,
				m_RequiredProperties,
				m_RequiredTag);

			return container;
		}
	}
}