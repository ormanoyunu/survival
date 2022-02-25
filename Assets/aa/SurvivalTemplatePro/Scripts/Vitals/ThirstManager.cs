using UnityEngine;
using UnityEngine.Events;

namespace SurvivalTemplatePro
{
    public class ThirstManager : CharacterVitalModule, IThirstManager
    {
        public bool HasMaxThirst => m_MaxThirst - m_Thirst < 0.01f;

        public float Thirst
        {
            get => m_Thirst;
            set
            {
                float clampedValue = Mathf.Clamp(value, 0f, m_MaxThirst);

                if (value != m_Thirst && clampedValue != m_Thirst)
                {
                    m_Thirst = clampedValue;
                    onThirstChanged?.Invoke(clampedValue);
                }
            }
        }

        public float MaxThirst
        {
            get => m_MaxThirst;
            set 
            {
                float clampedValue = Mathf.Max(value, 0f);

                if (value != m_MaxThirst && clampedValue != m_MaxThirst)
                {
                    m_MaxThirst = clampedValue;
                    onMaxThirstChanged?.Invoke(clampedValue);

                    Thirst = Mathf.Clamp(Thirst, 0f, m_MaxThirst);
                }
            }
        }

        public event UnityAction<float> onThirstChanged;
        public event UnityAction<float> onMaxThirstChanged;

        private float m_Thirst;
        private float m_MaxThirst;


        protected override void Awake()
        {
            base.Awake();

            InitalizeStat(ref m_Thirst, ref m_MaxThirst);
        }

        private void Update()
        {
            if (m_HealthManager.IsAlive)
                DepleteStat(ref m_Thirst, m_MaxThirst);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (!Application.isPlaying)
                return;

            MaxThirst = m_StatSettings.InitialMaxValue;
            Thirst = m_StatSettings.InitialValue;
        }
#endif
    }
}