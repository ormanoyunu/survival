using UnityEngine;

namespace SurvivalTemplatePro.UISystem
{
    public class ScopeUI : PlayerUIBehaviour
	{
        private static ScopeUI Instance { get; set; }

        [SerializeField]
        [Tooltip("The canvas group used to fade the scopes in & out.")]
        private CanvasGroup m_CanvasGroup;

		[SerializeField]
        [Tooltip("All of the existing UI scopes.")]
		private GameObject[] m_Scopes;

        private int m_CurrentScopeIndex = -1;
		private bool m_EnableScope;
		private float m_EnableTime;
        private float m_LerpSpeed;
        private float m_TargetAlpha;


        public override void OnAttachment()
        {
            foreach (var scope in m_Scopes)
                scope.SetActive(false);

            Instance = this;
        }

        public override void OnDetachment()
        {
            foreach (var scope in m_Scopes)
                scope.SetActive(false);

            Instance = null;
        }

        public static void EnableScope(int scopeIndex, float enableDelay = 0.5f, float lerpSpeed = 10f) 
        {
            if (Instance != null)
                Instance.EnableScopeUI(scopeIndex, enableDelay, lerpSpeed);
        }

        public static void DisableScope(float lerpSpeed)
        {
            if (Instance != null)
                Instance.DisableScopeUI(lerpSpeed);
        }

        private void EnableScopeUI(int index, float enableDelay = 0.5f, float lerpSpeed = 10f) 
        {
            m_EnableScope = true;
            m_EnableTime = Time.time + enableDelay;
            m_LerpSpeed = lerpSpeed;

            m_CanvasGroup.blocksRaycasts = true;

            int newIndex = Mathf.Clamp(index, 0, m_Scopes.Length - 1);

            // Change current scope
            if (newIndex != m_CurrentScopeIndex)
            {
                if (m_CurrentScopeIndex > -1)
                    m_Scopes[m_CurrentScopeIndex].SetActive(false);

                m_Scopes[newIndex].SetActive(true);
                m_CurrentScopeIndex = newIndex;
            }
        }

        private void DisableScopeUI(float lerpSpeed)
        {
            m_EnableScope = false;
            m_TargetAlpha = 0f;
            m_LerpSpeed = lerpSpeed;

            m_CanvasGroup.blocksRaycasts = false;

            if (m_CurrentScopeIndex > -1)
                m_Scopes[m_CurrentScopeIndex].SetActive(false);

            m_CurrentScopeIndex = -1;
        }

        private void Update()
        {
            if (m_EnableScope && Time.time > m_EnableTime)
            {
                m_TargetAlpha = 1f;
                m_EnableScope = false;
            }

            m_CanvasGroup.alpha = Mathf.MoveTowards(m_CanvasGroup.alpha, m_TargetAlpha, m_LerpSpeed * Time.deltaTime);
        }
    }
}