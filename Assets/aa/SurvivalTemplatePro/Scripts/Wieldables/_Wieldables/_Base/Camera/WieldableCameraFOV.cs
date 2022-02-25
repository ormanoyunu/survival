using System;
using UnityEngine;

namespace SurvivalTemplatePro.WieldableSystem
{
    public class WieldableCameraFOV : WieldableEffectsHandler
    {
        #region Internal
        [Serializable]
        protected class FOVSettings : WieldableEffect
        {
            [SerializeField, HideInInspector]
            public WieldableCameraFOV FOV;

            [Range(0f, 2)]
            public float FOVMod = 1f;

            [Range(0f, 2f)]
            public float FOVSetDelay = 0f;

            [Range(1f, 100f)]
            public float FOVSetSpeed = 10f;

            public bool ResetInstantly = false;

            public override void TriggerEffect(float value) => FOV.SetFOV(this, value);
        }
        #endregion

        [SerializeField, Range(0f, 110f), InfoBox("Field of View of the FP Model")]
        private float m_OverlayFOV = 45f;

        [Space]

        [SerializeField]
        private FOVSettings[] m_CustomFOVSettings;

        private float m_SetFOVDelayedTime;
        private bool m_SetFOVDelayed;
        private float m_FOVMod;

        private ICameraFOVHandler m_FOV;
        private FOVSettings m_Current;


        public override WieldableEffect[] GetAllEffects() => m_CustomFOVSettings;

        public override void StopEffects()
        {
            m_FOV.ClearCustomWorldFOV(m_Current != null ? m_Current.ResetInstantly : false);
            m_SetFOVDelayed = false;
        }

        protected override void InitModule(ICharacter character) => character.TryGetModule(out m_FOV);

        protected override void OnWieldableEquipped()
        {
            base.OnWieldableEquipped();
            m_FOV.SetCustomOverlayFOV(m_OverlayFOV);
        }

        protected override void OnWieldableHolstered(float holsterSpeed)
        {
            m_FOV.ClearCustomWorldFOV(false);
            m_SetFOVDelayed = false;
        }

        private void Update()
        {
            if (m_SetFOVDelayed && Time.time > m_SetFOVDelayedTime)
                m_FOV.SetCustomWorldFOV(m_FOVMod, m_Current.FOVSetSpeed);
        }

        private void SetFOV(FOVSettings fov, float fovMod) 
        {
            m_Current = fov;
            m_FOVMod = fov.FOVMod * fovMod;
            m_SetFOVDelayedTime = Time.time + m_Current.FOVSetDelay;
            m_SetFOVDelayed = true;
        }

        private void OnValidate()
        {
            if (m_CustomFOVSettings != null && m_CustomFOVSettings.Length > 0)
            {
                foreach (var custom in m_CustomFOVSettings)
                    custom.FOV = this;
            }
        }
    }
}