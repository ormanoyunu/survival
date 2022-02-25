using System;
using UnityEngine;

namespace SurvivalTemplatePro.InventorySystem
{
    [Serializable]
    public class ItemPropertyDefinition
    {
        [HideInInspector]
        public int Id;

        public string Name;

        public ItemPropertyType Type;
    }
}