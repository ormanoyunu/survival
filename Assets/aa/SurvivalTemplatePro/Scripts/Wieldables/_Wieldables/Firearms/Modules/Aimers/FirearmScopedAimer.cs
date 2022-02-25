using SurvivalTemplatePro.UISystem;
using UnityEngine;

namespace SurvivalTemplatePro.WieldableSystem
{
    public class FirearmScopedAimer : FirearmBasicAimer
    {
        [BHeader("Scope")]

        [SerializeField, Range(0f, 10f)]
        private float m_ScopeEnableDelay = 0.3f;

        [SerializeField, Range(0, 24)]
        private int m_ScopeIndex;

        [Space]

        [SerializeField]
        private GameObject m_ObjectToDisable;

        private bool m_HideGun;
        private float m_HideTime;
        private Vector3 m_DefaultGunModelScale;


        public override bool TryStartAim()
        {
            base.TryStartAim();

            if (!IsAiming)
                return false;

            // Scope
            ScopeUI.EnableScope(m_ScopeIndex, m_ScopeEnableDelay, 10f);

            m_HideGun = true;
            m_HideTime = Time.time + m_ScopeEnableDelay;

            return true;
        }

        public override bool TryEndAim()
        {
            base.TryEndAim();

            if (IsAiming)
                return false;

            ScopeUI.DisableScope(5f);

            m_HideGun = false;
            m_ObjectToDisable.transform.localScale = m_DefaultGunModelScale;

            return true;
        }

        protected override void Awake()
        {
            base.Awake();

            m_DefaultGunModelScale = m_ObjectToDisable.transform.localScale;
        }

        private void Update()
        {
            if (m_HideGun && Time.time > m_HideTime)
            {
                m_ObjectToDisable.transform.localScale = Vector3.zero;
                m_HideGun = false;
            }
        }
    }
}