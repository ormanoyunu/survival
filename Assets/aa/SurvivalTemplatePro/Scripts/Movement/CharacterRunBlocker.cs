using UnityEngine;

namespace SurvivalTemplatePro.MovementSystem
{
    public class CharacterRunBlocker : CharacterBehaviour
    {
        [SerializeField, Range(0f, 0.5f)]
        [Tooltip("At which stamina value (0-1) will the ability to run be disabled.")]
        private float m_DisableRunOnStaminaValue = 0.1f;

        [SerializeField, Range(0f, 0.5f)]
        [Tooltip("At which stamina value (0-1) will the ability to run be re-enabled (if disabled)")]
        private float m_EnableRunOnStaminaValue = 0.3f;

        private ICharacterMover m_Mover;
        private IStaminaController m_Stamina;

        private bool m_RunDisabled;


        public override void OnInitialized()
        {
            GetModule(out m_Mover);
            GetModule(out m_Stamina);

            m_Stamina.onStaminaChanged += OnStaminaChanged;
        }

        private void OnStaminaChanged(float stamina)
        {
            if (!m_RunDisabled && stamina < m_DisableRunOnStaminaValue)
            {
                m_Mover.BlockedMotions = m_Mover.BlockedMotions.SetFlag(CharMotionMask.Run);
                m_RunDisabled = true;
            }
            else if (m_RunDisabled && stamina > m_EnableRunOnStaminaValue)
            {
                m_Mover.BlockedMotions = m_Mover.BlockedMotions.UnsetFlag(CharMotionMask.Run);
                m_RunDisabled = false;
            }
        }
    }
}