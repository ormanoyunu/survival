using UnityEngine;
using UnityEngine.UI;

namespace SurvivalTemplatePro.UISystem
{
    public class PlayerVitalsUI : PlayerUIBehaviour
    {
        [SerializeField]
        [Tooltip("The health bar image, the fill amount will be modified based on the current health value.")]
        private Image m_HealthBar;

        [Space]

        [SerializeField]
        [Tooltip("The energy bar image, the fill amount will be modified based on the current energy value.")]
        private Image m_EnergyBar;

        [SerializeField]
        [Tooltip("The thirst bar image, the fill amount will be modified based on the current thirst value.")]
        private Image m_ThirstBar;

        [SerializeField]
        [Tooltip("The hunger bar image, the fill amount will be modified based on the current hunger value.")]
        private Image m_HungerBar;

        private IHealthManager m_HealthManager;
        private IEnergyManager m_EnergyManager;
        private IHungerManager m_HungerManager;
        private IThirstManager m_ThirstManager;

        private float m_NextTimeToUpdate;
        private const float k_UpdateRate = 0.5f;


        public override void OnAttachment()
        {
            GetModule(out m_HealthManager);
            GetModule(out m_EnergyManager);
            GetModule(out m_HungerManager);
            GetModule(out m_ThirstManager);

            m_HealthManager.onHealthChanged += OnHealthChanged;
            OnHealthChanged(m_HealthManager.Health);
        }

        private void OnHealthChanged(float health)
        {
            if (m_HealthBar != null)
                m_HealthBar.fillAmount = health / 100f;
        }

        public override void OnInterfaceUpdate()
        {
            float currentTime = Time.time;

            if (currentTime < m_NextTimeToUpdate)
                return;

            if (m_EnergyBar != null)
                m_EnergyBar.fillAmount = m_EnergyManager.Energy / 100f;

            if (m_ThirstBar != null)
                m_ThirstBar.fillAmount = m_ThirstManager.Thirst / 100f;

            if (m_HungerBar != null)
                m_HungerBar.fillAmount = m_HungerManager.Hunger / 100f;

            m_NextTimeToUpdate = currentTime + k_UpdateRate;
        }
    }
}