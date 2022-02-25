using UnityEngine;

namespace SurvivalTemplatePro.WieldableSystem
{
    public class FirearmBasicTrigger : FirearmTriggerBehaviour
    {
		[Space]

		[SerializeField, Range(0f, 10f)]
		[Tooltip("The minimum time that can pass between consecutive shots.")]
		private float m_PressCooldown = 0f;

		private float m_NextTimeCanPress;


        protected override void TapTrigger()
        {
			if (Time.time > m_NextTimeCanPress)
			{
				Shoot(1f);
				m_NextTimeCanPress = Time.time + m_PressCooldown;
			}
		}
	}
}