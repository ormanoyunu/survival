using UnityEngine;

namespace SurvivalTemplatePro.WieldableSystem
{
    [RequireComponent(typeof(Wieldable))]
    public class CharacterReloadBlocker : CharacterActionBlocker
    {
        [SerializeField, Space]
        private bool m_ReloadWhileRunning;

        private IReloadHandler m_ReloadHandler;
        private ICharacterMover m_Mover;


        public override void OnInitialized()
        {
            m_ReloadHandler = GetComponent<IReloadHandler>();
            GetModule(out m_Mover);
        }

        protected override bool IsActionValid()
        {
            bool isValid = !m_Mover.ActiveMotions.Has(CharMotionMask.Run) || m_ReloadWhileRunning;

            return isValid;
        }

        protected override void BlockAction() => m_ReloadHandler.RegisterReloadBlocker(this);
        protected override void UnblockAction() => m_ReloadHandler.UnregisterReloadBlocker(this);
    }
}