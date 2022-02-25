using UnityEngine;
using UnityEngine.Events;

namespace SurvivalTemplatePro
{
    [RequireComponent(typeof(IHealthManager))]
    public abstract class CharacterVitalModule : MonoBehaviour
    {
        #region Internal
        [System.Serializable]
        protected class StatDepleteSettings
        {
            [Range(0f, 1000f)]
            public float InitialValue = 100f;

            [Range(0f, 1000f)]
            public float InitialMaxValue = 100f;

            [Range(0f, 100f)]
            public float DepletionSpeed = 3f;

            [Range(0f, 100f)]
            public float DmgWhenDepleted = 3f;

            [Range(0f, 100f)]
            public float HealthRestore = 3f;

            [Space]

            public UnityEvent m_OnDepleted;
        }
        #endregion

        [SerializeField]
        protected StatDepleteSettings m_StatSettings;

        protected IHealthManager m_HealthManager;


        protected virtual void Awake()
        {
            if (!TryGetComponent(out m_HealthManager))
            {
                Debug.LogError($"This component requires a component that implements the 'IVitalsManager' module to function, Disabling... ");
                enabled = false;
            }
        }

        protected void InitalizeStat(ref float statValue, ref float maxStatValue)
        {
            statValue = Mathf.Clamp(m_StatSettings.InitialValue, 0, m_StatSettings.InitialMaxValue);
            maxStatValue = m_StatSettings.InitialMaxValue;
        }

        protected void DepleteStat(ref float statValue, float maxStatValue) 
        {
            float deltaTime = Time.deltaTime;
            float depletion = m_StatSettings.DepletionSpeed * deltaTime;

            statValue = Mathf.Clamp(statValue - depletion, 0, maxStatValue);

            if (statValue < 0.001f)
                m_HealthManager.ReceiveDamage(m_StatSettings.DmgWhenDepleted * deltaTime);

            if (statValue > maxStatValue - (statValue / 10f))
                m_HealthManager.RestoreHealth(m_StatSettings.HealthRestore * deltaTime);
        }
    }
}