using UnityEngine;
using UnityEngine.Events;

namespace SurvivalTemplatePro
{
    public class EnergyManager : CharacterVitalModule, IEnergyManager
    {
        public float Energy
        {
            get => m_Energy;
            set
            {
                float clampedValue = Mathf.Clamp(value, 0f, m_MaxEnergy);

                if (value != m_Energy && clampedValue != m_Energy)
                {
                    m_Energy = clampedValue;
                    onEnergyChanged?.Invoke(clampedValue);
                }
            }
        }

        public float MaxEnergy
        {
            get => m_MaxEnergy;
            set 
            {
                float clampedValue = Mathf.Max(value, 0f);

                if (value != m_MaxEnergy && clampedValue != m_MaxEnergy)
                {
                    m_MaxEnergy = clampedValue;
                    onMaxEnergyChanged?.Invoke(clampedValue);

                    Energy = Mathf.Clamp(Energy, 0f, m_MaxEnergy);
                }
            }
        }

        public event UnityAction<float> onEnergyChanged;
        public event UnityAction<float> onMaxEnergyChanged;

        private float m_Energy;
        private float m_MaxEnergy;


        protected override void Awake()
        {
            base.Awake();

            InitalizeStat(ref m_Energy, ref m_MaxEnergy);
        }

        private void Update()
        {
            if (m_HealthManager.IsAlive)
                DepleteStat(ref m_Energy, m_MaxEnergy);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (!Application.isPlaying)
                return;

            MaxEnergy = m_StatSettings.InitialMaxValue;
            Energy = m_StatSettings.InitialValue;
        }
#endif
    }
}