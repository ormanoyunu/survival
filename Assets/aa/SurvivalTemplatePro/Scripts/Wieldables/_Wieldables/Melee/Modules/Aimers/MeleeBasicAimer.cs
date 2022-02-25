using UnityEngine;

namespace SurvivalTemplatePro.WieldableSystem
{
	[RequireComponent(typeof(IWieldable))]
    public class MeleeBasicAimer : MeleeAimerBehaviour
    {
        [SerializeField, Range(0f, 5f)]
        private float m_AimThreshold;

		[SerializeField]
		private DelayedSoundRandom m_AimSounds;

		[BHeader("Crosshair")]

		[SerializeField, Range(-1, 24)]
		private int m_AimCrosshairIndex = 0;

		[BHeader("Local Effects")]

		[SerializeField, WieldableEffect]
		private int[] m_OnAimEffects;

		[SerializeField, WieldableEffect]
		private int[] m_OnAimEndEffects;

		[SerializeField, WieldableEffect(WieldableEffectPlayType.StopEffect)]
		private int[] m_AimEndEffectsToStop;

		private ICrosshairHandler m_CrosshairHandler;
		private float m_NextPossibleAimTime;


		public override bool TryStartAim()
		{
			if (IsAiming || Time.time < m_NextPossibleAimTime)
				return false;

			m_NextPossibleAimTime = Time.time + m_AimThreshold;

			// Crosshair
			m_CrosshairHandler.CrosshairIndex = m_AimCrosshairIndex;

			// Audio
			Wieldable.AudioPlayer.PlaySound(m_AimSounds);

			// Local Effects
			Wieldable.EventManager.PlayEffects(m_OnAimEffects, 1f);

			IsAiming = true;

			return true;
		}

        public override bool TryEndAim()
        {
			// Crosshair
			m_CrosshairHandler.ResetCrosshair();

			// Local Effects
			Wieldable.EventManager.PlayEffects(m_OnAimEndEffects, 1f);
			Wieldable.EventManager.StopEffects(m_AimEndEffectsToStop);

			IsAiming = false;

			return true;
		}

        protected override void Start()
        {
            base.Start();
			m_CrosshairHandler = Wieldable as ICrosshairHandler;
		}
    }
}