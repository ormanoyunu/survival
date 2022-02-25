using UnityEngine;

namespace SurvivalTemplatePro.WorldManagement
{
    [RequireComponent(typeof(SphereCollider))]
    public class TemperatureEffector : MonoBehaviour
    {
        public float Radius
        {
            get => m_Radius;

            set
            {
                m_Radius = value;
                m_InfluenceVolume.radius = m_Radius;
            }
        }

        public float TemperatureStrength { get; set; }

        private float m_Radius;
        private SphereCollider m_InfluenceVolume;


        private void Awake()
        {
            m_InfluenceVolume = GetComponent<SphereCollider>();
            m_InfluenceVolume.isTrigger = true;
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.TryGetComponent(out Character character)) 
            {
                if (character.TryGetModule(out ITemperatureManager temperatureManager))
                {
                    float distanceToEntity = Vector3.Distance(transform.position, other.transform.position);
                    float tempFactor = 1f - distanceToEntity / m_Radius;
                    float tempChange = TemperatureStrength * tempFactor * Time.deltaTime;

                    temperatureManager.Temperature = tempChange;
                }
            }
        }
    }
}