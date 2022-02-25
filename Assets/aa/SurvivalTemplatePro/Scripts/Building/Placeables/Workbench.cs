using SurvivalTemplatePro.InventorySystem;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SurvivalTemplatePro.BuildingSystem
{
    public class Workbench : Interactable, IExternalContainer
    {
        public ItemContainer ItemContainer { get; private set; }
        public List<CraftRequirement> RepairRequirementsForCurrentItem { get; private set; }

        public Item ItemToRepair => ItemContainer.Slots[0].Item;
        public float RepairDuration => m_RepairDuration;

        public event UnityAction onUpdated;

        [BHeader("Settings (Workbench)")]

        [SerializeField, Range(0f, 25f)]
        [Tooltip("The time it takes to repair an item at this workbench.")]
        private float m_RepairDuration = 1f;

        [SerializeField]
        [Tooltip("The id of the durability property. After repairing an item the workbench will increase the value of that property for the repaired item.")]
        private ItemPropertyReference m_DurabilityProperty;

        [Space]

        [SerializeField]
        [Tooltip("Repair sound to be played after successfully repairing an item.")]
        private SoundPlayer m_RepairAudio;


        public bool CanRepairItem()
        {
            if (ItemToRepair == null)
                return false;

            Item itemToRepair = ItemContainer.Slots[0].Item;
            bool canRepairItem = itemToRepair != null && itemToRepair.HasProperty(m_DurabilityProperty) && itemToRepair.GetProperty(m_DurabilityProperty).Float < 100f;

            return canRepairItem;
        }

        public void RepairItem(ICharacter character)
        {
            foreach (var req in RepairRequirementsForCurrentItem)
                character.Inventory.RemoveItems(req.Item, req.Amount, ItemContainerFlags.Storage);

            ItemToRepair.GetProperty(m_DurabilityProperty).Float = 100f;

            m_RepairAudio.PlayAtPosition(transform.position, 1f);
        }

        private void Awake()
        {
            ItemContainer = new ItemContainer("Workspace", 100, 1, ItemContainerFlags.External, null, null, null);

            RepairRequirementsForCurrentItem = new List<CraftRequirement>();
            ItemContainer.onChanged += OnItemToRepairChanged;
        }

        private void OnItemToRepairChanged(ItemSlot itemSlot)
        {
            RepairRequirementsForCurrentItem.Clear();

            var item = itemSlot.Item;

            if (item != null)
            {
                if (item.TryGetProperty(m_DurabilityProperty, out ItemProperty durabilityProperty))
                {
                    float durability = durabilityProperty.Float;

                    foreach (var req in item.Info.Crafting.Blueprint)
                    {
                        int requiredAmount = Mathf.Max(Mathf.RoundToInt(req.Amount * Mathf.Clamp01((100f - durability) / 100f)), 1);
                        RepairRequirementsForCurrentItem.Add(new CraftRequirement(req.Item, requiredAmount));
                    }
                }
            }

            onUpdated?.Invoke();
        }
    }
}