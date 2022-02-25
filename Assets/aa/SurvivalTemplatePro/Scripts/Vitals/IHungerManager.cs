using UnityEngine.Events;

namespace SurvivalTemplatePro
{
    public interface IHungerManager : ICharacterModule
    {
        bool HasMaxHunger { get; }

        float Hunger { get; set; }
        float MaxHunger { get; set;  }

        event UnityAction<float> onHungerChanged;
        event UnityAction<float> onMaxHungerChanged;
    }
}