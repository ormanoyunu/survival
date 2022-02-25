using UnityEngine;

namespace SurvivalTemplatePro.WieldableSystem
{
    [RequireComponent(typeof(Wieldable))]
    public class CharacterAimBlocker : CharacterActionBlocker
    {
        [SerializeField, Space]
        private bool m_AimWhileAirborne;

        [SerializeField]
        private bool m_AimWhileRunning;

        private IAimHandler m_AimHandler;
        private ICharacterMover m_Mover;


        public override void OnInitialized()
        {
            m_AimHandler = GetComponent<IAimHandler>();
            GetModule(out m_Mover);
        }

        protected override bool IsActionValid()
        {
            bool isValid = (m_AimWhileAirborne || m_Mover.Motor.IsGrounded) &&
                           (m_AimWhileRunning || !m_Mover.ActiveMotions.Has(CharMotionMask.Run));

            return isValid;
        }

        protected override void BlockAction() => m_AimHandler.RegisterAimBlocker(this);
        protected override void UnblockAction() => m_AimHandler.UnregisterAimBlocker(this);
    }
}
