using SurvivalTemplatePro.InventorySystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SurvivalTemplatePro.WieldableSystem
{
    /// <summary>
    /// Takes care of selecting wieldables based on inventory items.
    /// </summary>
    [RequireComponent(typeof(IWieldablesController))]
    public class WieldableInventorySelectionHandler : CharacterBehaviour, IWieldableSelectionHandler
    {
        #region Internal
        [System.Serializable]
        private struct WieldableItemPair
        {
            [Tooltip("Corresponding Database Item")]
            public ItemReference Item;

            public Wieldable Wieldable;
        }
        #endregion

        public int SelectedIndex => m_SelectedIndex;
        public int PreviousIndex => m_PrevIndex;

        public event UnityAction<int> onSelectedChanged;

        [InfoBox("Found in the Inventory Module.")]
        [SerializeField]
        [Tooltip("The corresponding inventory container (e.g. holster, backpack etc.) that this behaviour will use for selecting items.")]
        private string m_HolsterContainer = "Holster";

        [SerializeField]
        [Tooltip("Should this behaviour select the 'starting slot item' on start")]
        private bool m_SelectOnStart;

        [SerializeField, Range(1, 8)]
        [Tooltip("The fist slot that will be selected.")]
        private int m_StartingSlot;

        [Space]

        [SerializeField]
        [Tooltip("All of the Item Id - Wieldable pairs.")]
        private WieldableItemPair[] m_InventoryBasedWieldables;

        private int m_SelectedIndex = -1;
        private int m_PrevIndex = -1;
        private ItemContainer m_Holster;

        private readonly Dictionary<int, IWieldable> m_Wieldables = new Dictionary<int, IWieldable>();

        private IWieldablesController m_WieldableController;
        private IInventory m_Inventory;


        public override void OnInitialized()
        {
            GetModule(out m_WieldableController);
            GetModule(out m_Inventory);

            SpawnWieldables();

            StartCoroutine(C_SelectItemAtStartDelayed());
        }

        private void OnHolsterChanged(ItemSlot slot)
        {
            // get index of slot
            int indexOfSlot = m_Holster.GetSlotIndex(slot);

            if (m_SelectedIndex == indexOfSlot)
                SelectAtIndex(indexOfSlot, 1.5f);
            else if (slot.HasItem)
                SelectAtIndex(indexOfSlot, 1f);
        }

        public void Refresh() => SelectAtIndex(m_SelectedIndex, 1f);

        public void SelectAtIndex(int indexToSelect, float holsterSpeed = 1f)
        {
            m_SelectedIndex = Mathf.Clamp(indexToSelect, 0, m_Holster.Count - 1);

            EquipWieldable(m_Holster[m_SelectedIndex].Item, holsterSpeed);

            if (m_SelectedIndex != m_PrevIndex)
                onSelectedChanged?.Invoke(m_SelectedIndex);

            m_PrevIndex = m_SelectedIndex;
        }

        private void EquipWieldable(Item itemToAttach, float holsterSpeed)
        {
            if (itemToAttach != null && m_Wieldables.TryGetValue(itemToAttach.Id, out IWieldable wieldable))
                m_WieldableController.TryEquipWieldable(wieldable, itemToAttach, holsterSpeed);
            else
                m_WieldableController.TryEquipWieldable(null, null, holsterSpeed);
        }

        private IEnumerator C_SelectItemAtStartDelayed()
        {
            yield return null;
            yield return null;

            m_Holster = m_Inventory.GetContainerWithName(m_HolsterContainer);
            m_Holster.onChanged += OnHolsterChanged;

            if (m_SelectOnStart)
            {
                int startingSlot = Mathf.Clamp(m_StartingSlot - 1, 0, m_Holster.Count);
                SelectAtIndex(startingSlot);
            }
            else
            {
                m_SelectedIndex = 0;
            }
        }

        private void SpawnWieldables()
        {
            for (int i = 0; i < m_InventoryBasedWieldables.Length; i++)
            {
                if (m_InventoryBasedWieldables[i].Wieldable != null)
                {
                    int itemId = m_InventoryBasedWieldables[i].Item.GetItem().Id;
                    m_Wieldables.Add(itemId, m_WieldableController.SpawnWieldable(m_InventoryBasedWieldables[i].Wieldable));
                }
            }
        }
    }
}