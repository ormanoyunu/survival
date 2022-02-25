using UnityEngine;

namespace SurvivalTemplatePro.MovementSystem
{
    public class CharacterJumpBlocker : CharacterBehaviour
    {
        [SerializeField, Range(0f, 0.5f)]
        [Tooltip("At which stamina value (0-1) will the ability to jump be disabled.")]
        private float m_DisableJumpOnStaminaValue = 0.1f;

        [SerializeField, Range(0f, 0.5f)]
        [Tooltip("At which stamina value (0-1) will the ability to jump be re-enabled (if disabled)")]
        private float m_EnableJumpOnStaminaValue = 0.3f;

        private ICharacterMover m_Mover;
        private IStaminaController m_Stamina;

        private bool m_JumpDisabled;


        public override void OnInitialized()
        {
            GetModule(out m_Mover);
            GetModule(out m_Stamina);

            m_Stamina.onStaminaChanged += OnStaminaChanged;
        }

        private void OnStaminaChanged(float stamina)
        {
            if (!m_JumpDisabled && stamina < m_DisableJumpOnStaminaValue)
            {
                m_Mover.BlockedMotions = m_Mover.BlockedMotions.SetFlag(CharMotionMask.Jump);
                m_JumpDisabled = true;
            }
            else if (m_JumpDisabled && stamina > m_EnableJumpOnStaminaValue)
            {
                m_Mover.BlockedMotions = m_Mover.BlockedMotions.UnsetFlag(CharMotionMask.Jump);
                m_JumpDisabled = false;
            }
        }
    }
} 