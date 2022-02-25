using SurvivalTemplatePro.InventorySystem;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SurvivalTemplatePro.UISystem
{
    public class ItemWheelUI : PlayerUIBehaviour, IItemWheelUI
    {
        public ItemWheelState ItemWheelState => m_ItemWheelState;
        public bool IsVisible => m_Panel.IsVisible;

        [SerializeField]
        private string m_ContainerName;

        [SerializeField]
        private Animator m_Animator;

        [SerializeField]
        private PanelUI m_Panel;

        [BHeader("Wheel Feel")]

        [SerializeField, Range(0.1f, 25f)]
        private float m_Sensitivity = 3f;

        [SerializeField, Range(0.1f, 25f)]
        private float m_Range = 3f;

        [BHeader("Item Info")]

        [SerializeField]
        private string m_WeightNumberSuffix;

        [SerializeField]
        private Text m_DescriptionText;

        [SerializeField]
        private Text m_WeightText;

        [SerializeField]
        private Image m_WeightIcon;

        [SerializeField]
        private Text m_ItemNameText;

        [BHeader("Direction Arrow")]

        [SerializeField]
        private RectTransform m_DirectionArrow;

        [BHeader("Item Assign In Inventory")]

        [SerializeField]
        private CanvasGroup m_CanvasGroup;

        [SerializeField]
        private GameObject m_DescriptionPanel;

        private readonly Dictionary<WheelSlotUI, ItemSlot> m_SlotDictionary = new Dictionary<WheelSlotUI, ItemSlot>();
        private WheelSlotUI[] m_WheelSlots;

        private int m_LastSelectedSlot = -1;
        private int m_HighlightedSlot = -1;

        private Vector2 m_CursorPos;
        private Vector2 m_DirectionOfSelection;

        private ItemWheelState m_ItemWheelState;
        private IWieldableSelectionHandler m_SelectionHandler;
        private IPauseHandler m_PauseHandler;


        public void SetItemWheelState(ItemWheelState wheelState)
        {
            if (m_ItemWheelState != wheelState)
            {
                m_ItemWheelState = wheelState;

                switch (m_ItemWheelState)
                {
                    case ItemWheelState.SelectItems:
                        {
                            m_Animator.Play("Hide", 0, 1f);
                            m_CanvasGroup.blocksRaycasts = false;
                            m_CanvasGroup.interactable = false;

                            m_DescriptionPanel.SetActive(true);

                            break;
                        }
                    case ItemWheelState.InsertItems:
                        {
                            foreach (WheelSlotUI slot in m_WheelSlots)
                            {
                                slot.Deselect();
                                slot.SetSlotHighlights(WheelSlotUI.SelectionGraphicState.Normal);
                            }

                            m_Animator.Play("Show");
                            m_CanvasGroup.blocksRaycasts = true;
                            m_CanvasGroup.interactable = true;

                            m_DescriptionPanel.SetActive(false);

                            break;
                        }
                }
            }
        }

        public void SetItemWheelState(int wheelState) => SetItemWheelState((ItemWheelState)wheelState);

        public override void OnAttachment()
        {
            GetModule(out m_PauseHandler);
            GetModule(out m_SelectionHandler);

            m_ItemWheelState = ItemWheelState.SelectItems;
            m_WheelSlots = GetComponentsInChildren<WheelSlotUI>();

            ItemContainer container = PlayerInventory.GetContainerWithName(m_ContainerName);

            if (container != null)
            {
                for (int i = 0; i < container.Count; i++)
                {
                    m_SlotDictionary.Add(m_WheelSlots[i], container[i]);
                    m_WheelSlots[i].LinkToSlot(container[i]);
                }
            }
        }

        public void StartInspection()
        {
            if (m_PauseHandler.PauseActive)
                return;

            SetItemWheelState(ItemWheelState.SelectItems);

            m_Panel.Show(true);

            m_LastSelectedSlot = m_SelectionHandler.SelectedIndex;
            m_HighlightedSlot = m_SelectionHandler.SelectedIndex;

            // Set the highlight to the selected slot
            HandleSlotHighlighting(m_HighlightedSlot);

            m_PauseHandler.RegisterLocker(this, new PlayerPauseParams(false, true, true, true));
        }

        public void EndInspection()
        {
            m_Panel.Show(false);
            SelectSlot(m_HighlightedSlot);

            m_PauseHandler.UnregisterLocker(this);
        }

        public void UpdateSelection(Vector2 input)
        {
            if (!IsVisible || m_ItemWheelState == ItemWheelState.InsertItems)
                return;

            int highlightedSlot = GetHighlightedSlot(input);

            if (highlightedSlot != m_HighlightedSlot)
                HandleSlotHighlighting(highlightedSlot);
        }

        private int GetHighlightedSlot(Vector2 directionOfSelection)
        {
            directionOfSelection *= m_Range;

            if (directionOfSelection != Vector2.zero)
                m_DirectionOfSelection = Vector2.Lerp(m_DirectionOfSelection, directionOfSelection, Time.deltaTime * m_Sensitivity);

            m_CursorPos = m_DirectionOfSelection;

            float angle = -Vector2.SignedAngle(Vector2.up, m_CursorPos);

            if (angle < 0)
                angle = 360f - Mathf.Abs(angle);

            if (m_DirectionArrow != null)
                m_DirectionArrow.rotation = Quaternion.Euler(0f, 0f, -angle);

            angle = 360f - angle;

            float angleBetweenSlots = 360f / m_WheelSlots.Length;

            angle -= angleBetweenSlots / 2;

            if (angle > 360f)
                angle -= 360f;

            if (!(angle + angleBetweenSlots / 2 > 360 - angleBetweenSlots / 2))
                return Mathf.Clamp(Mathf.RoundToInt((angle + angleBetweenSlots / 2) / angleBetweenSlots), 0, m_WheelSlots.Length - 1);
            else
                return 0;
        }

        private void HandleSlotHighlighting(int targetSlotIndex)
        {
            m_WheelSlots[targetSlotIndex].SetSlotHighlights(WheelSlotUI.SelectionGraphicState.Highlighted);
            m_WheelSlots[targetSlotIndex].Select();

            // Disable the previous slot only if it's not the selected one
            if (m_LastSelectedSlot != m_HighlightedSlot)
                m_WheelSlots[m_HighlightedSlot].Deselect();

            m_WheelSlots[m_HighlightedSlot].SetSlotHighlights(WheelSlotUI.SelectionGraphicState.Normal); 

            m_HighlightedSlot = targetSlotIndex;
            TryShowSlotInfo(m_WheelSlots[targetSlotIndex]);
        }

        private void SelectSlot(int highlightedSlot)
        {
            m_SelectionHandler.SelectAtIndex(highlightedSlot);

            // Remove highlight from previous slot
            if (highlightedSlot != m_LastSelectedSlot)
            {
                m_WheelSlots[m_LastSelectedSlot].Deselect();
                m_WheelSlots[m_LastSelectedSlot].SetSlotHighlights(WheelSlotUI.SelectionGraphicState.Normal);
            }
        }

        private void TryShowSlotInfo(WheelSlotUI slot)
        {
            if (m_SlotDictionary.TryGetValue(slot, out ItemSlot itemSlot))
            {
                if (itemSlot != null && itemSlot.HasItem)
                {
                    m_ItemNameText.text = itemSlot.Item.Name;
                    m_DescriptionText.text = itemSlot.Item.Info.Description;
                    m_WeightText.text = itemSlot.Item.Info.Weight.ToString() + " " + m_WeightNumberSuffix;
                    m_WeightIcon.enabled = true;
                }
                else
                {
                    m_ItemNameText.text = "";
                    m_DescriptionText.text = "";
                    m_WeightText.text = "";
                    m_WeightIcon.enabled = false;
                }
            }
        }
    }
}
