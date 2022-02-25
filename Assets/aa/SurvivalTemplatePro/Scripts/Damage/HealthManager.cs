using UnityEngine;
using UnityEngine.Events;

namespace SurvivalTemplatePro
{
    /// <summary>
    /// Manages the parent character's health and death
    /// </summary>
    public class HealthManager : CharacterBehaviour, IHealthManager
    {
        public bool IsAlive => !m_IsDead;

        public float Health
        {
            get => m_CurrentHealth;
            set
            {
                float clampedValue = Mathf.Clamp(value, 0f, m_MaxHealth);

                if (value != m_CurrentHealth && clampedValue != m_CurrentHealth)
                {
                    m_PrevHealth = m_CurrentHealth;
                    m_CurrentHealth = clampedValue;
                    onHealthChanged?.Invoke(clampedValue);

                    // Raise respawn event
                    if (m_IsDead && m_CurrentHealth > m_PrevHealth)
                    {
                        m_IsDead = false;
                        onRespawn?.Invoke();
                    }
                    // Raise death event
                    else if (!m_IsDead && m_CurrentHealth < 0.01f)
                    {
                        m_IsDead = true;
                        onDeath?.Invoke();
                    }             
                }
            }
        }

        public float PrevHealth => m_PrevHealth;

        public float MaxHealth
        {
            get => m_MaxHealth;
            set
            {
                float clampedValue = Mathf.Max(value, 0f);

                if (value != m_MaxHealth && clampedValue != m_MaxHealth)
                {
                    m_MaxHealth = clampedValue;
                    onMaxHealthChanged?.Invoke(clampedValue);

                    Health = Mathf.Clamp(Health, 0f, m_MaxHealth);
                }
            }
        }

        public event UnityAction<DamageInfo> onDamageTaken;
        public event UnityAction<float> onHealthRestored;
        public event UnityAction<float> onHealthChanged;
        public event UnityAction<float> onMaxHealthChanged;
        public event UnityAction onDeath;
        public event UnityAction onRespawn;

        [SerializeField, Range(0, 1000)]
        [Tooltip("The starting health of this character (can't be higher than the max).")]
        private float m_StartingHealth = 100f;

        [SerializeField, Range(0, 1000)]
        [Tooltip("The starting highest health of this character (can be modified at runtime).")]
        private float m_StartingMaxHealth = 100f;

        private float m_CurrentHealth;
        private float m_PrevHealth;
        private float m_MaxHealth;
        private bool m_IsDead;

        //bu script bütün healt olayları
        public override void OnInitialized()
        {
            m_MaxHealth = m_StartingMaxHealth;
            m_CurrentHealth = Mathf.Clamp(m_StartingHealth, 0f, m_MaxHealth);
        }

        public virtual void RestoreHealth(float healthRestore)
        {
            float newHealthValue = Mathf.Min(m_CurrentHealth + Mathf.Abs(healthRestore), m_MaxHealth);

            if (newHealthValue > m_CurrentHealth)
            {
                Health = newHealthValue;
                onHealthRestored?.Invoke(healthRestore);
            }
        }

        //hasarı güncellendiği hasarın alındığı ana mekan
        public virtual void ReceiveDamage(DamageInfo damageInfo)
        {
            Debug.Log("burada karakter hasar alınca olan" + damageInfo.DamageType);
            float newHealthValue = Mathf.Max(m_CurrentHealth - damageInfo.Damage, 0f);

            if (newHealthValue < m_CurrentHealth)
            {
                Health = newHealthValue;
                onDamageTaken?.Invoke(damageInfo);
            }
        }

        public void ReceiveDamage(float damage)
        {
            Debug.Log("burada karakter hasar alınca olan2");
            float newHealthValue = Mathf.Max(m_CurrentHealth - damage, 0f);

            if (newHealthValue < m_CurrentHealth)
                Health = newHealthValue;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (!Application.isPlaying)
                return;

            MaxHealth = m_StartingMaxHealth;
            Health = m_StartingHealth;
        }
#endif
    }
}