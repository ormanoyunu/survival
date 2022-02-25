using UnityEngine;

namespace SurvivalTemplatePro.WieldableSystem
{
    public class WieldableEffectsToggler : MonoBehaviour
    {
        [SerializeField]
        private LightEffect[] m_LightEffects;

        [SerializeField]
        private AudioEffect[] m_AudioEffects;

        [SerializeField]
        private ParticleSystem[] m_Particles;


        public void EnableEffects() 
        {
            for (int i = 0; i < m_LightEffects.Length; i++)
                m_LightEffects[i].Play(true);

            for (int i = 0; i < m_LightEffects.Length; i++)
                m_AudioEffects[i].Play();

            for (int i = 0; i < m_LightEffects.Length; i++)
                m_Particles[i].Play(true);
        }

        public void DisableEffects() 
        {
            for (int i = 0; i < m_LightEffects.Length; i++)
                m_LightEffects[i].Stop(true);

            for (int i = 0; i < m_LightEffects.Length; i++)
                m_AudioEffects[i].Stop();

            for (int i = 0; i < m_LightEffects.Length; i++)
                m_Particles[i].Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
    }
}