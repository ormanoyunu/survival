using System;
using UnityEngine;

namespace SurvivalTemplatePro
{
    [Serializable]
    public class AnimationOverrideClips
    {
        [Serializable]
        public struct AnimationClipPair
        {
            public AnimationClip Original;
            public AnimationClip Override;
        }

        public RuntimeAnimatorController Controller { get => m_Controller; set => m_Controller = value; }
        public AnimationClipPair[] Clips { get => m_Clips; }

        [SerializeField]
        private RuntimeAnimatorController m_Controller;

        [SerializeField]
        private AnimationClipPair[] m_Clips;
    }
}