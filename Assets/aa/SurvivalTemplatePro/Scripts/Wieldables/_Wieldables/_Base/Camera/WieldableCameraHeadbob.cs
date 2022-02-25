using SurvivalTemplatePro.CameraSystem;
using System;
using UnityEngine;

namespace SurvivalTemplatePro.WieldableSystem
{
    public class WieldableCameraHeadbob : WieldableEffectsHandler
    {
        #region Internal

        [Serializable]
        protected class HeadbobSettings : WieldableEffect
        {
            [SerializeField, HideInInspector]
            public WieldableCameraHeadbob CameraHeadbob;

            public CameraBob Headbob;


            public override void TriggerEffect(float value) => CameraHeadbob.SetHeadbob(Headbob);
        }

        #endregion

        [SerializeField]
        private bool m_UseBaseHeadbob;

        [SerializeField]
        private CameraBob m_BaseHeadbob;

        [Space]

        [SerializeField]
        private HeadbobSettings[] m_CustomHeadbobSettings;

        private ICameraMotionHandler m_Motion;


        public override WieldableEffect[] GetAllEffects() => m_CustomHeadbobSettings;
        public override void StopEffects() => m_Motion.SetCustomHeadbob(m_UseBaseHeadbob ? m_BaseHeadbob : null, 1f);

        protected void SetHeadbob(CameraBob cameraHeadbob) => m_Motion.SetCustomHeadbob(cameraHeadbob, 1f);

        protected override void InitModule(ICharacter character)
        {
            character.TryGetModule(out m_Motion);
        }

        protected override void OnWieldableEquipped()
        {
            base.OnWieldableEquipped();
            m_Motion.SetCustomHeadbob(m_UseBaseHeadbob ? m_BaseHeadbob : null, 1f);
        }

        protected override void OnWieldableHolstered(float holsterSpeed)
        {
            m_Motion.SetCustomHeadbob(null, 1f);
        }

        protected void OnValidate()
        {
            if (m_CustomHeadbobSettings != null && m_CustomHeadbobSettings.Length > 0)
            {
                foreach (var headbob in m_CustomHeadbobSettings)
                    headbob.CameraHeadbob = this;
            }
        }
    }
}