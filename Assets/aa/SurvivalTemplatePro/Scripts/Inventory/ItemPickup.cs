using SurvivalTemplatePro.InventorySystem;
using SurvivalTemplatePro.UISystem;
using UnityEngine;

namespace SurvivalTemplatePro
{
    /// <summary>
    /// Basic item pickup class.
    /// References items from the Item Database.
    /// </summary>
    public class ItemPickup : Interactable
	{
		public Item Item => m_ItemInstance;

		[BHeader("Settings (Item)")]

		[SerializeField]
		[Tooltip("Corresponding item id.")]
		private ItemReference m_Item;

		[SerializeField, Range(0, 99)]
		[Tooltip("How many items of the same type is this item going to contain.")]
		private int m_ItemCount = 1;

		[SerializeField]
		[Tooltip("The first type of container that this item will be added to (e.g. a weapon would be added to the holster at first).")]
		private ItemContainerFlags m_PrimaryFlags = ItemContainerFlags.Storage;

		[Space]

		[SerializeField]
		[Tooltip("Sound that will be played upon picking the item up.")]
		private SoundPlayer m_PickUpSound;

		private Item m_ItemInstance;
		private float m_NextTimeCanInteract;


        public override void OnInteract(ICharacter character)
		{
			if (Time.time < m_NextTimeCanInteract || m_ItemInstance == null)
				return;

			base.OnInteract(character);

			if (InteractionEnabled)
			{
				if (m_ItemInstance.CurrentStackSize == 1)
					PickUpSingleItem(character);
				else if (m_ItemInstance.CurrentStackSize > 1)
					PickUpItemStack(character);
			}

			m_NextTimeCanInteract = Time.time + 0.5f;
		}

		public void LinkWithItem(Item item)
		{
			m_ItemInstance = item;

			if (m_ItemInstance != null)
				m_Item = m_ItemInstance.Id;
		}

		private void Awake()
		{
			if (m_Item != ItemDatabase.NullItem.Id)
				m_ItemInstance = new Item(m_Item.GetItem(), m_ItemCount);		
		}

		private void PickUpSingleItem(ICharacter character)
		{
			if (m_ItemInstance != null)
			{
				bool added = character.Inventory.AddItem(m_ItemInstance, m_PrimaryFlags) > 0;

				if (!added)
					added = character.Inventory.AddItem(m_ItemInstance, ItemContainerFlags.Everything) > 0;

				if (added)
				{
					MessageDisplayerUI.PushMessage($"Picked up {m_ItemInstance.Name}");
					m_PickUpSound.Play2D(1f, SelectionType.Random);

					Destroy(gameObject);
				}
				else
				{
					MessageDisplayerUI.PushMessage("Inventory Full", Color.red);
				}
			}
			else
			{
				Debug.LogError("Item Instance is null, can't pick up anything.");
				return;
			}
		}

		private void PickUpItemStack(ICharacter character)
		{
			if (m_ItemInstance != null)
			{
				int originalCount = m_ItemInstance.CurrentStackSize;
				int addedCount = character.Inventory.AddItem(m_ItemInstance, m_PrimaryFlags);

				if (addedCount < originalCount)
					addedCount += character.Inventory.AddItem(m_ItemInstance, ItemContainerFlags.Everything);

				if (addedCount > 0)
				{
					MessageDisplayerUI.PushMessage($"Picked up {m_ItemInstance.Name} x {addedCount}");

					m_PickUpSound.Play2D(1f, SelectionType.Random);

					if (addedCount >= originalCount)
						Destroy(gameObject);
				}
				else
				{
					MessageDisplayerUI.PushMessage("Inventory Full", Color.red);
				}
			}
			else
			{
				Debug.LogError("Item Instance is null, can't pick up anything.");
				return;
			}
		}
	}
}