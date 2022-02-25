using UnityEngine;
using UnityEngine.Events;

namespace SurvivalTemplatePro
{
    public interface ILookHandler : ICharacterModule
    {
        Vector2 LookAngle { get; } 
        Vector2 CurrentInput { get; }

        Transform XTransform { get; set; }
        Transform YTransform { get; set; }

        event UnityAction onPostViewUpdate;

        void UpdateLook(Vector2 input);
        void AddAdditiveLookOverTime(Vector2 amount, float duration);
        void SetSensitivityMod(float mod);
    }
}