using System;
using UnityEngine;

namespace SurvivalTemplatePro
{
	/// <summary>
	/// Handles playing sounds for humanoid characters.
	/// </summary>
    public class HumanSoundsManager : CharacterBehaviour
    {
		#region Internal
		[Serializable]
		private struct MovementAudio
		{
			public StandardSound RunAudio;
			public StandardSound JumpAudio;
			public StandardSound CrouchAudio;
			public StandardSound StandUpAudio;
		}

		[Serializable]
		private struct DamageAudio
		{
			[Range(0f, 100f), Tooltip("")]
			public float DamageAmountThreshold;

			[Tooltip("The sounds that will be played when this entity receives damage.")]
			public StandardSound GenericDamageAudio;

			[Range(0f, 50f), Tooltip("")]
			public float FallDamageSpeedThreshold;

			public StandardSound FallDamageAudio;
			public StandardSound DeathAudio;

			[Range(0f, 100f), Tooltip("")]
			public float HeartbeatHealthThreshold;

			public StandardSound HeartbeatAudio;
		}

		[Serializable]
		private struct StaminaAudio
		{
			[Range(0.1f, 25f)]
			public float BreathingHeavyDuration;

			public StandardSound BreathingHeavyAudio;
		}
		#endregion

		[SerializeField]
		private MovementAudio m_MovementAudio;

		[SerializeField]
		private DamageAudio m_DamageAudio;

		[SerializeField]
		private StaminaAudio m_StaminaAudio;

		private IAudioPlayer m_AudioPlayer;
		private ICharacterMover m_CharacterMover;
		private IHealthManager m_HealthManager;
		private IStaminaController m_StaminaController;

		private bool m_HeartbeatActive;
		private float m_LastHeavyBreathTime;


        public override void OnInitialized()
        {
			GetModule(out m_CharacterMover);
			GetModule(out m_AudioPlayer);
			GetModule(out m_HealthManager);
			GetModule(out m_StaminaController);

			m_CharacterMover.onMotionChanged += OnMotionChanged;
			m_CharacterMover.onFallImpact += OnFallImpact;

			m_HealthManager.onDeath += OnDeath;
			m_HealthManager.onHealthChanged += OnHealthChanged;

			m_StaminaController.onStaminaChanged += OnStaminaChanged;
		}

        private void OnDestroy()
        {
			if (m_CharacterMover != null)
			{
				m_CharacterMover.onMotionChanged -= OnMotionChanged;
				m_CharacterMover.onFallImpact -= OnFallImpact;
			}

			if (m_HealthManager != null)
			{
				m_HealthManager.onDeath -= OnDeath;
				m_HealthManager.onHealthChanged -= OnHealthChanged;
			}

			if (m_StaminaController != null)
				m_StaminaController.onStaminaChanged -= OnStaminaChanged;
		}

        private void OnMotionChanged(CharMotionMask motionMask, bool isActive)
		{
            switch (motionMask)
            {
                case CharMotionMask.Run:
					m_AudioPlayer.PlaySound(m_MovementAudio.RunAudio);
					break;
                case CharMotionMask.Crouch:
					m_AudioPlayer.PlaySound(isActive ? m_MovementAudio.CrouchAudio : m_MovementAudio.StandUpAudio);
					break;
                case CharMotionMask.Jump:
					if (isActive) m_AudioPlayer.PlaySound(m_MovementAudio.JumpAudio);
					break;
            }
        }

        private void OnDeath() => m_AudioPlayer.PlaySound(m_DamageAudio.DeathAudio);

		private void OnFallImpact(float impactSpeed)
		{
			// Don't play the clip when the impact speed is low
			bool wasHardImpact = Mathf.Abs(impactSpeed) >= m_DamageAudio.FallDamageSpeedThreshold;
			wasHardImpact &= m_HealthManager.Health < m_HealthManager.PrevHealth;

			if (wasHardImpact)
				m_AudioPlayer.PlaySound(m_DamageAudio.FallDamageAudio);
		}

		private void OnHealthChanged(float health)
		{
			// On damage...
			if (health < m_HealthManager.PrevHealth)
			{
				if ((m_HealthManager.PrevHealth - health) > m_DamageAudio.DamageAmountThreshold)
					m_AudioPlayer.PlaySound(m_DamageAudio.GenericDamageAudio);

				// Start heartbeat loop sound...
				if (!m_HeartbeatActive && health < m_DamageAudio.HeartbeatHealthThreshold)
				{
					m_AudioPlayer.LoopSound(m_DamageAudio.HeartbeatAudio, 1000f);
					m_HeartbeatActive = true;
				}
			}
			// On health restored...
			else
			{
				// Stop heartbeat loop sound...
				if (m_HeartbeatActive && health > m_DamageAudio.HeartbeatHealthThreshold)
				{
					m_AudioPlayer.StopLoopingSound(m_DamageAudio.HeartbeatAudio);
					m_HeartbeatActive = false;
				}
			}
		}

		private void OnStaminaChanged(float stamina)
		{
			if (Time.time - m_LastHeavyBreathTime > m_StaminaAudio.BreathingHeavyDuration)
			{
				if (stamina < 0.01f)
				{
					m_AudioPlayer.LoopSound(m_StaminaAudio.BreathingHeavyAudio, m_StaminaAudio.BreathingHeavyDuration);
					m_LastHeavyBreathTime = Time.time;
				}
			}
		}
    }
}