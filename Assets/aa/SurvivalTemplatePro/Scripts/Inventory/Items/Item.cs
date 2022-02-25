using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SurvivalTemplatePro.InventorySystem
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
	public class Item
	{
		public ItemInfo Info => ItemDatabase.GetItemById(m_Id);
		public ItemProperty[] Properties => m_Properties;

		public int Id => m_Id;
		public string Name => m_Name;

		public int CurrentStackSize
		{
			get => m_CurrentStackSize;
			set
			{
				int oldStackSize = m_CurrentStackSize;
				m_CurrentStackSize = value;

				if (m_CurrentStackSize != oldStackSize)
					onStackChanged?.Invoke();
			}
		}

		public event UnityAction<ItemProperty> onPropertyChanged;
		public event UnityAction onStackChanged;

		[SerializeField]
		private int m_Id;

		[SerializeField]
		private string m_Name;

		[SerializeField]
		private int m_CurrentStackSize;

		[SerializeField]
		private ItemProperty[] m_Properties;


		public static implicit operator bool(Item item) => item != null;

		/// <summary>
		/// 
		/// </summary>
		public Item(ItemInfo itemInfo, int count = 1, ItemProperty[] customProperties = null)
		{
			m_Id = itemInfo.Id;
			m_Name = itemInfo.Name;
		
			CurrentStackSize = Mathf.Clamp(count, 1, itemInfo.StackSize);

			if (customProperties != null)
				m_Properties = CloneProperties(customProperties);
			else
				m_Properties = InstantiateProperties(itemInfo.Properties);

			for(int i = 0;i < m_Properties.Length;i++)
				m_Properties[i].onChanged += OnPropertyChanged;
		}

		public void OnDeserialization(object sender)
		{
			if (ItemDatabase.AssetExists)
			{
				if (ItemDatabase.TryGetItemById(m_Id, out ItemInfo itemInfo))
				{
					for (int i = 0; i < m_Properties.Length; i++)
						m_Properties[i].onChanged += OnPropertyChanged;
				}
				else
					Debug.LogErrorFormat("[SavableItem] - This item couldn't be initialized and will not function properly. No item with the name {0} was found in the database!", Name);
			}
			else
				Debug.LogError("[SavableItem] - This item couldn't be initialized and will not function properly. No item database found!");
		}

		public bool HasProperty(int id)
		{
			for (int i = 0;i < m_Properties.Length;i++)
			{
				if (m_Properties[i].PropertyId == id)
					return true;
			}

			return false;
		}

		public bool HasProperty(string name)
		{
			for (int i = 0;i < m_Properties.Length;i++)
			{
				if (m_Properties[i].PropertyName == name)
					return true;
			}

			return false;
		}

		/// <summary>
		/// Use this if you are sure the item has this property.
		/// </summary>
		public ItemProperty GetProperty(int id)
		{
			ItemProperty itemProperty = null;

			for (int i = 0;i < m_Properties.Length;i++)
			{
				if (m_Properties[i].PropertyId == id)
				{
					itemProperty = m_Properties[i];
					break;
				}
			}

			return itemProperty;
		}

		/// <summary>
		/// Use this if you are sure the item has this property.
		/// </summary>
		public ItemProperty GetProperty(string name)
		{
			ItemProperty itemProperty = null;

			for (int i = 0;i < m_Properties.Length;i++)
			{
				if (m_Properties[i].PropertyName == name)
				{
					itemProperty = m_Properties[i];
					break;
				}
			}

			return itemProperty;
		}

		/// <summary>
		/// Use this if you are NOT sure the item has this property.
		/// </summary>
		public bool TryGetProperty(int id, out ItemProperty itemProperty)
		{
			itemProperty = null;

			for(int i = 0;i < m_Properties.Length;i++)
			{
				if(m_Properties[i].PropertyId == id)
				{
					itemProperty = m_Properties[i];
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Use this if you are NOT sure the item has this property.
		/// </summary>
		public bool TryGetProperty(string name, out ItemProperty itemProperty)
		{
			itemProperty = null;

			for(int i = 0;i < m_Properties.Length;i++)
			{
				if(m_Properties[i].PropertyName == name)
				{
					itemProperty = m_Properties[i];
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Use this if you are sure the item has this property.
		/// </summary>
		public ItemProperty[] GetAllPropertiesWithId(int id)
		{
			List<ItemProperty> itemProperties = new List<ItemProperty>();

			for(int i = 0;i < m_Properties.Length;i++)
			{
				if (m_Properties[i].PropertyId == id)
					itemProperties.Add(m_Properties[i]);
			}

			return itemProperties.ToArray();
		}

		/// <summary>
		/// Use this if you are sure the item has this property.
		/// </summary>
		public ItemProperty[] GetAllPropertiesWithName(string name)
		{
			List<ItemProperty> itemProperties = new List<ItemProperty>();

			for (int i = 0; i < m_Properties.Length; i++)
			{
				if (m_Properties[i].PropertyName == name)
					itemProperties.Add(m_Properties[i]);
			}

			return itemProperties.ToArray();
		}

		public override string ToString() => "Item Name: " + m_Name + " | Stack Size: " + m_CurrentStackSize;

		private ItemProperty[] CloneProperties(ItemProperty[] properties)
		{
			ItemProperty[] clonedProperties = new ItemProperty[properties.Length];

			for(int i = 0;i < properties.Length;i++)
				clonedProperties[i] = properties[i].GetMemberwiseClone();

			return clonedProperties;
		}

		private ItemProperty[] InstantiateProperties(ItemPropertyInfoList propertyInfos)
		{
			ItemProperty[] properties = new ItemProperty[propertyInfos.Length];

			for (int i = 0;i < propertyInfos.Length;i++)
				properties[i] = new ItemProperty(propertyInfos[i]);

			return properties;
		}

		private void OnPropertyChanged(ItemProperty property) => onPropertyChanged?.Invoke(property);
	}
}