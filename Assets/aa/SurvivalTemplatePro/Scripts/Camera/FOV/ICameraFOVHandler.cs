using UnityEngine;

namespace SurvivalTemplatePro
{
    public interface ICameraFOVHandler : ICharacterModule
    {
        Camera UnityWorldCamera { get; }
        Camera UnityOverlayCamera { get; }

        float BaseWorldFOV { get; }
        float BaseOverlayFOV { get; }

        void SetCustomOverlayFOV(float fov);
        void SetCustomOverlayFOV(float fovMod, float setSpeed);
        void ClearCustomOverlayFOV(bool instantly = false);
        void SetCustomWorldFOV(float fovMod, float setSpeed = 10);
        void ClearCustomWorldFOV(bool instantly = false);
    }
}