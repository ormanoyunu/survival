using System.Collections.Generic;
using UnityEngine;

namespace SurvivalTemplatePro
{
    public class AudioPlayer : MonoBehaviour, IAudioPlayer
    {
        #region Internal
        public struct QueuedSound
        {
            public ISound Sound { get; }
            public float StopTime { get; }
            public int AudioSourceIndex { get; }


            public QueuedSound(ISound sound, int audioSourceIndex)
            {
                this.Sound = sound;
                this.StopTime = Time.time + sound.Delay;
                this.AudioSourceIndex = audioSourceIndex;
            }
        }

        public class LoopingQueuedSound
        {
            public ISound Sound { get; }
            public float StopTime { get; set; }
            public float LoopVolume { get; }
            public int AudioSourceIndex { get; }


            public LoopingQueuedSound(ISound sound, float duration, int audioSourceIndex)
            {
                this.Sound = sound;
                this.StopTime = Time.time + duration;
                this.LoopVolume = sound.Volume;
                this.AudioSourceIndex = audioSourceIndex;
            }
        }
        #endregion

        [SerializeField]
        private Transform m_PlayRoot;

        [Space]

        [SerializeField, Range(0, 24)]
        private int m_MainAudioSourcesCount = 6;

        [SerializeField, Range(0, 24)]
        private int m_LoopingAudioSourcesCount = 6;

        private readonly List<QueuedSound> m_QueuedSounds = new List<QueuedSound>();
        private readonly List<AudioSource> m_MainAudioSources = new List<AudioSource>();
        private readonly List<AudioSource> m_LoopingAudioSources = new List<AudioSource>();
        private readonly List<LoopingQueuedSound> m_LoopingQueuedSounds = new List<LoopingQueuedSound>();

        private int m_MainAudioSourceIndex = -1;
        private int m_LoopingAudioSourceIndex = -1;


        public void PlaySound(ISound sound)
        {
            if (sound == null || m_MainAudioSourcesCount <= 0)
                return;

            IncrementMainAudioSourcesIndex();

            if (sound.Delay > 0.03f)
                m_QueuedSounds.Add(new QueuedSound(sound, m_MainAudioSourceIndex));
            else
            {
                var audioClip = sound.AudioClip;

                if (audioClip != null)
                {
                    m_MainAudioSources[m_MainAudioSourceIndex].pitch = sound.Pitch;
                    m_MainAudioSources[m_MainAudioSourceIndex].PlayOneShot(audioClip, sound.Volume);
                }
            }
        }

        public void PlaySounds(ISound[] sounds) 
        {
            if (sounds == null)
                return;

            for (int i = 0; i < sounds.Length; i++)
                PlaySound(sounds[i]);
        }

        public void LoopSound(ISound sound, float duration)
        {
            if (sound == null || m_LoopingAudioSourcesCount <= 0)
                return;

            IncrementLoopingAudioSourcesIndex();
            m_LoopingQueuedSounds.Add(new LoopingQueuedSound(sound, duration, m_LoopingAudioSourceIndex));
        }

        public void ClearAllQueuedSounds() => m_QueuedSounds.Clear();

        public void StopLoopingSound(ISound sound)
        {
            if (sound == null)
                return;

            for (int i = 0; i < m_LoopingQueuedSounds.Count; i++)
            {
                if (m_LoopingQueuedSounds[i].Sound == sound)
                    m_LoopingQueuedSounds[i].StopTime = Time.time + 0.5f;
            }
        }

        private void Awake()
        {
            for (int i = 0; i < m_MainAudioSourcesCount; i++)
            {
                m_MainAudioSources.Add(AudioManager.Instance.CreateAudioSource(m_PlayRoot.gameObject, false, 1f, 1f, "Effects"));
                m_MainAudioSources[i].loop = false;
            }

            for (int i = 0; i < m_LoopingAudioSourcesCount; i++)
            {
                m_LoopingAudioSources.Add(AudioManager.Instance.CreateAudioSource(m_PlayRoot.gameObject, false, 1f, 1f, "Effects"));
                m_LoopingAudioSources[i].loop = true;
            }
        }

        private void Update()
        {
            if (m_QueuedSounds.Count > 0)
                UpdateDelayedSounds();

            if (m_LoopingQueuedSounds.Count > 0)
                UpdateLoopingSounds();
        }

        private void UpdateDelayedSounds() 
        {
            for (int i = 0; i < m_QueuedSounds.Count; i++)
            {
                QueuedSound queuedSound = m_QueuedSounds[i];

                if (Time.time >= queuedSound.StopTime)
                {
                    ISound sound = queuedSound.Sound;

                    m_MainAudioSources[queuedSound.AudioSourceIndex].pitch = sound.Pitch;
                    m_MainAudioSources[queuedSound.AudioSourceIndex].PlayOneShot(sound.AudioClip, sound.Volume);

                    m_QueuedSounds.RemoveAt(i);
                }
            }
        }

        private void UpdateLoopingSounds()
        {
            for (int i = 0; i < m_LoopingQueuedSounds.Count; i++)
            {
                AudioSource loopingAudioSource = m_LoopingAudioSources[m_LoopingQueuedSounds[i].AudioSourceIndex];

                if (!loopingAudioSource.isPlaying)
                {
                    loopingAudioSource.clip = m_LoopingQueuedSounds[i].Sound.AudioClip;
                    loopingAudioSource.pitch = m_LoopingQueuedSounds[i].Sound.Pitch;
                    loopingAudioSource.Play();
                }

                // Fade in volume
                if (Time.time < m_LoopingQueuedSounds[i].StopTime)
                    loopingAudioSource.volume = Mathf.MoveTowards(loopingAudioSource.volume, m_LoopingQueuedSounds[i].LoopVolume, Time.deltaTime * 2f);
                // Fade out volume
                else
                {
                    loopingAudioSource.volume = Mathf.MoveTowards(loopingAudioSource.volume, 0f, Time.deltaTime);

                    if (loopingAudioSource.volume < 0.01f)
                    {
                        loopingAudioSource.Stop();
                        m_LoopingQueuedSounds.RemoveAt(i);
                    }
                }
            }
        }

        private void IncrementMainAudioSourcesIndex() => m_MainAudioSourceIndex = (int)Mathf.Repeat(m_MainAudioSourceIndex + 1, m_MainAudioSourcesCount - 1);
        private void IncrementLoopingAudioSourcesIndex() => m_LoopingAudioSourceIndex = (int)Mathf.Repeat(m_LoopingAudioSourceIndex + 1, m_LoopingAudioSourcesCount - 1);
    }
}