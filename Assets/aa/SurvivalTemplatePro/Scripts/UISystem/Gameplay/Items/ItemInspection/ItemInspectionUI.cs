using SurvivalTemplatePro.InputSystem;
using UnityEngine;

namespace SurvivalTemplatePro.UISystem
{
    public class ItemInspectionUI : PlayerUIBehaviour
    {
        #region Internal
        public enum ItemInspectionState
        {
            ShowInfo = 1,
            ShowActions = 2,
            None = 0
        }
        #endregion

        [SerializeField]
        private RectTransform m_RectTransform;

        [Space]

        [SerializeField]
        private ItemInfoDisplayUI m_ItemInfo;

        [SerializeField]
        private ItemActionsUI m_ItemActions;

        private IInventoryInspectManager m_InventoryInspector;
        private ItemInspectionState m_State = ItemInspectionState.None;
        private ItemSlotUI m_InspectedSlot;
        private GameObject m_CurrentRaycast;

        private Vector2 m_PointerPosition;
        private bool m_UpdatePosition;
        private bool m_UpdateSelectedSlot;


        #region Called from Unity Events

        public void OnPointerClick() => SetState(ItemInspectionState.None);

        public void OnPointerAltClick()
        {
            UpdateSelectedSlot(m_CurrentRaycast, true);
            UpdatePosition(m_PointerPosition, true);

            if (m_InspectedSlot != null)
            {
                SetState(ItemInspectionState.ShowActions);

                if (!m_ItemActions.IsActive)
                    m_ItemActions.EnablePanel(true);
            }
        }

        public void OnPointerMoved(PointerRaycastEventParams pointerParams)
        {
            if (m_State == ItemInspectionState.None)
                SetState(ItemInspectionState.ShowInfo);

            UpdateSelectedSlot(pointerParams.RaycastObject);
            UpdatePosition(pointerParams.RaycastPosition);
        }

        #endregion

        public override void OnAttachment()
        {
            GetModule(out m_InventoryInspector);

            m_ItemActions.onActionStart += () => m_ItemActions.EnablePanel(false);
            m_InventoryInspector.onInspectEnded += OnInventoryInspectionEnded;
            m_InventoryInspector.onInspectStarted += OnInventoryInspectionStarted;

            m_ItemActions.EnablePanel(false);
            m_ItemInfo.EnablePanel(false);
        }

        public override void OnDetachment()
        {
            m_InventoryInspector.onInspectEnded -= OnInventoryInspectionEnded;
        }

        /// <summary>
        /// Hide panels when the inventory inspection ends
        /// </summary>
        private void OnInventoryInspectionEnded() => SetState(ItemInspectionState.None);
        private void OnInventoryInspectionStarted(InventoryInspectState state) => UpdatePosition(Vector2.zero, true);

        private void SetState(ItemInspectionState state)
        {
            if (m_State != state)
            {
                m_State = state;

                if (m_State == ItemInspectionState.ShowActions)
                {
                    m_ItemActions.EnablePanel(true);
                    m_ItemInfo.EnablePanel(false);
                    m_UpdatePosition = false;
                    m_UpdateSelectedSlot = false;
                }
                else if (m_State == ItemInspectionState.ShowInfo)
                {
                    m_ItemActions.EnablePanel(false);
                    m_ItemInfo.EnablePanel(true);
                    m_UpdatePosition = true;
                    m_UpdateSelectedSlot = true;
                }
                else
                {
                    m_ItemActions.EnablePanel(false);
                    m_ItemInfo.EnablePanel(false);
                    m_UpdatePosition = false;
                    m_UpdateSelectedSlot = false;
                }
            }
        }

        public void UpdateSelectedSlot(GameObject raycastObject, bool forceUpdate = false)
        {
            m_CurrentRaycast = raycastObject;

            if (m_UpdateSelectedSlot || forceUpdate)
            {
                if (m_CurrentRaycast != null && m_CurrentRaycast.TryGetComponent(out ItemSlotUI itemSlot) && itemSlot.HasItem)
                {
                    if (m_InspectedSlot != null)
                        m_InspectedSlot.onStateChanged -= UpdateSlotInfo;

                    m_InspectedSlot = itemSlot;
                    m_InspectedSlot.onStateChanged += UpdateSlotInfo;
                }
                else
                    m_InspectedSlot = null;
            }

            UpdateSlotInfo(SlotUI.State.Normal);
        }

        private void UpdateSlotInfo(SlotUI.State state)
        {
            m_ItemInfo.UpdateItemInfo(m_InspectedSlot);
            m_ItemActions.UpdateEnabledActions(m_InspectedSlot);
        }

        private void UpdatePosition(Vector2 pointerPosition, bool forceUpdate = false)
        {
            m_PointerPosition = pointerPosition;

            if (m_RectTransform != null && (m_UpdatePosition || forceUpdate))
            {
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(m_RectTransform.parent as RectTransform, m_PointerPosition, null, out Vector2 position))
                    m_RectTransform.anchoredPosition = position;
            }
        }
    }
}