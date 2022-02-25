using SurvivalTemplatePro.WorldManagement;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace SurvivalTemplatePro.UISystem
{
    [RequireComponent(typeof(Animator), typeof(CanvasGroup))]
    public class SleepUI : PlayerUIBehaviour
    {
        [SerializeField]
        [Tooltip("The canvas group used to fade the sleep UI in & out.")]
        private CanvasGroup m_CanvasGroup;

        [Space]

        [SerializeField, Range(0f, 1f)]
        [Tooltip("The max alpha that will be reached when fading in.")]
        private float m_MaxCanvasAlpha = 1f;

        [SerializeField, Range(0.1f, 10f)]
        [Tooltip("The speed at which the sleep UI will be faded in & out.")]
        private float m_CanvasLerpSpeed = 3f;

        [Space]

        [SerializeField]
        [Tooltip("A UI text component that's used for displaying the current time while sleeping.")]
        private Text m_CurrentTimeText;

        [SerializeField]
        [Tooltip("The background rect of this UI piece.")]
        private RectTransform m_BackgroundRect;

        [Space]

        [SerializeField]
        [Tooltip("Reference to the animator used for the sleeping UI.")]
        private Animator m_Animator;

        [SerializeField, AnimatorParameter(AnimatorControllerParameterType.Trigger)]
        [Tooltip("The 'show' animator trigger.")]
        private string m_ShowTrigger = "Show";

        [SerializeField, AnimatorParameter(AnimatorControllerParameterType.Trigger)]
        [Tooltip("The 'hide' animator trigger.")]
        private string m_HideTrigger = "Hide";

        [Space]

        [SerializeField, InfoBox("Number of text templates each side")]
        [Tooltip("How many time templates should be spawned each side.")]
        private int m_NumberOfTemplates;

        [SerializeField]
        [Tooltip("A prefab with a text component on it that will be instantiated.")]
        private Text m_TimeTextTemplate;

        [SerializeField]
        [Tooltip("How will the text scale over the duration of its lifetime.")]
        private AnimationCurve m_TextSizeOverDistance;

        [SerializeField]
        [Tooltip("How will the text y position change over the duration of its lifetime.")]
        private AnimationCurve m_TextYPositionOverDistance;

        [SerializeField]
        [Tooltip("How will the color of the text change over the duration of its lifetime.")]
        private Gradient m_TextColorOverDistance;

        private float m_BackgroundRectHalfSize;
        private ISleepHandler m_SleepHandler;
        private Text[] m_TimeTexts;


        public override void OnAttachment()
        {
            GetModule(out m_SleepHandler);
            m_SleepHandler.onSleepStart += OnSleepStart;

            m_BackgroundRectHalfSize = m_BackgroundRect.rect.size.x / 2f;
            m_CanvasGroup.alpha = 0f;

            SetupTemplates();
        }

        public override void OnDetachment()
        {
            m_SleepHandler.onSleepStart -= OnSleepStart;
        }

        private void OnSleepStart(GameTime timeToSleep)
        {
            StopAllCoroutines();
            StartCoroutine(C_UpdateTimeGraphics());
        }

        private void SetupTemplates()
        {
            // Instantiate templates
            m_TimeTexts = new Text[m_NumberOfTemplates * 2];

            for (int i = 0; i < m_TimeTexts.Length; i++)
                m_TimeTexts[i] = Instantiate(m_TimeTextTemplate, m_BackgroundRect);
        }

        private IEnumerator C_UpdateTimeGraphics() 
        {
            // Update Current Time HUD
            m_CurrentTimeText.text = WorldManagerBase.Instance.GetGameTime().GetTimeToString(true, true, false);

            ResetSideTexts(WorldManagerBase.Instance.GetGameTime().Hours);
            UpdateSideTexts(WorldManagerBase.Instance.GetGameTime());

            // Trigger the show animation
            m_Animator.SetTrigger(m_ShowTrigger);

            // Show canvas
            while (m_CanvasGroup.alpha < m_MaxCanvasAlpha - 0.01f)
            {
                m_CanvasGroup.alpha = Mathf.Lerp(m_CanvasGroup.alpha, m_MaxCanvasAlpha, m_CanvasLerpSpeed * Time.deltaTime);
                yield return null;
            }

            m_CanvasGroup.alpha = m_MaxCanvasAlpha;

            // Update time texts
            while (m_SleepHandler.SleepActive)
            {
                // Get the currentTime
                var gameTime = WorldManagerBase.Instance.GetGameTime();

                // Update Current Time HUD
                m_CurrentTimeText.text = gameTime.GetTimeToString(true, true, false);

                UpdateSideTexts(gameTime);

                yield return null;
            }

            // Trigger the hide animation
            m_Animator.SetTrigger(m_HideTrigger);

            // Hide Canvas
            while (m_CanvasGroup.alpha > 0.001f)
            {
                m_CanvasGroup.alpha = Mathf.Lerp(m_CanvasGroup.alpha, 0f, m_CanvasLerpSpeed * Time.deltaTime);
                yield return null;
            }

            m_CanvasGroup.alpha = 0f;
        }

        private void UpdateSideTexts(GameTime gameTime) 
        {
            // Update side time texts
            for (int i = 0; i < m_TimeTexts.Length; i++)
            {
                var textRect = m_TimeTexts[i].rectTransform;

                // Update position
                textRect.anchoredPosition -= new Vector2(WorldManagerBase.Instance.GetTimeIncrementPerSecond() * 24f * (m_BackgroundRectHalfSize / m_NumberOfTemplates) * Time.deltaTime, 0f);

                // Reset Text
                if (textRect.anchoredPosition.x < -m_BackgroundRectHalfSize)
                {
                    m_TimeTexts[i].text = ((int)Mathf.Repeat(gameTime.Hours + m_NumberOfTemplates, 23f)).ToString("00") + ":00";
                    textRect.anchoredPosition = new Vector2(m_BackgroundRectHalfSize, 0f);
                }

                // Update graphics
                float distanceFromCenter = GetDistanceFromCenter(textRect);
                UpdateTextGraphics(m_TimeTexts[i], textRect, distanceFromCenter);
            }
        }

        private float GetDistanceFromCenter(RectTransform textRect)
        {
            return Mathf.Abs(m_BackgroundRect.anchoredPosition.x - textRect.anchoredPosition.x) / m_BackgroundRectHalfSize;
        }

        // Update the text templates based on their distance from the center of the background rect
        private void UpdateTextGraphics(Text text, RectTransform textRect, float distanceFromCenter) 
        {
            text.color = m_TextColorOverDistance.Evaluate(distanceFromCenter);

            textRect.anchoredPosition = new Vector2(textRect.anchoredPosition.x, m_TextYPositionOverDistance.Evaluate(distanceFromCenter));
            textRect.localScale = Vector3.one * m_TextSizeOverDistance.Evaluate(distanceFromCenter);
        }

        private void ResetSideTexts(int currentHour)
        {
            float distanceBetweenTemplates = m_BackgroundRectHalfSize / m_NumberOfTemplates;
            float currentXPosition = 0f;
            currentHour++;

            for (int i = 0; i < m_TimeTexts.Length; i++)
            {
                currentXPosition += distanceBetweenTemplates;
                m_TimeTexts[i].text = ((int)Mathf.Repeat(currentHour + i, 23f)).ToString("00") + ":00";
                m_TimeTexts[i].rectTransform.anchoredPosition = new Vector2(currentXPosition, 0f);
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (m_Animator == null)
                m_Animator = GetComponent<Animator>();
        }
#endif
    }
}