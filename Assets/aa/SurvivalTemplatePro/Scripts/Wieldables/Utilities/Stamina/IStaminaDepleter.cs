using UnityEngine.Events;

namespace SurvivalTemplatePro.WieldableSystem
{
    public interface IStaminaDepleter
    {
        event UnityAction<float> onDepleteStamina;
    }
}