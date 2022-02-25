using UnityEngine;
using UnityEngine.Events;

namespace SurvivalTemplatePro.WieldableSystem
{
    public interface IUseHandler
    {
        event UnityAction onUse;

        void Use(UsePhase usePhase);

        void RegisterUseBlocker(Object blocker);
        void UnregisterUseBlocker(Object blocker);
    }

    public enum UsePhase
    {
        Start,
        Hold,
        End
    }
}