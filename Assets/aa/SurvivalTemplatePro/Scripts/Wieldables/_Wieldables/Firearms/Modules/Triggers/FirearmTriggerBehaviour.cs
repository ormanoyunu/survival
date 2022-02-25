using UnityEngine.Events;

namespace SurvivalTemplatePro.WieldableSystem
{
    public abstract class FirearmTriggerBehaviour : FirearmAttachmentBehaviour, IFirearmTrigger
    {
        public bool IsTriggerHeld { get; protected set; }

        public event UnityAction<float> onShoot;


        public virtual void HoldTrigger()
        {
            if (!IsTriggerHeld)
                TapTrigger();

            IsTriggerHeld = true;
        }

        public virtual void ReleaseTrigger()
        {
            IsTriggerHeld = false;
        }

        protected virtual void TapTrigger() { }
        protected virtual void Shoot(float value) => onShoot?.Invoke(value);

        protected virtual void OnEnable() => Firearm.SetTrigger(this);
        protected virtual void OnDisable() { }
    }
}
