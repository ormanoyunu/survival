using System;
using UnityEngine;

namespace SurvivalTemplatePro.InventorySystem
{
    [Serializable]
    public class ItemCategory
    {
        public string Name => m_Name;
        public ItemInfo[] Items => m_Items;

        [SerializeField]
        private string m_Name;

        [SerializeField]
        private ItemInfo[] m_Items;
    }
}