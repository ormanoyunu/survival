using UnityEngine.Events;

namespace SurvivalTemplatePro.WieldableSystem
{
    public abstract class FirearmAimerBehaviour : FirearmAttachmentBehaviour, IFirearmAimer
    {
        public bool IsAiming
        {
            get => m_IsAiming;
            protected set
            {
                if (value != m_IsAiming)
                {
                    m_IsAiming = value;
                    onAim?.Invoke(m_IsAiming);
                }
            }
        }

        public virtual float HipShootSpread => 1f;
        public virtual float AimShootSpread => 1f;

        public event UnityAction<bool> onAim;

        private bool m_IsAiming;


        public virtual bool TryStartAim() => IsAiming = true;
        public virtual bool TryEndAim() => IsAiming = false;

        protected virtual void OnEnable() => Firearm.SetAimer(this);
        protected virtual void OnDisable() => IsAiming = false;
    }
}
