using System.Collections.Generic;
using UnityEngine;

namespace SurvivalTemplatePro.WieldableSystem
{
    [RequireComponent(typeof(FPWieldableMotion))]
    public class FPWieldableRecoil : WieldableEffectsHandler
    {
        #region Internal
        [System.Serializable]
        protected class RecoilSettings : WieldableEffect
        {
            [SerializeField, HideInInspector]
            public FPWieldableRecoil Recoil;

            public FPRecoilForce Force;

            [Space]

            public bool DoShake;
            public ShakeSettings Shake;

            public override void TriggerEffect(float recoilMod)
            {
                Recoil.AddRecoilForce(Force, recoilMod);

                if (DoShake)
                    Recoil.DoShake(Shake, recoilMod);
            }
        }
        #endregion

        [SerializeField, Range(0f, 100f)]
        private float m_SpringLerpSpeed = 30f;

        [Space]

        [SerializeField]
        private Sprind.Settings m_PositionSpring = Sprind.Settings.Default;

        [SerializeField]
        private Sprind.Settings m_RotationSpring = Sprind.Settings.Default;

        [Space]

        [SerializeField]
        private RecoilSettings[] m_CustomRecoilSettings;

        private Sprind m_PosSpring;
        private Sprind m_RotSpring;

        private readonly List<SpringShake> m_Shakes = new List<SpringShake>();


        #region Public Methods
        public void AddRecoilForce(FPRecoilForce recoilForce, float recoilMod = 1f)
        {
            m_PosSpring.AddForce(recoilForce.GetPositionForce(recoilMod));
            m_RotSpring.AddForce(recoilForce.GetRotationForce(recoilMod));
        }

        public void AddPositionRecoil(SpringForce force) => m_PosSpring.AddForce(force);
        public void AddRotationRecoil(SpringForce force) => m_RotSpring.AddForce(force);

        public void SetCustomSpringSettings(Sprind.Settings positionSettings, Sprind.Settings rotationSettings)
        {
            m_PosSpring.Adjust(positionSettings);
            m_RotSpring.Adjust(rotationSettings);
        }

        public void ClearCustomSpringSettings() 
        {
            m_PosSpring.Adjust(m_PositionSpring);
            m_RotSpring.Adjust(m_RotationSpring);
        }

        public void DoShake(ShakeSettings shake, float shakeScale = 1f) => m_Shakes.Add(new SpringShake(shake, m_PosSpring, m_RotSpring, shakeScale));
        #endregion

        public override WieldableEffect[] GetAllEffects() => m_CustomRecoilSettings;

        private void Awake()
        {
            Transform pivot = GetComponent<FPWieldableMotion>().Pivot;

            m_PosSpring = new Sprind(Sprind.RefreshType.AddToPosition, pivot, Vector3.zero, m_SpringLerpSpeed);
            m_RotSpring = new Sprind(Sprind.RefreshType.AddToRotation, pivot, Vector3.zero, m_SpringLerpSpeed);

            SetCustomSpringSettings(m_PositionSpring, m_RotationSpring);
        }

        private void FixedUpdate()
        {
            float fixedDeltaTime = Time.fixedDeltaTime;

            m_PosSpring.FixedUpdate(fixedDeltaTime);
            m_RotSpring.FixedUpdate(fixedDeltaTime);
        }

        private void Update()
        {
            float deltaTime = Time.deltaTime;

            UpdateShakes();

            m_PosSpring.Update(deltaTime);
            m_RotSpring.Update(deltaTime);
        }

        private void UpdateShakes()
        {
            if (m_Shakes.Count == 0)
                return;

            int i = 0;

            while (true)
            {
                if (m_Shakes[i].IsDone)
                    m_Shakes.RemoveAt(i);
                else
                {
                    m_Shakes[i].Update();
                    i++;
                }

                if (i >= m_Shakes.Count)
                    break;
            }
        }

        private void OnValidate()
        {
            if (Application.isPlaying && m_PosSpring != null)
            {
                m_PosSpring.Adjust(m_PositionSpring);
                m_RotSpring.Adjust(m_RotationSpring);
            }

            if (m_CustomRecoilSettings != null && m_CustomRecoilSettings.Length > 0)
            {
                foreach (var custom in m_CustomRecoilSettings)
                    custom.Recoil = this;
            }
        }
    }
}