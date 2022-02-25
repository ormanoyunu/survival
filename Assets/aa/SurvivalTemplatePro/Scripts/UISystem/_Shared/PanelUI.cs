using UnityEngine;
using UnityEngine.Events;

namespace SurvivalTemplatePro.UISystem
{
	/// <summary>
	/// Basic UI Panel that can be toggled on and off.
	/// It requires a show and hide animation to function.
	/// </summary>
    public class PanelUI : MonoBehaviour 
	{
		public bool IsVisible { get; private set; }
		public bool IsInteractable { get; private set; }
		public CanvasGroup CanvasGroup => m_CanvasGroup;

		public event UnityAction<bool> onToggled;

		[SerializeField]
		private bool m_ShowOnAwake;

        [Space]

		[SerializeField]
		private CanvasGroup m_CanvasGroup;

		[SerializeField]
		private Animator m_Animator;

		[BHeader("Animation Speed")]

		[SerializeField, Range(0f, 2f)]
		private float m_ShowSpeed = 1f;

		[SerializeField, Range(0f, 2f)]
		private float m_HideSpeed = 1f;

		[BHeader("Audio")]

		[SerializeField]
		private SoundPlayer m_ShowAudio;

		[SerializeField]
		private SoundPlayer m_HideAudio;

		private static int m_HashedShowTrigger = Animator.StringToHash("Show");
		private static int m_HashedHideTrigger = Animator.StringToHash("Hide");


		/// <summary>
		/// Show/Hide the panel.
		/// </summary>
		/// <param name="show"></param>
		public void Show(bool show)
		{
            if (IsVisible == show)
                return;

            if (m_Animator != null)
				m_Animator.SetTrigger(show ? m_HashedShowTrigger : m_HashedHideTrigger);

			SetIsInteractable(show);

			IsVisible = show;

            onToggled?.Invoke(IsVisible);

			if (show)
				m_ShowAudio.Play2D();
			else
				m_HideAudio.Play2D();
		}

		/// <summary>
		/// Reset the panel settings and animator.
		/// </summary>
        public void ResetToDefault()
        {
            if (m_Animator != null)
                m_Animator.Play(m_HashedHideTrigger, 0, 1f);

            SetIsInteractable(false);

            IsVisible = false;
        }

		/// <summary>
		/// Sets the "IsInteractable" flag
		/// </summary>
		/// <param name="isInteractable"></param>
        public void SetIsInteractable(bool isInteractable)
		{
			if (m_CanvasGroup != null)
				m_CanvasGroup.blocksRaycasts = isInteractable;

			IsInteractable = isInteractable;
		}		

        private void Start()
        {
			RefreshAnimationSpeeds();

			if (!m_ShowOnAwake)
				ResetToDefault();
			else
				Show(true);
		}

        private void OnValidate()
		{
			if (m_CanvasGroup == null)
				m_CanvasGroup = GetComponent<CanvasGroup>();

			if (m_Animator == null)
				m_Animator = GetComponent<Animator>();

			RefreshAnimationSpeeds();
		}

		private void RefreshAnimationSpeeds()
		{
			if (Application.isPlaying && m_Animator != null && m_Animator.isInitialized)
			{
				m_Animator.SetFloat("Show Speed", m_ShowSpeed);
				m_Animator.SetFloat("Hide Speed", m_HideSpeed);
			}
		}
	}
}