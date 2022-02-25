using SurvivalTemplatePro.CameraSystem;

namespace SurvivalTemplatePro
{
    public interface ICameraEffectsHandler : ICharacterModule
    {
        void DoAnimationEffect(CameraEffectSettings effect);
    }
}