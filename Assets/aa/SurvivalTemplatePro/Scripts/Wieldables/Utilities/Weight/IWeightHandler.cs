using UnityEngine.Events;

namespace SurvivalTemplatePro.WieldableSystem
{
    public interface IWeightHandler
    {
        float TotalWeight { get; }

        event UnityAction onWeightChanged;

        void AddWeight(float weight);
        void RemoveWeight(float weight);
    }
}