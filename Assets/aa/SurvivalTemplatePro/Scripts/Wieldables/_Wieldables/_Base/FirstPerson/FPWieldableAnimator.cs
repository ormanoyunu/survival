using System.Collections.Generic;
using UnityEngine;
using System;

namespace SurvivalTemplatePro.WieldableSystem
{
    public class FPWieldableAnimator : WieldableEffectsHandler
    {
        #region Internal
        [Serializable]
        public class AnimatorParamaterEffect : WieldableEffect
        {
            [NonSerialized]
            public Animator Animator;

            [SerializeField]
            private AnimatorParameterTrigger[] m_Parameters;


            public AnimatorParamaterEffect Clone() => (AnimatorParamaterEffect)MemberwiseClone();

            public override void TriggerEffect(float value)
            {
                for (int i = 0; i < m_Parameters.Length; i++)
                    m_Parameters[i].TriggerParameter(Animator);
            }
        }
        #endregion

        public Animator Animator
        {
            get
            {
                if (m_Animator == null)
                    m_Animator = GetComponentInChildren<Animator>();

                return m_Animator;
            } 
        }

        public AnimationOverrideClips AnimationClips => m_Clips;

        [SerializeField]
        private AnimationOverrideClips m_Clips;

        [BHeader("Equip & Holster")]

        [SerializeField]
        private AnimatorParameterTrigger[] m_EquipParams = new AnimatorParameterTrigger[]
        {
            new AnimatorParameterTrigger(AnimatorControllerParameterType.Float, "EquipSpeed", 1f),
            new AnimatorParameterTrigger(AnimatorControllerParameterType.Trigger, "Equip", 1f)
        };

        [SerializeField]
        private AnimatorParameterTrigger[] m_HolsterParams = new AnimatorParameterTrigger[]
        {
            new AnimatorParameterTrigger(AnimatorControllerParameterType.Float, "HolsterSpeed", 1f),
            new AnimatorParameterTrigger(AnimatorControllerParameterType.Trigger, "Holster", 1f)
        };

        [Space]

        [SerializeField]
        private AnimatorParamaterEffect[] m_CustomAnimatorEffects;

        private Animator m_Animator;
        private IWieldable m_Wieldable;


        public override WieldableEffect[] GetAllEffects() => m_CustomAnimatorEffects;

        public void SetCustomStates(AnimatorParamaterEffect[] states) 
        {
            var animatorParams = new AnimatorParamaterEffect[states.Length];

            for (int i = 0; i < animatorParams.Length; i++)
                animatorParams[i] = states[i].Clone();

            m_CustomAnimatorEffects = animatorParams;
        }

        protected override void InitModule(ICharacter character) { }

        private void Awake()
        {
            m_Wieldable = GetComponentInParent<IWieldable>();

            SetupAnimator();

            m_Wieldable.onEquippingStarted += OnEquipStarted;
            m_Wieldable.onHolsteringStarted += OnHolsterStarted;
        }

        private void SetupAnimator()
        {
            m_Animator = GetComponentInChildren<Animator>(true);

            if (m_Animator != null && m_Clips.Controller != null)
            {
                var overrideController = new AnimatorOverrideController(m_Clips.Controller);
                var overrides = new List<KeyValuePair<AnimationClip, AnimationClip>>();

                foreach (var clipPair in m_Clips.Clips)
                    overrides.Add(new KeyValuePair<AnimationClip, AnimationClip>(clipPair.Original, clipPair.Override));

                overrideController.ApplyOverrides(overrides);

                m_Animator.runtimeAnimatorController = overrideController;

                for (int i = 0; i < m_CustomAnimatorEffects.Length; i++)
                    m_CustomAnimatorEffects[i].Animator = m_Animator;
            }
        }

        #region REFACTOR
        private void OnEquipStarted()
        {
            m_Animator.ResetTrigger("Holster");

            for (int i = 0; i < m_EquipParams.Length; i++)
                m_EquipParams[i].TriggerParameter(m_Animator);
        }

        private void OnHolsterStarted(float holsterSpeed)
        {
            for (int i = 0; i < m_HolsterParams.Length; i++)
            {
                if (m_HolsterParams[i].ParameterType == AnimatorControllerParameterType.Float)
                    m_HolsterParams[i].TriggerParameter(m_Animator, holsterSpeed);
                else
                    m_HolsterParams[i].TriggerParameter(m_Animator);
            }
        }
        #endregion
    }
}