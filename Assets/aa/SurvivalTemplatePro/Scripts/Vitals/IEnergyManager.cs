using UnityEngine.Events;

namespace SurvivalTemplatePro
{
    public interface IEnergyManager : ICharacterModule
    {
        float Energy { get; set; }
        float MaxEnergy { get; set; }

        event UnityAction<float> onEnergyChanged;
        event UnityAction<float> onMaxEnergyChanged;
    }
}