using UnityEngine;
using UnityEngine.Events;

namespace SurvivalTemplatePro
{
    /// <summary>
    /// TODO: Implement
    /// </summary>
    public class TemperatureManager : MonoBehaviour, ITemperatureManager
    {
        public float Temperature
        {
            get => m_Hunger;
            set
            {
                float clampedValue = Mathf.Clamp(value, 0f, 1f);

                if (value != m_Hunger && clampedValue != m_Hunger)
                {
                    m_Hunger = clampedValue;
                    onTemperatureChanged?.Invoke(clampedValue);
                }
            }
        }

        public event UnityAction<float> onTemperatureChanged;

        private float m_Hunger;
    }
}