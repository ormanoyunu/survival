using System;
using UnityEngine;

namespace SurvivalTemplatePro.InventorySystem
{
    [Serializable]
    public class ItemGenerator
    {
        public ItemGenerationMethod Method;

        public ItemReference SpecificItem;

        [Range(1, 30)]
        public int Count = 1;

        public ItemCategoryReference Category;
    }

    public enum ItemGenerationMethod { Specific, Random, RandomFromCategory }
}