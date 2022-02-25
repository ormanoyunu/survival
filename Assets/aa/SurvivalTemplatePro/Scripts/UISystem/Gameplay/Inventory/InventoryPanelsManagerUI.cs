using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SurvivalTemplatePro.UISystem
{
    public class InventoryPanelsManagerUI : PlayerUIBehaviour
	{
        [SerializeField]
        private PanelUI m_CharacterPanel;

        [SerializeField]
        private PanelUI m_CharacterHeaderPanel;

        [SerializeField]
        private PanelUI m_BackpackPanel;

        [Space]

        [SerializeField]
        private UnityEvent m_InventoryOpenUnityEvent;

        [SerializeField]
        private UnityEvent m_InventoryCloseUnityEvent;

		private PanelUI[] m_Panels;
		private List<ItemContainerUI> m_ItemContainers = new List<ItemContainerUI>();

        private IInventoryInspectManager m_InventoryInspector;


        public override void OnAttachment()
        {
            if (TryGetModule(out m_InventoryInspector))
            {
                m_InventoryInspector.onInspectStarted += OnInventoryInspectStart;
                m_InventoryInspector.onInspectEnded += OnInventoryInspectEnd;
            }
        }

        private void OnInventoryInspectStart(InventoryInspectState inspectState)
        {
            if (inspectState == InventoryInspectState.Default)
            {
                m_CharacterPanel.Show(true);
                m_CharacterHeaderPanel.Show(true);
            }

            m_BackpackPanel.Show(true);

            m_InventoryOpenUnityEvent.Invoke();
        }

        private void OnInventoryInspectEnd()
        {
            foreach (PanelUI panel in m_Panels)
                panel.Show(false);

            m_InventoryCloseUnityEvent.Invoke();
        }

        private void Awake()
        {
            m_Panels = GetComponentsInChildren<PanelUI>(true);
            GetComponentsInChildren(false, m_ItemContainers);
        }
    }
}
