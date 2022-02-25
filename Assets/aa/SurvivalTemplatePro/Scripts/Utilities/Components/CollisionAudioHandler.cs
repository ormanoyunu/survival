using UnityEngine;

namespace SurvivalTemplatePro
{
    public class CollisionAudioHandler : MonoBehaviour
    {
		[SerializeField]
		private AudioSource m_AudioSource;

		[Space]

		[SerializeField, Range(0, 100)]
		private int m_MaxPlayCount = 0;

		[SerializeField, Range(0f, 10f)]
		private float m_MinTimeThreshold = 0.5f;

		[SerializeField, Range(0f, 10f)]
		private float m_MinSpeed = 2f;

		[Space]

		[SerializeField]
		private SoundPlayer m_Audio;

		private int m_CollisionsCount = 0;
		private float m_NextTimeCanPlayAudio = 0f;


		private void OnCollisionEnter(Collision col)
		{
			bool shouldPlayAudio = m_MaxPlayCount <= 0 || m_CollisionsCount < m_MaxPlayCount;
			shouldPlayAudio &= Time.time > m_NextTimeCanPlayAudio;
			shouldPlayAudio &= col.relativeVelocity.magnitude > m_MinSpeed;

			if (shouldPlayAudio)
			{
				float volume = Mathf.Clamp(col.relativeVelocity.sqrMagnitude / m_MinSpeed / 10, 0.2f, 1f);

				if (m_AudioSource != null)
					m_Audio.Play(m_AudioSource, volume);
				else
					m_Audio.PlayAtPosition(transform.position, volume);

				m_NextTimeCanPlayAudio = Time.time + m_MinTimeThreshold;
				m_CollisionsCount++;
			}
		}

		private void OnValidate()
		{
			m_AudioSource = GetComponentInChildren<AudioSource>();

			if (m_AudioSource != null)
				m_AudioSource.spatialBlend = 1f;
		}
	}
}