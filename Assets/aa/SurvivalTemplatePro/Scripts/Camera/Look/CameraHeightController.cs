using System.Collections;
using UnityEngine;

namespace SurvivalTemplatePro.CameraSystem
{
    public class CameraHeightController : CharacterBehaviour
	{
        #region Internal
        [System.Serializable]
		public struct EasingOptions
		{
			public Easings.Function Function;

			public float Duration;
		}
        #endregion

        [SerializeField, Range(0.001f, 20f)]
		[Tooltip("How fast should the camera adjust to the current Y position. (up - down)")]
		private float m_YLerpSpeed = 7f;

		[Space]

		[SerializeField, Range(-2f, 0f)]
		[Tooltip("An offset that will be applied to the camera position after crouching.")]
		private float m_CrouchOffset = -1f;

		[SerializeField]
		[Tooltip("Crouch movement easing type.")]
		private EasingOptions m_CrouchEasing;

		private float m_CurrentOffsetOnY;
		private float m_LastWorldSpaceYPos;
		private Vector3 m_InitialPosition;

		private Easer m_HeightEaser;

		private ICharacterMover m_Mover;
		private Coroutine m_CrouchCoroutine;


        public override void OnInitialized()
        {
			m_InitialPosition = transform.localPosition;
			m_HeightEaser = new Easer(m_CrouchEasing.Function, m_CrouchEasing.Duration);

			m_LastWorldSpaceYPos = transform.position.y;

			if (TryGetModule(out m_Mover))
			{
				m_Mover.onMotionChanged += OnStartCrouch;

				ILookHandler lookhandler = GetModule<ILookHandler>();
				lookhandler.onPostViewUpdate += UpdatePosition;
			}
		}

        private void UpdatePosition()
        {
			if (!IsInitialized || !Character.HealthManager.IsAlive || m_CrouchCoroutine != null)
				return;

			m_LastWorldSpaceYPos = Mathf.Lerp(m_LastWorldSpaceYPos, (Character.transform.position.y + m_InitialPosition.y + m_CurrentOffsetOnY), Time.deltaTime * (m_Mover.Motor.IsGrounded ? m_YLerpSpeed : 30f));
            transform.position = new Vector3(transform.position.x, m_LastWorldSpaceYPos, transform.position.z);
        }

        private void OnStartCrouch(CharMotionMask motionMask, bool isActive)
		{
			if (motionMask.Has(CharMotionMask.Crouch))
			{
				if (m_CrouchCoroutine != null)
					StopCoroutine(m_CrouchCoroutine);

				if (isActive)
					m_CrouchCoroutine = StartCoroutine(C_SetVerticalOffset(m_CrouchOffset));
				else
					m_CrouchCoroutine = StartCoroutine(C_SetVerticalOffset(0f));
			}
		}

		private IEnumerator C_SetVerticalOffset(float offset)
		{
			var startOffset = m_CurrentOffsetOnY;
			m_HeightEaser.Reset();

			while (m_HeightEaser.InterpolatedValue < 1f)
			{
				m_HeightEaser.Update(Time.deltaTime);
				m_CurrentOffsetOnY = Mathf.Lerp(startOffset, offset, m_HeightEaser.InterpolatedValue);

				transform.localPosition = m_InitialPosition + Vector3.up * m_CurrentOffsetOnY;

				yield return null;
			}
		}
	}
}