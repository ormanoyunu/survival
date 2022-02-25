using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SurvivalTemplatePro.WieldableSystem
{
    [RequireComponent(typeof(WieldableEffectsHandler))]
    public class MeleeWeapon : Wieldable, IUseHandler, IStaminaDepleter
    {
        #region Internal
        [System.Serializable]
        private class SwingCombo
        {
            public string ComboName;
            public SelectionType SelectionType;
            public MeleeSwingBehaviour[] Swings;

            [System.NonSerialized, HideInInspector]
            public int LastSelected = -1;
        }
        #endregion

        public event UnityAction onUse;
        public event UnityAction<float> onDepleteStamina;

        [BHeader("Melee Weapon")]

        [SerializeField]
        private bool m_SwingContinuosly = true;

        [SerializeField]
        private SwingCombo[] m_Swings;

        protected float m_NextPossibleSwingTime;
        protected IMeleeSwing m_LastMeleeSwing;


        public void Use(UsePhase usePhase)
        {
            if (!CanUse(usePhase))
                return;

            int comboIndex = GetSwingWithHighestPriority();
            SwingCombo swingCombo = m_Swings[comboIndex];

            m_LastMeleeSwing = swingCombo.Swings.Select(ref swingCombo.LastSelected, swingCombo.SelectionType);
            m_LastMeleeSwing.DoSwing(Character);
            m_NextPossibleSwingTime = Time.time + m_LastMeleeSwing.SwingDuration;

            onUse?.Invoke();

            float staminaToDeplete = m_LastMeleeSwing.AttackEffort;
            onDepleteStamina?.Invoke(staminaToDeplete);
        }

        private int GetSwingWithHighestPriority()
        {
            int swingComboIndex = 0;

            for (int i = 0; i < m_Swings.Length; i++)
            {
                var firstSwing = m_Swings[i].Swings[0];

                if (firstSwing != null && firstSwing.CanSwing())
                {
                    swingComboIndex = i;
                    break;
                }
            }

            return Mathf.Clamp(swingComboIndex, 0, m_Swings.Length - 1);
        }

        protected virtual bool CanUse(UsePhase usePhase)
        {
            bool canUse = !m_IsUseActionBlocked &&
                          usePhase != UsePhase.End &&
                          Time.time > m_NextPossibleSwingTime &&
                          Time.time > (LastEquipOrHolsterTime + HolsterDuration + 0.1f) &&
                          (m_SwingContinuosly || usePhase != UsePhase.Hold) &&
                          (ItemDurability == null || ItemDurability.Float > 0f);

            return canUse;
        }

        #region Action Blocking

        private bool m_IsUseActionBlocked = false;
        private readonly List<Object> m_UseBlockers = new List<Object>();

        public void RegisterUseBlocker(Object blocker)
        {
            if (m_UseBlockers.Contains(blocker))
                return;

            m_UseBlockers.Add(blocker);
            m_IsUseActionBlocked = m_UseBlockers.Count > 0;
        }

        public void UnregisterUseBlocker(Object blocker)
        {
            if (!m_UseBlockers.Contains(blocker))
                return;

            m_UseBlockers.Remove(blocker);
            m_IsUseActionBlocked = m_UseBlockers.Count > 0;
        }

        #endregion
    }
}