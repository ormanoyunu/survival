using UnityEngine;
using UnityEngine.Events;

namespace SurvivalTemplatePro.WieldableSystem
{
    public interface IAimHandler
    {
        bool IsAiming { get; }

        /// <summary> Bool: Start/End </summary>
        event UnityAction<bool> onAim;

        void StartAiming();
        void EndAiming();

        void RegisterAimBlocker(Object blocker);
        void UnregisterAimBlocker(Object blocker);
    }
}
