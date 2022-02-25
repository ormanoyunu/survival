using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SurvivalTemplatePro.WieldableSystem
{
    public class Polearm : MeleeWeapon, IAimHandler
    {
        public bool IsAiming => m_Aimer != null && m_Aimer.IsAiming;

        public event UnityAction<bool> onAim;
        
        [BHeader("Polearm")]

        [SerializeField]
        private MeleeAimerBehaviour m_Aimer;

        private float m_LastAimToggleTime;


        public void StartAiming()
        {
            if (m_Aimer == null)
                return;

            if (CanAim() && m_Aimer.TryStartAim())
            {
                m_LastAimToggleTime = Time.time;
                onAim?.Invoke(true);
            }
        }

        public void EndAiming()
        {
            if (m_Aimer == null)
                return;

            if (m_Aimer.TryEndAim())
            {
                m_LastAimToggleTime = Time.time;
                onAim?.Invoke(false);
            }
        }

        protected virtual bool CanAim() 
        {
            bool canAim = !m_IsAimActionBlocked && 
                          Time.time > LastEquipOrHolsterTime + HolsterDuration + 0.1f &&
                          Time.time > m_NextPossibleSwingTime && 
                          (ItemDurability == null || ItemDurability.Float > 0f);

            return canAim;
        }

        protected override bool CanUse(UsePhase usePhase)
        {
            return m_LastAimToggleTime + 0.3f < Time.time && base.CanUse(usePhase);
        }

        #region Action Blocking

        private bool m_IsAimActionBlocked = false;
        private readonly List<Object> m_AimBlockers = new List<Object>();

        public void RegisterAimBlocker(Object blocker)
        {
            if (m_AimBlockers.Contains(blocker))
                return;

            m_AimBlockers.Add(blocker);
            m_IsAimActionBlocked = m_AimBlockers.Count > 0;

            if (m_IsAimActionBlocked && IsAiming)
                EndAiming();
        }

        public void UnregisterAimBlocker(Object blocker)
        {
            if (!m_AimBlockers.Contains(blocker))
                return;

            m_AimBlockers.Remove(blocker);
            m_IsAimActionBlocked = m_AimBlockers.Count > 0;
        }

        #endregion
    }
}