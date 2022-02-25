using UnityEngine;
using UnityEngine.Events;

namespace SurvivalTemplatePro.WieldableSystem
{
    public interface IReloadHandler
    {
        bool IsReloading { get; }

        /// <summary> Bool: Start/End </summary>
        event UnityAction<bool> onReload;

        void StartReloading();
        void CancelReloading();

        void RegisterReloadBlocker(Object blocker);
        void UnregisterReloadBlocker(Object blocker);
    }
}