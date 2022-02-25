using SurvivalTemplatePro.UISystem;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace SurvivalTemplatePro.InputSystem
{
    public struct PointerRaycastEventParams
    {
        public GameObject RaycastObject { get; }
        public Vector2 RaycastPosition { get; }


        public PointerRaycastEventParams(GameObject raycastObject, Vector2 raycastPosition) 
        {
            this.RaycastObject = raycastObject;
            this.RaycastPosition = raycastPosition;
        }
    }

    [System.Serializable]
    public class PointerRaycastEvent : UnityEvent<PointerRaycastEventParams> { }

    /// <summary>
    /// TODO: Refactor/Remove (Use the event system instead)
    /// </summary>
    public class PlayerUIPointerInput : PlayerUIBehaviour
    {
        public bool PointerDown => m_PointerDownLastFrame;
        public bool AltPointerDown => m_AltPointerDownLastFrame;
        public bool PointerMoved => m_PointerMoved;
        public Vector2 PointerPosition => m_PointerPositionInput.action.ReadValue<Vector2>();
        public GameObject CurrentRaycast => m_RaycastResults.Count > 0 ? m_RaycastResults[0].gameObject : null;

        [SerializeField]
        private GraphicRaycaster m_Raycaster;

        [SerializeField]
        private EventSystem m_EventSystem;

        [Space]

        [SerializeField]
        private InputActionReference m_PointerPositionInput;

        [SerializeField]
        [Tooltip("Left Click")]
        private InputActionReference m_PointerClickInput;

        [SerializeField]
        [Tooltip("Right Click")]
        private InputActionReference m_PointerAltClickInput;

        [Space]

        [SerializeField]
        private PointerRaycastEvent m_PointerMovedCallback;

        [SerializeField]
        private UnityEvent m_PointerDownCallback;

        [SerializeField]
        private UnityEvent m_AltPointerDownCallback;

        private InputActionMap m_UIActionMap;

        private PointerEventData m_PointerEventData;
        private List<RaycastResult> m_RaycastResults = new List<RaycastResult>();

        private bool m_PointerMoved;
        private bool m_PointerDownLastFrame;
        private bool m_AltPointerDownLastFrame;
        private Vector2 m_PointerPositionLastFrame;

        private IInventoryInspectManager m_InventoryInspector;
        private ICustomActionManager m_CustomActionManager;


        public override void OnAttachment()
        {
            GetModule(out m_InventoryInspector);
            GetModule(out m_CustomActionManager);

            m_PointerEventData = new PointerEventData(m_EventSystem);

            m_UIActionMap = m_PointerPositionInput.action.actionMap;
            m_UIActionMap.Enable();

            m_PointerPositionLastFrame = m_PointerPositionInput.action.ReadValue<Vector2>();
        }

        public override void OnDetachment()
        {
            m_UIActionMap?.Disable();
        }

        public override void OnInterfaceUpdate()
        {
            if (m_InventoryInspector.InspectState == InventoryInspectState.None || m_CustomActionManager.CustomActionActive)
                return;

            UpdatePointerMovement();
            UpdatePointerDown();
            UpdateAltPointerDown();
        }

        private void UpdatePointerMovement() 
        {
            m_PointerMoved = (m_PointerPositionInput.action.ReadValue<Vector2>() - m_PointerPositionLastFrame) != Vector2.zero;

            if (m_PointerMoved)
            {
                Raycast();
                m_PointerMovedCallback?.Invoke(new PointerRaycastEventParams(CurrentRaycast, PointerPosition));
            }

            m_PointerPositionLastFrame = PointerPosition;
        }

        private void UpdatePointerDown() 
        {
            bool pointerDown = m_PointerClickInput.action.ReadValue<float>() > 0.5f;

            if (pointerDown && m_PointerDownLastFrame != pointerDown)
                m_PointerDownCallback?.Invoke();

            m_PointerDownLastFrame = pointerDown;
        }

        private void UpdateAltPointerDown() 
        {
            bool altPointerDown = m_PointerAltClickInput.action.ReadValue<float>() > 0.5f;

            if (altPointerDown && m_AltPointerDownLastFrame != altPointerDown)
                m_AltPointerDownCallback?.Invoke();

            m_AltPointerDownLastFrame = altPointerDown;
        }

        private void Raycast()
        {
            // Set the Pointer Event Position to that of the game object
            m_PointerEventData.position = m_PointerPositionInput.action.ReadValue<Vector2>();

            m_RaycastResults.Clear();

            // Raycast using the Graphics Raycaster and mouse click position
            m_Raycaster.Raycast(m_PointerEventData, m_RaycastResults);
        }
    }
}