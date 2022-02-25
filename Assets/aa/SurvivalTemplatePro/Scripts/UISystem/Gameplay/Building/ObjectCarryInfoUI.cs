using UnityEngine;

namespace SurvivalTemplatePro.UISystem
{
    public class ObjectCarryInfoUI : PlayerUIBehaviour
    {
        [SerializeField]
        private CanvasGroup m_CanvasGroup;

        [SerializeField, Range(0.1f, 20f)]
        private float m_AlphaLerpSpeed = 7f;

        private float m_TargetAlpha = 0;
        private IObjectCarryController m_ObjectCarry; 


        public override void OnAttachment()
        {
            GetModule(out m_ObjectCarry);

            m_ObjectCarry.onObjectCarryStart += OnObjectCarryStart;
            m_ObjectCarry.onObjectCarryEnd += OnObjectCarryEnd;
        }

        private void OnObjectCarryEnd() => m_TargetAlpha = 0f;
        private void OnObjectCarryStart() => m_TargetAlpha = 1f;

        public override void OnInterfaceUpdate()
        {
            m_CanvasGroup.alpha = Mathf.MoveTowards(m_CanvasGroup.alpha, m_TargetAlpha, Time.deltaTime * m_AlphaLerpSpeed);
        }
    }
}