using SurvivalTemplatePro.WieldableSystem;
using UnityEngine;
using UnityEngine.Events;

namespace SurvivalTemplatePro
{
    public class StaminaController : CharacterBehaviour, IStaminaController
    {
        public float Stamina
        {
            get => m_Stamina;
            private set
            {
                if (m_Stamina != value)
                {
                    m_Stamina = value;
                    onStaminaChanged?.Invoke(m_Stamina);
                }
            }
        }

        public event UnityAction<float> onStaminaChanged;

        [SerializeField, Range(0f, 3f)]
        [Tooltip("How fast will the stamina regenerate.")]
        private float m_RegenerationRate = 0.1f;

        [SerializeField, Range(0f, 5f)]
        [Tooltip("How much time the stamina regeneration will be paused after it gets lowered.")]
        private float m_RegenerationPause = 3f;

        [Space]

        [SerializeField, Range(0f, 1f)]
        [Tooltip("By how much will the stamina decrease per second when running")]
        private float m_RunDecreaseRate = 0.135f;

        [SerializeField, Range(0f, 1f)]
        [Tooltip("By how much will the stamina decrease after jumping.")]
        private float m_JumpTake = 0.15f;

        private ICharacterMover m_Mover;
        private IStaminaDepleter m_Depleter;
        private IWieldablesController m_Wieldables;

        private float m_Stamina;
        private float m_NextAllowedRegenTime;


        public override void OnInitialized()
        {
            m_Stamina = 1f;

            if (TryGetModule(out m_Mover))
                m_Mover.onMotionChanged += OnMotionChanged;

            if (TryGetModule(out m_Wieldables))
                m_Wieldables.onWieldableEquipped += OnWieldableChanged;
        }

        private void OnWieldableChanged(IWieldable wieldable)
        {
            if (m_Depleter != null)
                m_Depleter.onDepleteStamina -= OnDepleteStamina;

            m_Depleter = wieldable as IStaminaDepleter;

            if (m_Depleter != null)
                m_Depleter.onDepleteStamina += OnDepleteStamina;
        }

        private void OnDepleteStamina(float amount) => AdjustStamina(-amount);

        private void Update()
        {
            if (!IsInitialized)
                return;

            if (m_Mover.ActiveMotions.Has(CharMotionMask.Run))
                AdjustStamina(-m_RunDecreaseRate * Time.deltaTime);
            else if (Time.time > m_NextAllowedRegenTime)
                AdjustStamina(m_RegenerationRate * Time.deltaTime);
        }

        private void OnMotionChanged(CharMotionMask motion, bool isActive)
        {
            if (motion == CharMotionMask.Jump && isActive)
                AdjustStamina(-m_JumpTake);
        }

        private void AdjustStamina(float adjustment)
        {
            Stamina = Mathf.Clamp01(Stamina + adjustment);

            if (adjustment < 0f)
                m_NextAllowedRegenTime = Time.time + m_RegenerationPause;
        }
    }
}