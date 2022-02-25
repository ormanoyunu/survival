using UnityEngine;
using UnityEngine.Events;

namespace SurvivalTemplatePro
{
    public class HungerManager : CharacterVitalModule, IHungerManager
    {
        public bool HasMaxHunger => m_MaxHunger - m_Hunger < 0.01f;

        public float Hunger
        {
            get => m_Hunger;
            set
            {
                float clampedValue = Mathf.Clamp(value, 0f, m_MaxHunger);

                if (value != m_Hunger && clampedValue != m_Hunger)
                {
                    m_Hunger = clampedValue;
                    onHungerChanged?.Invoke(clampedValue);
                }
            }
        }

        public float MaxHunger
        {
            get => m_MaxHunger;
            set
            {
                float clampedValue = Mathf.Max(value, 0f);

                if (value != m_MaxHunger && clampedValue != m_MaxHunger)
                {
                    m_MaxHunger = clampedValue;
                    onMaxHungerChanged?.Invoke(clampedValue);

                    Hunger = Mathf.Clamp(Hunger, 0f, m_MaxHunger);
                }
            }
        }

        public event UnityAction<float> onHungerChanged;
        public event UnityAction<float> onMaxHungerChanged;

        private float m_Hunger;
        private float m_MaxHunger;


        protected override void Awake()
        {
            base.Awake();

            InitalizeStat(ref m_Hunger, ref m_MaxHunger);
        }

        private void Update()
        {
            if (m_HealthManager.IsAlive)
                DepleteStat(ref m_Hunger, m_MaxHunger);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (!Application.isPlaying)
                return;

            MaxHunger = m_StatSettings.InitialMaxValue;
            Hunger = m_StatSettings.InitialValue;
        }
#endif
    }
}