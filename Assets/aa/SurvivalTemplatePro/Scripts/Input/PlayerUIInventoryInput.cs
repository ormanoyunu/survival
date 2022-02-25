using SurvivalTemplatePro.UISystem;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace SurvivalTemplatePro.InputSystem
{
    [System.Serializable]
    public struct DragEventParams
    {
        public GameObject DragStartRaycast { get; }
        public GameObject CurrentRaycast { get; }
        public Vector2 CurrentPointerPosition { get; }
        public bool SplitItemStack { get; }


        public DragEventParams(GameObject dragStartRaycast, GameObject currentRaycast, Vector2 currentPointerPosition, bool splitItemStack)
        {
            this.DragStartRaycast = dragStartRaycast;
            this.CurrentRaycast = currentRaycast;
            this.CurrentPointerPosition = currentPointerPosition;
            this.SplitItemStack = splitItemStack;
        }
    }

    [System.Serializable]
    public class SlotEvent : UnityEvent<ItemSlotUI> {  }

    [System.Serializable]
    public class DragEvent : UnityEvent<DragEventParams> {  }

    public class PlayerUIInventoryInput : PlayerUIBehaviour
    {
        [SerializeField]
        private InputActionReference m_InventoryToggleInput;

        [SerializeField]
        private InputActionReference m_InventoryCloseInput;

        [SerializeField]
        private InputActionReference m_AutoMoveItemInput;

        [SerializeField]
        private InputActionReference m_SplitItemStackInput;

        [Space]

        [SerializeField]
        private SlotEvent m_AutoMoveItemCallback;

        [SerializeField]
        private DragEvent m_DragStartCallback;

        [SerializeField]
        private DragEvent m_DragCallback;

        [SerializeField]
        private DragEvent m_DragEndCallback;

        private InputActionMap m_PlayerUIActionMap;
        private PlayerUIPointerInput m_PointerInput;

        private GameObject m_DragStartRaycast;
        private bool m_IsDragging;

        private IInventoryInspectManager m_InventoryInspector;
        private ICustomActionManager m_CustomActionManager;


        public override void OnAttachment()
        {
            m_PointerInput = GetComponentInParent<PlayerUIPointerInput>();

            GetModule(out m_InventoryInspector);
            GetModule(out m_CustomActionManager);

            m_PlayerUIActionMap = m_AutoMoveItemInput.action.actionMap;
            m_PlayerUIActionMap.Enable();

            m_InventoryToggleInput.action.started += OnInventoryToggleInput;
            m_InventoryCloseInput.action.started += OnInventoryCloseInput;
            m_AutoMoveItemInput.action.performed += OnAutoMoveItemInput;
        }

        public override void OnDetachment()
        {
            m_PlayerUIActionMap?.Disable();

            m_InventoryToggleInput.action.started -= OnInventoryToggleInput;
            m_InventoryCloseInput.action.started -= OnInventoryCloseInput;
            m_AutoMoveItemInput.action.performed -= OnAutoMoveItemInput;
        }

        public override void OnInterfaceUpdate()
        {
            if (!Player.HealthManager.IsAlive)
                return;

            UpdateDragging();
        }

        private void OnInventoryToggleInput(InputAction.CallbackContext context)
        {
            if (m_CustomActionManager.CustomActionActive)
                return;

            if (m_InventoryInspector.InspectState == InventoryInspectState.None)
                m_InventoryInspector.TryInspect(InventoryInspectState.Default);
            else
                m_InventoryInspector.TryStopInspecting();
        }

        private void OnInventoryCloseInput(InputAction.CallbackContext obj)
        {
            if (m_InventoryInspector.InspectState != InventoryInspectState.None)
            {
                m_InventoryInspector.TryStopInspecting();
            }
        }

        private void OnAutoMoveItemInput(InputAction.CallbackContext context)
        {
            if (m_InventoryInspector.InspectState == InventoryInspectState.None)
                return;

            if (m_PointerInput.CurrentRaycast != null && m_PointerInput.CurrentRaycast.TryGetComponent(out ItemSlotUI slot))
                m_AutoMoveItemCallback.Invoke(slot);
        }

        private void UpdateDragging()
        {
            if (!m_IsDragging && m_PointerInput.PointerDown && m_PointerInput.PointerMoved)
            {
                m_IsDragging = true;

                m_DragStartRaycast = m_PointerInput.CurrentRaycast;
                bool splitItemStack = m_SplitItemStackInput.action.ReadValue<float>() == 1f;
                m_DragStartCallback.Invoke(new DragEventParams(m_DragStartRaycast, m_DragStartRaycast, m_PointerInput.PointerPosition, splitItemStack));
            }
            else if (m_IsDragging && !m_PointerInput.PointerDown)
            {
                m_IsDragging = false;

                bool splitItemStack = m_SplitItemStackInput.action.ReadValue<float>() == 1f;
                m_DragEndCallback.Invoke(new DragEventParams(m_DragStartRaycast, m_PointerInput.CurrentRaycast, m_PointerInput.PointerPosition, splitItemStack));
            }
            else if (m_IsDragging && m_PointerInput.PointerMoved)
            {
                bool splitItemStack = m_SplitItemStackInput.action.ReadValue<float>() == 1f;
                m_DragCallback.Invoke(new DragEventParams(m_DragStartRaycast, m_PointerInput.CurrentRaycast, m_PointerInput.PointerPosition, splitItemStack));
            }
        }
    }
}