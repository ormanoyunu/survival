using System.Collections;
using UnityEngine;

namespace SurvivalTemplatePro
{
    /// <summary>
    /// Deals with the Player death and respawn behaviour.
    /// </summary>
    public class PlayerDeathHandler : CharacterBehaviour
    {
        private enum ItemDropType { All, Equipped, None }

        [InfoBox("Items to drop on death.")]
        [SerializeField]
        private ItemDropType m_ItemDropType = ItemDropType.None;


        public override void OnInitialized()
        {
            Character.HealthManager.onDeath += OnPlayerDeath;
            Character.HealthManager.onRespawn += OnPlayerRespawn;
        }

        private void OnPlayerDeath()
        {
            // Pause the player
            if (TryGetModule(out IPauseHandler pauseHandler))
                pauseHandler.RegisterLocker(this, new PlayerPauseParams(true, true, true, true));

            // Disable the Character Controller
            CharacterController characterController = Character.GetComponent<CharacterController>();
            characterController.enabled = false;

            // Reset the Character Mover
            Character.Mover.ResetStateAndVelocity();

            // Stop inventory inspection
            if (TryGetModule(out IInventoryInspectManager inventoryInspectManager))
                inventoryInspectManager.TryStopInspecting();

            // Handle item dropping
            if (TryGetModule(out IItemDropHandler itemDropHandler))
                HandleItemDrop(itemDropHandler);

            // Do death module effects
            if (TryGetModule(out IDeathModule deathModule))
                deathModule.DoDeathEffects(Character);
        }

        private void OnPlayerRespawn() 
        {
            // Unpause the player
            if (TryGetModule(out IPauseHandler pauseHandler))
                pauseHandler.UnregisterLocker(this);

            // Do death module respawn effects
            if (TryGetModule(out IDeathModule deathModule))
                deathModule.DoRespawnEffects(Character);

            // Reset Thirst
            if (TryGetModule(out IThirstManager thirst))
                thirst.Thirst = thirst.MaxThirst;

            // Reset Energy
            if (TryGetModule(out IEnergyManager energy))
                energy.Energy = energy.MaxEnergy;

            // Reset Hunger
            if (TryGetModule(out IHungerManager hunger))
                hunger.Hunger = hunger.MaxHunger;

            StartCoroutine(C_EnableController());

            // Reset the Character Mover
            Character.Mover.ResetStateAndVelocity();
        }

        private IEnumerator C_EnableController() 
        {
            yield return new WaitForEndOfFrame();

            // Re-enable the Character Controller
            CharacterController characterController = Character.GetComponent<CharacterController>();
            characterController.enabled = true;
        }

        private void DropAllItems(IInventory inventory, IItemDropHandler dropHandler)
        {
            for (int i = 0; i < inventory.Containers.Count; i++)
            {
                for (int j = 0; j < inventory.Containers[i].Slots.Length; j++)
                {
                    var slot = inventory.Containers[i].Slots[j];

                    if (slot.HasItem)
                        dropHandler.DropItem(slot);
                }
            }
        }

        private void HandleItemDrop(IItemDropHandler dropHandler) 
        {
            switch (m_ItemDropType)
            {
                // Drop all inventory items
                case ItemDropType.All:
                    DropAllItems(Character.Inventory, dropHandler);
                    break;

                // Drop the Holstered wieldable
                case ItemDropType.Equipped:
                    if (m_ItemDropType == ItemDropType.Equipped)
                    {
                        if (TryGetModule(out IWieldablesController wieldablesController) && wieldablesController.ActiveWieldable != null)
                        {
                            var attachedItem = wieldablesController.ActiveWieldable.AttachedItem;

                            if (attachedItem != null)
                                dropHandler.DropItem(attachedItem, 0f);
                        }
                    }
                    break;

                // Don't drop anything
                case ItemDropType.None:
                    break;
            }
        }
    }
}
