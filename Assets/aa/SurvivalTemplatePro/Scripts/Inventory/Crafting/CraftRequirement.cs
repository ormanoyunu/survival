using System;
using UnityEngine;

namespace SurvivalTemplatePro.InventorySystem
{
    [Serializable]
    public struct CraftRequirement
    {
        public ItemReference Item;

        [Range(1, 20)]
        public int Amount;


        public CraftRequirement(int itemId, int amount)
        {
            this.Item = new ItemReference(itemId);
            this.Amount = amount;
        }
    }
}
