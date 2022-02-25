using SurvivalTemplatePro.WorldManagement;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityStandardAssets.ImageEffects;

namespace SurvivalTemplatePro.CameraSystem
{
    /// <summary>
    /// Handles the Visual Effects of a camera.
    /// </summary>
    public class CameraEffectsHandler : CharacterBehaviour, ICameraEffectsHandler
    {
        [SerializeField]
        private Blur m_BackgroundBlur;

        [SerializeField]
        private Blur m_ForegroundBlur;

        [SerializeField]
        private ColorCorrectionCurves m_BackgroundColorTweaks;

        [SerializeField]
        private ColorCorrectionCurves m_ForegroundColorTweaks;

        private ColorGrading m_ColorGrading;
        private Vignette m_Vignette;
        private ChromaticAberration m_ChromaticAberration;

        private float m_DefaultExposure;
        private float m_DefaultSaturation;
        private float m_DefaultVignette;
        private float m_DefaultChromaticAberration;

        private Coroutine m_AnimationCoroutine;


        public override void OnInitialized()
        {
            if (PostProcessingManager.Instance == null)
                return;

            PostProcessingManager.OverlayVolume.profile.TryGetSettings(out m_ColorGrading);
            PostProcessingManager.OverlayVolume.profile.TryGetSettings(out m_Vignette);
            PostProcessingManager.OverlayVolume.profile.TryGetSettings(out m_ChromaticAberration);

            m_DefaultExposure = m_ColorGrading.postExposure.value;
            m_DefaultSaturation = m_ColorGrading.saturation.value;
            m_DefaultVignette = m_Vignette.intensity.value;
            m_DefaultChromaticAberration = m_ChromaticAberration.intensity.value;
        }

        public void EnablePauseEffects(PlayerPauseParams pauseParams)
        {
            m_BackgroundBlur.enabled = !pauseParams.BlurForeground && pauseParams.BlurBackground;
            m_ForegroundBlur.enabled = pauseParams.BlurForeground;

            m_BackgroundColorTweaks.enabled = !pauseParams.BlurForeground && pauseParams.EnableColorTweaks && pauseParams.BlurBackground;
            m_ForegroundColorTweaks.enabled = pauseParams.EnableColorTweaks && pauseParams.BlurForeground;
        }

        public void DisablePauseEffects()
        {
            m_BackgroundBlur.enabled = false;
            m_ForegroundBlur.enabled = false;
            m_BackgroundColorTweaks.enabled = false;
            m_ForegroundColorTweaks.enabled = false;
        }

        public void DoAnimationEffect(CameraEffectSettings effect)
        {
            if (effect == null)
                return;

            if (m_AnimationCoroutine != null)
            {
                StopCoroutine(m_AnimationCoroutine);
                ResetPostProcessingValues();
            }

            m_AnimationCoroutine = StartCoroutine(C_DoAnimatedEffect(effect));
        }

        private void ResetPostProcessingValues()
        {
            m_ColorGrading.postExposure.value = m_DefaultExposure;
            m_ColorGrading.saturation.value = m_DefaultSaturation;
            m_Vignette.intensity.value = m_DefaultVignette;
            m_ChromaticAberration.intensity.value = m_DefaultChromaticAberration;
        }

        private IEnumerator C_DoAnimatedEffect(CameraEffectSettings effects)
        {
            // Check if the module is initialized
            if (!IsInitialized)
                yield break;

            float EndTime = Time.time + effects.Duration;
            float animTimer = 0f;

            while (animTimer < 1f)
            {
                animTimer = 1 - ((EndTime - Time.time) / effects.Duration);

                if (effects.ExposureEffect.Enabled)
                    UpdateEffect(m_ColorGrading.postExposure, effects.ExposureEffect, animTimer, m_DefaultExposure);

                if (effects.SaturationEffect.Enabled)
                    UpdateEffect(m_ColorGrading.saturation, effects.SaturationEffect, animTimer, m_DefaultSaturation);

                if (effects.VignetteEffect.Enabled)
                    UpdateEffect(m_Vignette.intensity, effects.VignetteEffect, animTimer, m_DefaultVignette);

                if (effects.ChromaticAberationEffect.Enabled)
                    UpdateEffect(m_ChromaticAberration.intensity, effects.ChromaticAberationEffect, animTimer, m_DefaultChromaticAberration);

                yield return null;
            }

            ResetPostProcessingValues();
        }

        private void UpdateEffect(FloatParameter value, CameraEffectSettings.Effect effect, float animTime, float originalValue)
        {
            value.value = originalValue + animTime * effect.ValueChange * effect.ValueChangeOverTime.Evaluate(animTime);
        }
    }
}