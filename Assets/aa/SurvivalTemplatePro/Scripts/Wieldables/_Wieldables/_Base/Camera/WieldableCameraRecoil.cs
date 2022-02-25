using UnityEngine;

namespace SurvivalTemplatePro.WieldableSystem
{
    public class WieldableCameraRecoil : WieldableEffectsHandler
    {
        #region Internal
        [System.Serializable]
        protected class RecoilSettings : WieldableEffect
        {
            [SerializeField, HideInInspector]
            public WieldableCameraRecoil Recoil;

            public Vector2 ControllableRecoil;
            public float RecoilDuration;

            [Space]

            public FPRecoilForce Force;
            public ShakeSettings Shake;

            public override void TriggerEffect(float value) => Recoil.DoRecoil(this, value);
        }
        #endregion

        [SerializeField]
        private Sprind.Settings m_SpringSettings = Sprind.Settings.Default;

        [Space]

        [SerializeField]
        private RecoilSettings[] m_CustomRecoilSettings;

        private ICameraMotionHandler m_CameraMotion;
        private ILookHandler m_LookHandler;


        public override WieldableEffect[] GetAllEffects() => m_CustomRecoilSettings;

        protected override void InitModule(ICharacter character)
        {
            character.TryGetModule(out m_CameraMotion);
            character.TryGetModule(out m_LookHandler);
        }

        protected override void OnWieldableEquipped()
        {
            base.OnWieldableEquipped();
            m_CameraMotion.SetCustomForceSpringSettings(m_SpringSettings);
        }

        protected override void OnWieldableHolstered(float holsterSpeed)
        {
            m_CameraMotion.ClearCustomForceSpringSettings();
        }

        private void DoRecoil(RecoilSettings settings, float recoilMod)
        {
            if (!enabled)
                return;

            recoilMod = Mathf.Max(recoilMod, 0.3f);

            // Controllable recoil
            Vector2 recoil = new Vector2(-settings.ControllableRecoil.x, Random.Range(settings.ControllableRecoil.y, -settings.ControllableRecoil.y)) * recoilMod;
            m_LookHandler.AddAdditiveLookOverTime(recoil, settings.RecoilDuration);

            // Shake recoil
            m_CameraMotion.AddRotationForce(settings.Force.GetRotationForce() * recoilMod);
            m_CameraMotion.DoShake(settings.Shake, recoilMod);
        }

        private void OnValidate()
        {
            if (Application.isPlaying && m_CameraMotion != null)
                m_CameraMotion.SetCustomForceSpringSettings(m_SpringSettings);

            if (m_CustomRecoilSettings != null && m_CustomRecoilSettings.Length > 0)
            {
                foreach (var custom in m_CustomRecoilSettings)
                    custom.Recoil = this;
            }
        }
    }
}