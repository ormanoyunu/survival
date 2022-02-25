using UnityEngine.Events;

namespace SurvivalTemplatePro.WieldableSystem
{
    public interface ICrosshairHandler
    {
        int CrosshairIndex { get; set;  }

        event UnityAction<int> onCrosshairIndexChanged;

        void ResetCrosshair();
        float GetCrosshairAccuracy();
    }
}