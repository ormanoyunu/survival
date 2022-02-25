using System;
using UnityEngine;

namespace SurvivalTemplatePro.InventorySystem
{
    /// <summary>
    /// Information about an item, which should be set in the item database.
    /// </summary>
    [Serializable]
	public class ItemInfo
	{
		public int Id => m_Id;
		public string Name => m_Name;
		public Sprite Icon => m_Icon;
		public string Description => m_Description;
		public ItemPickup Pickup => m_Pickup;
		public int StackSize => m_StackSize;
		public CraftingSettings Crafting => m_Crafting;
        public float Weight => m_Weight;
		public ItemPropertyInfoList Properties => m_Properties;
		public string Tag => m_Tag;

		public string Category { get => m_Category; set => m_Category = value; }

		[SerializeField]
		[Tooltip("Item name.")]
		private string m_Name;

		[Space]

		[SerializeField, ReadOnly]
		[Tooltip("Unique id (auto generated).")]
		private int m_Id;

		[SerializeField, ReadOnly]
		[Tooltip("Category of this item.")]
		private string m_Category;

		[Space]

		[SerializeField, PreviewSprite]
		[Tooltip("Item Icon.")]
		private Sprite m_Icon;

		[SerializeField, MultilineCustom(5)]
		[Tooltip("Item description to display in the UI.")]
		private string m_Description;

		[SerializeField]
		[Tooltip("Corresponding pickup for this item, so you can actually drop it, or pick it up from the ground.")]
		private ItemPickup m_Pickup;

		[SerializeField, Range(1, 1000)]
		[Tooltip("How many items of this type can be stacked in a single slot.")]
		private int m_StackSize = 1;

        [SerializeField, Range(0.01f, 10f)]
		[Tooltip("")]
		private float m_Weight = 1f;

		[Space]

		[SerializeField]
		[Tooltip("String tag (similar to GameObject.Tag), it can have many use cases but the main one is to limit the item from being placed in certain item containers.")]
		private ItemTagReference m_Tag;

		[SerializeField, Reorderable]
		[Tooltip("all the item's properties, like Durability, Health Restore, Fuel and so on.")]
		private ItemPropertyInfoList m_Properties;

		[SerializeField]
		private CraftingSettings m_Crafting;


		public ItemInfo(int id, string name)
		{
			this.m_Id = id;
			this.m_Name = name;
		}

		public bool CompareTag(string tag)
		{
			if (tag == m_Tag)
				return true;

			return false;
		}
	}
}