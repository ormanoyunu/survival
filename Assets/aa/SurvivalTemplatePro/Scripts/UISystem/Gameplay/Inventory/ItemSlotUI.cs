using SurvivalTemplatePro.InventorySystem;
using UnityEngine;
using UnityEngine.UI;

namespace SurvivalTemplatePro.UISystem
{
    public class ItemSlotUI : SlotUI
	{
		public ItemSlot ItemSlot
		{
			get
			{
				if(m_ItemSlot != null)
					return m_ItemSlot;
				else
				{
					Debug.LogError("No item slot is linked to this interface.");

					return null;
				}
			}
		}

		public bool HasItem { get { return m_ItemSlot == null ? false : m_ItemSlot.HasItem; } }
		public Item Item { get { return m_ItemSlot == null ? null : m_ItemSlot.Item; } }

		public ItemContainerUI Parent { get; private set; }

		[BHeader("Item Slot")]

		[SerializeField]
		private Image m_Icon;

		[SerializeField]
		private Text m_Stack;

		[SerializeField]
		private Color m_NormalStackColor = Color.grey;

		[SerializeField]
		private Color m_HighlightStackColor = Color.black;

		[Space]

		[SerializeField]
		private FillBarUI m_DurabilityBar;

		[SerializeField]
		private ItemPropertyReference m_DurabilityProperty;

		private ItemSlot m_ItemSlot;


		public void LinkToSlot(ItemSlot itemSlot)
		{
			if (m_ItemSlot != null)
				m_ItemSlot.onChanged -= OnSlotChanged;

			m_ItemSlot = itemSlot;
			m_ItemSlot.onChanged += OnSlotChanged;

			DoRefresh();
		}

		public void UnlinkFromSlot()
		{
			if (m_ItemSlot == null)
				return;

			m_ItemSlot.onChanged -= OnSlotChanged;
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();

			if (m_ItemSlot != null)
				m_ItemSlot.onChanged -= OnSlotChanged;
		}

		public void DoRefresh()
		{
			m_Icon.enabled = HasItem;

            if (m_Stack != null)
			    m_Stack.enabled = HasItem && Item.CurrentStackSize > 1;

			m_DurabilityBar.SetActive(HasItem && Item.HasProperty(m_DurabilityProperty));

			if (m_Icon.enabled)
				m_Icon.sprite = Item.Info.Icon;
			
			if (m_Stack != null && m_Stack.enabled)
				m_Stack.text = "x" + Item.CurrentStackSize.ToString();

            if (m_DurabilityBar.Active)
            	m_DurabilityBar.SetFillAmount(Item.GetProperty(m_DurabilityProperty).Float / 100f);
		}
			
		// <summary>
		/// Will return a clone of this slot, without the background.
		/// </summary>
		public RectTransform GetItemUI(Item item, float alpha)
		{
			ItemSlotUI itemUI = Instantiate(this);

			// Disable the slot UI
			itemUI.enabled = false;
			itemUI._Graphic.enabled = false;

			// Set up the icon
			itemUI.m_Icon.enabled = true;
			itemUI.m_Icon.sprite = item.Info.Icon;

            // Set up the stack text
            if (m_Stack != null)
            {
                itemUI.m_Stack.enabled = item.CurrentStackSize > 1;
                itemUI.m_Stack.text = string.Format("x{0}", item.CurrentStackSize);
            }

			// Set up the durability bar
			itemUI.m_DurabilityBar.SetActive(item.HasProperty(m_DurabilityProperty));

			if (itemUI.m_DurabilityBar.Active)
				itemUI.m_DurabilityBar.SetFillAmount(item.GetProperty(m_DurabilityProperty).Float / 100);

			// Add a CanvasGroup so we can set a global alpha value
			var group = itemUI.gameObject.AddComponent<CanvasGroup>();
			group.alpha = alpha;
			group.interactable = false;

			return itemUI.GetComponent<RectTransform>();
		}

		protected override void Awake()
		{
			Parent = GetComponentInParent<ItemContainerUI>();
			onStateChanged += OnStateChanged;
		}

		private void OnSlotChanged(ItemSlot itemSlot, SlotChangeType changeType) => DoRefresh();

		private void OnStateChanged(State state)
		{
			if (m_Stack != null)
				m_Stack.color = state == State.Normal ? m_NormalStackColor : m_HighlightStackColor;
		}
	}
}