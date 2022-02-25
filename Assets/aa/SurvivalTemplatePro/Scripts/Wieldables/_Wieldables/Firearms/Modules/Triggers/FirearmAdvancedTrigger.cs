using System.Collections;
using UnityEngine;

namespace SurvivalTemplatePro.WieldableSystem
{
    public class FirearmAdvancedTrigger : FirearmTriggerBehaviour
	{
		#region Internal
		public enum FireMode
		{
			Safety,
			SemiAuto,
			Burst,
			FullAuto,
		}
		#endregion

		[Space]

		[SerializeField]
		private FireMode m_Firemode;

		[SerializeField, ShowIf("m_Firemode", (int)FireMode.SemiAuto)]
		[Tooltip("The minimum time that can pass between consecutive shots.")]
		private float m_PressCooldown = 0.22f;

		[SerializeField, ShowIf("m_Firemode", (int)FireMode.Burst)]
		[Tooltip("How many shots will the gun fire when in Burst-mode.")]
		private int m_BurstLength = 3;

		[SerializeField, ShowIf("m_Firemode", (int)FireMode.Burst)]
		[Tooltip("How much time it takes to fire all the shots.")]
		private float m_BurstDuration = 0.3f;

		[SerializeField, ShowIf("m_Firemode", (int)FireMode.Burst)]
		[Tooltip("The minimum time that can pass between consecutive bursts.")]
		private float m_BurstPause = 0.35f;

		[SerializeField, ShowIf("m_Firemode", (int)FireMode.FullAuto)]
		[Tooltip("The maximum amount of shots that can be executed in a minute.")]
		private int m_RoundsPerMinute = 450;

		private float m_NextTimeCanShoot;
		private float m_ShootThreshold;
		private WaitForSeconds m_BurstWait;


        public override void HoldTrigger()
        {
			base.HoldTrigger();

			if (Time.time < m_NextTimeCanShoot)
				return;

			if (m_Firemode == FireMode.FullAuto)
				Shoot(1f);

			m_NextTimeCanShoot = Time.time + m_ShootThreshold;
		}

		protected override void TapTrigger()
		{
			if (Time.time < m_NextTimeCanShoot)
				return;

			if (m_Firemode == FireMode.SemiAuto)
				Shoot(1f);
			else if (m_Firemode == FireMode.Burst)
				StartCoroutine(C_DoBurst());

			m_NextTimeCanShoot = Time.time + m_ShootThreshold;
		}

		private IEnumerator C_DoBurst()
        {
            for (int i = 0; i < m_BurstLength; i++)
            {
				Shoot(1f);

				yield return m_BurstWait;
            }
        }

		protected override void Awake()
		{
			base.Awake();

			HandleSettings();
		}

		private void HandleSettings() 
		{
			m_BurstWait = new WaitForSeconds(m_BurstDuration / m_BurstLength);

			if (m_Firemode == FireMode.SemiAuto)
				m_ShootThreshold = m_PressCooldown;
			else if (m_Firemode == FireMode.Burst)
				m_ShootThreshold = m_BurstDuration + m_BurstPause;
			else
				m_ShootThreshold = 60f / m_RoundsPerMinute;
		}

#if UNITY_EDITOR
		private void OnValidate() => HandleSettings();
#endif
	}
}
