using System.Collections;
using UnityEngine;

namespace SurvivalTemplatePro.UISystem
{
    public class SurvivalBookUI : PlayerUIBehaviour
    {
        [SerializeField]
        private ChildOfConstraint m_MenuCanvas;

        [SerializeField]
        private ChildOfConstraint m_ContentCanvas;

        private IWieldableSurvivalBookHandler m_SurvivalBook;

        private Coroutine m_UIVisibilityToggler;
        private Vector3 m_MenuCanvasScale;
        private Vector3 m_ContentCanvasScale;


        public override void OnAttachment()
        {
            if (TryGetModule(out m_SurvivalBook))
            {
                Camera camera = GetModule<ICameraFOVHandler>().UnityOverlayCamera;

                foreach (var canvas in GetComponentsInChildren<Canvas>(true))
                {
                    if (canvas.renderMode == RenderMode.WorldSpace)
                        canvas.worldCamera = camera;
                }

                foreach (var category in GetComponentsInChildren<IBookCategoryUI>(true))
                    category.AttachToPlayer(Player);

                m_MenuCanvas.Parent = m_SurvivalBook.LeftPages;
                m_ContentCanvas.Parent = m_SurvivalBook.RightPages;

                m_MenuCanvasScale = m_MenuCanvas.transform.localScale;
                m_ContentCanvasScale = m_ContentCanvas.transform.localScale;

                m_SurvivalBook.onInspectionStarted += ShowUI;
                m_SurvivalBook.onInspectionEnded += HideUI;

                HideUI();
            }
        }

        public override void OnDetachment()
        {
            if (m_SurvivalBook != null)
            {
                m_SurvivalBook.onInspectionStarted -= ShowUI;
                m_SurvivalBook.onInspectionEnded -= HideUI;
            }
        }

        private void ShowUI()
        {
            // Show the UI with a delay
            if (m_UIVisibilityToggler != null)
                StopCoroutine(m_UIVisibilityToggler);

            m_UIVisibilityToggler = StartCoroutine(C_ShowUIWithDelay(true, 0.3f));
        }

        private void HideUI() 
        {
            // Hide the UI with a delay
            if (m_UIVisibilityToggler != null)
                StopCoroutine(m_UIVisibilityToggler);

            m_UIVisibilityToggler = StartCoroutine(C_ShowUIWithDelay(false, m_SurvivalBook.AttachedWieldable.HolsterDuration));
        }

        private IEnumerator C_ShowUIWithDelay(bool show, float delay)
        {
            float timer = Time.time + delay;

            while (Time.time < timer)
                yield return null;

            m_MenuCanvas.transform.localScale = show ? m_MenuCanvasScale : Vector3.zero;
            m_ContentCanvas.transform.localScale = show ? m_ContentCanvasScale : Vector3.zero;
        }
    }
}
