using UnityEngine;

namespace SurvivalTemplatePro.WieldableSystem
{
    public class CharacterUseBlocker : CharacterActionBlocker
    {
        [SerializeField, Space]
        private bool m_UseWhileAirborne;

        [SerializeField]
        private bool m_UseWhileCrouched = true;

        [SerializeField]
        private bool m_UseWhileRunning;

        [SerializeField]
        private bool m_UseWithoutStamina = true;

        private IStaminaController m_StaminaController;
        private IUseHandler m_UseHandler;
        private ICharacterMover m_Mover;


        public override void OnInitialized()
        {
            m_UseHandler = GetComponent<IUseHandler>();

            TryGetModule(out m_StaminaController);
            TryGetModule(out m_Mover);
        }

        protected override bool IsActionValid()
        {
            bool isValid = (m_UseWhileAirborne || Character.Mover.Motor.IsGrounded) && 
                           (m_UseWhileCrouched || !m_Mover.ActiveMotions.Has(CharMotionMask.Crouch)) &&
                           (m_UseWhileRunning || !m_Mover.ActiveMotions.Has(CharMotionMask.Run)) &&
                           (m_UseWithoutStamina || m_StaminaController.Stamina > 0.01f);

            return isValid;
        }

        protected override void BlockAction() => m_UseHandler.RegisterUseBlocker(this);
        protected override void UnblockAction() => m_UseHandler.UnregisterUseBlocker(this);
    }
}