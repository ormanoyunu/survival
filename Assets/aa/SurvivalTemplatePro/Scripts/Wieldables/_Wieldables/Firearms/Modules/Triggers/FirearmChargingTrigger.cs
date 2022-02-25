using UnityEngine;

namespace SurvivalTemplatePro.WieldableSystem
{
    public class FirearmChargingTrigger : FirearmTriggerBehaviour, IChargeHandler
	{
		[Space]

		[SerializeField, Range(0f, 10f)]
		[Tooltip("The minimum time that can pass between consecutive shots.")]
		private float m_PressCooldown = 0f;

		[SerializeField, Range(0f, 0.95f)]
		[Tooltip("Minimum charge needed to shoot")]
		private float m_MinChargeTime = 0f;

		[SerializeField, Range(0f, 10f)]
		private float m_MaxChargeTime = 1f;

		[SerializeField]
		private AnimationCurve m_ChargeCurve;

		[BHeader("Audio")]

		[SerializeField]
		private StandardSound m_ChargeStartSound;

		[SerializeField]
		private StandardSound m_ChargeEndSound;

		[BHeader("Local Effects")]

		[SerializeField, WieldableEffect]
		private int[] m_TriggerChargeStartEffects;

		[SerializeField, WieldableEffect]
		private int[] m_TriggerChargeEndEffects;

		[SerializeField, WieldableEffect]
		private int[] m_TriggerMaxChargeEffects;

		private float m_NextTimeCanHold;
		private float m_TriggerChargeStartTime;
		private bool m_TriggerChargeStarted = false;
		private bool m_ChargeMaxed = false;


        public override void HoldTrigger()
		{
			if (Time.time < m_NextTimeCanHold)
				return;

			if (Firearm.Reloader.IsReloading || Firearm.Reloader.IsMagazineEmpty)
			{
				Shoot(0f);
				return;
			}

			base.HoldTrigger();

			// Charge Start
			if (!m_TriggerChargeStarted && GetNormalizedCharge() > m_MinChargeTime)
			{
				// Audio
				Firearm.AudioPlayer.PlaySound(m_ChargeStartSound);

				// Local Effects
				EventManager.PlayEffects(m_TriggerChargeStartEffects, 1f);

				m_TriggerChargeStarted = true;
				m_TriggerChargeStartTime = Time.time;
			}

			// Charge Max
			if (!m_ChargeMaxed && GetNormalizedCharge() > (m_MaxChargeTime - 0.01f))
			{
				// Local Effects
				EventManager.PlayEffects(m_TriggerMaxChargeEffects, 1f);
				m_ChargeMaxed = true;
			}
		}

		public override void ReleaseTrigger()
		{
			if (!IsTriggerHeld)
				return;

			// Charge end
			if (GetNormalizedCharge() >= m_MinChargeTime)
			{
				float normalizedCharge = GetNormalizedCharge();
				float chargeAmount = normalizedCharge * m_ChargeCurve.Evaluate(normalizedCharge);

				Shoot(chargeAmount);

				// Audio
				Firearm.AudioPlayer.PlaySound(m_ChargeEndSound);

				// Local Effects
				EventManager.PlayEffects(m_TriggerChargeEndEffects, chargeAmount);
			}

			m_TriggerChargeStarted = false;
			m_ChargeMaxed = false;

			m_NextTimeCanHold = Time.time + m_PressCooldown;
			IsTriggerHeld = false;
		}

        public float GetNormalizedCharge()
        {
			if (!IsTriggerHeld)
				return 0f;

			float normalizedCharge = (Time.time - m_TriggerChargeStartTime) / m_MaxChargeTime;
			normalizedCharge = Mathf.Clamp(normalizedCharge, 0.05f, 1f);

			return normalizedCharge;
		}
    }
}