using UnityEngine;

namespace SurvivalTemplatePro.WieldableSystem
{
    public abstract class MeleeAimerBehaviour : MonoBehaviour
    {
        public bool IsAiming { get; protected set; }
        protected IWieldable Wieldable { get; private set; }


        public virtual bool TryStartAim() => IsAiming = true;
        public virtual bool TryEndAim() => IsAiming = false;

        protected virtual void Start() => Wieldable = GetComponent<IWieldable>();
    }
}