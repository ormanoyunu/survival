using UnityEngine.Events;

namespace SurvivalTemplatePro.WieldableSystem
{
    public interface IFirearmTrigger : IFirearmAttachment
    {
        bool IsTriggerHeld { get; }

        event UnityAction<float> onShoot;


        void HoldTrigger();
        void ReleaseTrigger();
    }
}