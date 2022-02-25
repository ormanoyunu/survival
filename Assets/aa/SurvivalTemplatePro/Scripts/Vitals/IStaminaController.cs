using UnityEngine.Events;

namespace SurvivalTemplatePro
{
    public interface IStaminaController : ICharacterModule
    {
        float Stamina { get; }

        event UnityAction<float> onStaminaChanged;
    }
}