using UnityEngine.Events;

namespace SurvivalTemplatePro
{
    public interface ITemperatureManager : ICharacterModule
    {
        float Temperature { get; set; }

        event UnityAction<float> onTemperatureChanged;
    }
}