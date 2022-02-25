using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SurvivalTemplatePro
{
    [Serializable]
	public class ShakeSettings
	{
		public Vector3 PositionAmplitude;
		public Vector3 RotationAmplitude;

		public AnimationCurve Decay = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);

		[Range(0f, 10f)]
		public float Duration = 2f;

		[Range(0f,100f)]
		public float Speed = 1f;
	}

	[Serializable]
	public class SpringShake
	{
		public bool IsDone => (Time.time > m_EndTime || Time.time == m_EndTime);

		private ShakeSettings m_Settings;

		private Sprind m_PositionSpring;
		private Sprind m_RotationSpring;

		private float m_Speed;
		private float m_Scale;
		private float xSign, ySign, zSign;

		private float m_EndTime;


		public SpringShake(ShakeSettings settings, Sprind positionSpring, Sprind rotationSpring, float scale = 1f)
		{
			m_Settings = settings;
			m_PositionSpring = positionSpring;
			m_RotationSpring = rotationSpring;

			m_Speed = m_Settings.Speed;
			m_Scale = scale;

			xSign = Random.Range(0, 100) > 50 ? 1f : -1f;
			ySign = Random.Range(0, 100) > 50 ? 1f : -1f;
			zSign = Random.Range(0, 100) > 50 ? 1f : -1f;

			m_EndTime = Time.time + settings.Duration;
		}

		public void Update()
		{
			if (IsDone)
				return;

			UpdateShake(m_PositionSpring, m_Settings.PositionAmplitude);
			UpdateShake(m_RotationSpring, m_Settings.RotationAmplitude);
		}

		private void UpdateShake(Sprind spring, Vector3 amplitude)
		{
			float timer = (m_EndTime - Time.time) * m_Speed;

			Vector3 shake = new Vector3(
				xSign * Mathf.Sin(timer) * amplitude.x * m_Scale,
				ySign * Mathf.Cos(timer) * amplitude.y * m_Scale,
				zSign * Mathf.Sin(timer) * amplitude.z * m_Scale
			);

			spring.AddForce(shake * m_Settings.Decay.Evaluate(1f - (m_EndTime - Time.time) / m_Settings.Duration));
		}
	}
}