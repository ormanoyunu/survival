using SurvivalTemplatePro.WieldableSystem;
using UnityEngine;
using UnityEngine.Events;

namespace SurvivalTemplatePro
{
    public interface IWieldableSurvivalBookHandler : ICharacterModule
    {
        bool InspectionActive { get; }
        IWieldable AttachedWieldable { get; }

        Transform LeftPages { get; }
        Transform RightPages { get; }

        event UnityAction onInspectionStarted;
        event UnityAction onInspectionEnded;

        void ToggleInspection();
    }
}