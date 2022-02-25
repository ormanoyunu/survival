using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SurvivalTemplatePro.UISystem
{
    public class ItemActionsUI : MonoBehaviour
    {
        public event UnityAction onActionStart;
        public bool IsActive => m_IsActive;

        [SerializeField]
        private CanvasGroup m_CanvasGroup;

        [SerializeField]
        private Button m_ActionBtnTemplate;

        [SerializeField, HideInInspector]
        private ItemActionUI[] m_Actions;

        private ItemSlotUI m_ItemSlot;
        private Button[] m_ActionButtons;

        private bool m_IsActive;


        public void EnablePanel(bool enable)
        {
            m_IsActive = enable;
            m_CanvasGroup.alpha = enable ? 1f : 0f;
            m_CanvasGroup.blocksRaycasts = enable;
        }

        public void UpdateEnabledActions(ItemSlotUI slot)
        {
            if (slot == null || slot.Item == null)
                return;

            m_ItemSlot = slot;
            bool isSlotViable = m_ItemSlot != null && m_ItemSlot.HasItem;

            for (int i = 0; i < m_Actions.Length; i++)
            {
                bool isViable = isSlotViable && m_Actions[i].IsViableForItem(m_ItemSlot.ItemSlot);
                m_Actions[i].gameObject.SetActive(isViable);
            }
        }

        private void Awake()
        {
            m_ActionButtons = new Button[m_Actions.Length];

            m_Actions = GetComponentsInChildren<ItemActionUI>(true);

            for (int i = 0; i < m_Actions.Length; i++)
            {
                var actionRect = m_Actions[i].GetComponent<RectTransform>();
                var actionBtnRect = m_ActionBtnTemplate.GetComponent<RectTransform>();

                actionRect.anchorMin = actionBtnRect.anchorMin;
                actionRect.anchorMax = actionBtnRect.anchorMax;
                actionRect.anchoredPosition = actionBtnRect.anchoredPosition;
                actionRect.sizeDelta = actionBtnRect.sizeDelta;
                actionRect.pivot = actionBtnRect.pivot;
            }

            for (int i = 0; i < m_Actions.Length; i++)
            {
                var action = m_Actions[i];

                Button actionBtn = Instantiate(m_ActionBtnTemplate.gameObject, action.transform).GetComponent<Button>();
                actionBtn.GetComponentInChildren<Text>().text = action.ActionName;
                m_ActionButtons[i] = actionBtn;

                actionBtn.onClick.AddListener(() => StartAction(action));
                action.gameObject.SetActive(false);
            }
        }

        private void StartAction(ItemActionUI action) 
        {
            action.StartAction(m_ItemSlot.ItemSlot);
            onActionStart?.Invoke();
        }
    }
}