using System.Collections;
using UnityEngine;

namespace SurvivalTemplatePro.UISystem
{
    [RequireComponent(typeof(CanvasGroup))]
    public class FadePanelUI : MonoBehaviour
    {
        [SerializeField]
        private CanvasGroup m_CanvasGroup;

        [SerializeField, Range(0f, 25f)]
        private float m_FadeSpeed = 2f;


        public void Fade(bool toVisible, float fadeDelay = 0f) 
        {
            StopAllCoroutines();
            StartCoroutine(C_LerpAlpha(toVisible, fadeDelay));
        }

        private IEnumerator C_LerpAlpha(bool toVisible, float fadeDelay)
        {
            float targetAlpha = toVisible ? 1f : 0f;
            m_CanvasGroup.blocksRaycasts = toVisible;

            if (fadeDelay > 0.01f)
            {
                yield return new WaitForSeconds(fadeDelay);

                while (Mathf.Abs(m_CanvasGroup.alpha - targetAlpha) > 0.01f)
                {
                    m_CanvasGroup.alpha = Mathf.Lerp(m_CanvasGroup.alpha, targetAlpha, m_FadeSpeed * Time.deltaTime);

                    yield return null;
                }
            }

            m_CanvasGroup.alpha = targetAlpha;
        }
    }
}