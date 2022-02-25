using SurvivalTemplatePro.CameraSystem;
using SurvivalTemplatePro.WorldManagement;
using UnityEngine;

namespace SurvivalTemplatePro
{
    [RequireComponent(typeof(ISleepHandler))]
    public class CharacterSleepEffects : CharacterBehaviour
    {
        [BHeader("Sleep Stats Change")]

        [SerializeField, Range(0, 100f)]
        private float m_HealthIncreasePerHour;

        [SerializeField, Range(0, 100f)]
        private float m_EnergyIncreasePerHour;

        [BHeader("Camera Settings")]

        [SerializeField]
        private CameraMotionState m_SleepMotionState;

        [SerializeField]
        private CameraEffectSettings m_SleepEffects;

        [BHeader("Audio Settings")]

        [SerializeField]
        private DelayedSound[] m_SleepingSounds;

        [SerializeField]
        private DelayedSound[] m_GetUpSounds;

        private ICameraMotionHandler m_CameraMotionModule;
        private ICameraEffectsHandler m_CameraEffectsModule;

        private ISleepHandler m_SleepHandler;
        private IEnergyManager m_EnergyManager;


        public override void OnInitialized()
        {
            GetModule(out m_CameraMotionModule);
            GetModule(out m_CameraEffectsModule);
            GetModule(out m_EnergyManager);

            m_SleepHandler = GetComponent<ISleepHandler>();

            m_SleepHandler.onSleepStart += OnSleepStart;
            m_SleepHandler.onSleepEnd += OnSleepEnd;
        }

        private void OnSleepStart(GameTime timeToSleep) 
        {
            m_CameraMotionModule.SetCustomState(m_SleepMotionState);
            m_CameraEffectsModule.DoAnimationEffect(m_SleepEffects);

            Character.AudioPlayer.PlaySounds(m_SleepingSounds);
        }

        private void OnSleepEnd(GameTime timeSlept)
        {
            m_CameraMotionModule.ClearCustomState();

            int hoursSlept = timeSlept.Hours;

            if (hoursSlept > 0)
            {
                Character.HealthManager.RestoreHealth(hoursSlept * m_HealthIncreasePerHour);
                m_EnergyManager.Energy += hoursSlept * m_EnergyIncreasePerHour;
            }

            Character.AudioPlayer.PlaySounds(m_GetUpSounds);
        }
    }
}