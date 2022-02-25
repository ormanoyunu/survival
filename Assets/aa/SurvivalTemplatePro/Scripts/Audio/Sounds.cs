using System;
using UnityEngine;

namespace SurvivalTemplatePro
{
    public interface ISound
    {
        AudioClip AudioClip { get; }
        float Volume { get; }
        float Pitch { get; }
        float Delay { get; }
    }

    [Serializable]
    public class StandardSound : ISound
    {
        public AudioClip AudioClip => m_Clips.Array.Select(ref m_LastSelected, SelectionType.RandomExcludeLast);
        public float Volume => m_Volume.Jitter(m_VolumeJitter);
        public float Pitch => m_Pitch.Jitter(m_PitchJitter);
        public float Delay => 0f;

        [SerializeField, Reorderable]
        private AudioClipList m_Clips;

        [SerializeField, Range(0f, 1f)]
        private float m_Volume = 0.65f;

        [SerializeField, Range(0f, 2f)]
        private float m_Pitch = 1f;

        [SerializeField, Range(0f, 0.5f)]
        private float m_VolumeJitter = 0.1f;

        [SerializeField, Range(0f, 0.5f)]
        private float m_PitchJitter = 0.1f;

        private int m_LastSelected;
    }

    [Serializable]
    public class SimpleSound : ISound
    {
        public AudioClip AudioClip => m_Clip;
        public float Volume => m_Volume;
        public float Pitch => 1f;
        public float Delay => 0f;

        [SerializeField]
        private AudioClip m_Clip;

        [SerializeField, Range(0f, 1f)]
        private float m_Volume = 0.5f;
    }

    [Serializable]
    public class DelayedSoundRandom : ISound
    {
        public AudioClip AudioClip => m_Clips.Array.Select(ref m_LastSelected, SelectionType.RandomExcludeLast);
        public float Volume => m_Volume;
        public float Pitch => 1f;
        public float Delay => m_Delay;

        [SerializeField, Reorderable]
        private AudioClipList m_Clips;

        [SerializeField, Range(0f, 1f)]
        private float m_Volume = 0.5f;

        [SerializeField, Range(0f, 20f)]
        private float m_Delay;

        private int m_LastSelected;
    }

    [Serializable]
    public class DelayedSound : ISound
    {
        public AudioClip AudioClip => m_Clip;
        public float Volume => m_Volume;
        public float Pitch => 1f;
        public float Delay => m_Delay;

        [SerializeField]
        private AudioClip m_Clip;

        [SerializeField, Range(0f, 1f)]
        private float m_Volume = 0.5f;

        [SerializeField, Range(0f, 20f)]
        private float m_Delay;
    }
}
