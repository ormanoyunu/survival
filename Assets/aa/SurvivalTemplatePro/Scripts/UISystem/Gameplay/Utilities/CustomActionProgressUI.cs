using UnityEngine;
using UnityEngine.UI;

namespace SurvivalTemplatePro.UISystem
{
    /// <summary>
    /// TODO: Show multiple custom actions at once
    /// </summary>
    public class CustomActionProgressUI : PlayerUIBehaviour
    {
        [SerializeField]
        private CanvasGroup m_CanvasGroup;

        [SerializeField]
        private Image m_FillImg;

        [SerializeField]
        private Text m_LoadTxt;

        [SerializeField, Range(1f, 20f)]
        private float m_AlphaLerpSpeed = 10f;

        private ICustomActionManager m_ActionManager;
        private CustomActionParams m_CurrentAction;


        public override void OnAttachment()
        {
            if (TryGetModule(out m_ActionManager))
            {
                m_ActionManager.onActionStart += OnActionStart;
                m_ActionManager.onActionEnd += OnActionEnd;
            }

            m_CanvasGroup.alpha = 0f;
        }

        public override void OnDetachment()
        {
            if (m_ActionManager != null)
            {
                m_ActionManager.onActionStart -= OnActionStart;
                m_ActionManager.onActionEnd -= OnActionEnd;
            }
        }

        public override void OnInterfaceUpdate()
        {
            if (m_ActionManager.CustomActionActive)
            {
                m_CanvasGroup.alpha = Mathf.Lerp(m_CanvasGroup.alpha, 1f, Time.deltaTime * m_AlphaLerpSpeed);
                m_FillImg.fillAmount = m_CurrentAction.GetProgress();
            }
        }

        private void OnActionStart(CustomActionParams actionParams)
        {
            m_CurrentAction = actionParams;
            m_CanvasGroup.blocksRaycasts = true;

            m_LoadTxt.text = m_CurrentAction.Description;
        }

        private void OnActionEnd()
        {
            m_CanvasGroup.blocksRaycasts = false;

            m_CanvasGroup.alpha = 0f;
        }
    }
}