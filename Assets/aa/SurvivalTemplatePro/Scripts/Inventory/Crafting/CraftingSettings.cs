using System;
using UnityEngine;

namespace SurvivalTemplatePro.InventorySystem
{
    [Serializable]
    public class CraftingSettings
    {
        public CraftRequirementList Blueprint => m_Blueprint;
        public bool IsCraftable => m_HasBlueprint && m_IsCraftable;
        public float CraftDuration => m_CraftDuration;
        public bool AllowDismantle => m_HasBlueprint && m_AllowDismantle;
        public float DismantleEfficiency => m_DismantleEfficiency;

        [SerializeField]
        [Tooltip("Does this item have a blueprint? (Required for crafting and dismantling)")]
        private bool m_HasBlueprint;

        [SerializeField]
        [Tooltip("If enabled this item will show up in the survival book as a craftable item.")]
        private bool m_IsCraftable;

        [SerializeField, Range(0f, 300f)]
        [Tooltip("How much time does it take to craft this item, in seconds.")]
        private float m_CraftDuration;

        [SerializeField, Reorderable]
        [Tooltip("A list with all the 'ingredients' necessary to craft this item, it's also used in dismantling.")]
        private CraftRequirementList m_Blueprint;

        [SerializeField]
        [Tooltip("Can this item be dismantled?")]
        private bool m_AllowDismantle;

        [SerializeField, Range(0, 1f)]
        [Tooltip("An efficiency of 1 will result in getting all of the item back after dismantling, while 0 means that no item from the blueprint will be made available.")]
        private float m_DismantleEfficiency;
    }
}