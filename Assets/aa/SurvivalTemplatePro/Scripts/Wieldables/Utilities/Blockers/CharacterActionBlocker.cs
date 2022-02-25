using System.Collections;
using UnityEngine;

namespace SurvivalTemplatePro.WieldableSystem
{
    [RequireComponent(typeof(Wieldable))]
    public abstract class CharacterActionBlocker : CharacterBehaviour
    {
        [SerializeField]
        [InfoBox("How fast should this behaviour be updated, set it to a lower value for more consistency and higher for better perfomance."), Range(0.01f, 10f)]
        private float m_TickRate = 0.15f;

        [Space]

        [SerializeField]
        [InfoBox("For how much time should the corresponding action be put to 'sleep' (unable to start) after it's been blocked."), Range(0.01f, 10f)]
        private float m_Cooldown = 0.35f;

        private float m_NextTimeCanDoAction;
        private bool m_IsBlocked;

        private WaitForSeconds m_UpdateWait;


        private void OnEnable() => StartCoroutine(C_Update());
        private void OnDisable() => StopAllCoroutines();

        private IEnumerator C_Update() 
        {
            m_UpdateWait = new WaitForSeconds(m_TickRate);

            while (true)
            {
                if (m_IsBlocked && Time.time < m_NextTimeCanDoAction)
                {
                    yield return null;
                    continue;
                }
                else
                    yield return m_UpdateWait;

                bool isValid = IsActionValid();

                if (!isValid)
                    m_NextTimeCanDoAction = Time.time + m_Cooldown;

                if (!m_IsBlocked && !isValid)
                {
                    m_IsBlocked = true;
                    BlockAction();
                }
                else if (m_IsBlocked && isValid)
                {
                    m_IsBlocked = false;
                    UnblockAction();
                }
            }
        }

        protected abstract bool IsActionValid();
        protected abstract void BlockAction();
        protected abstract void UnblockAction();

#if UNITY_EDITOR
        private void OnValidate()
        {
            m_UpdateWait = new WaitForSeconds(m_TickRate);
        }
#endif
    }
}