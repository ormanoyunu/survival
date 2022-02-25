using UnityEngine.Events;

namespace SurvivalTemplatePro
{
    public interface IThirstManager : ICharacterModule
    {
        bool HasMaxThirst { get; }

        float Thirst { get; set; }
        float MaxThirst { get; set;  }

        event UnityAction<float> onThirstChanged;
        event UnityAction<float> onMaxThirstChanged;
    }
}