using SurvivalTemplatePro.UISystem;
using SurvivalTemplatePro.WorldManagement;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace SurvivalTemplatePro
{
    public class CharacterSleepHandler : CharacterBehaviour, ISleepHandler
    {
        public Vector3 LastSleepPosition => m_LastSleepPosition;
        public Quaternion LastSleepRotation => m_LastSleepRotation;

        public bool SleepActive => m_SleepActive;

        public event UnityAction<GameTime> onSleepStart;
        public event UnityAction<GameTime> onSleepEnd;

        [BHeader("Sleep Settings")]

        [SerializeField, Range(0f, 10f)]
        [Tooltip("How much time it takes to transition to sleeping (e.g. moving to bed).")]
        private float m_GoToSleepDuration = 2f;

        [SerializeField, Range(0f, 10f)]
        [Tooltip("How much time it takes to fall asleep.")]
        private float m_SleepDelay = 1.5f;

        [SerializeField, Range(0, 60)]
        [Tooltip("Sleep duration in seconds")]
        private float m_SleepDuration;

        [SerializeField, Range(0f, 10f)]
        [Tooltip("How much time to wait after the sleep is done, before getting up.")]
        private float m_WakeUpDelay = 1.5f;

        [SerializeField, Range(0f, 10f)]
        [Tooltip("How much time it takes to transition from sleeping to standing up.")]
        private float m_WakeUpDuration = 2f;

        [Space]

        [SerializeField, Range(0, 24)]
        [Tooltip("Max hours that can pass while sleeping")]
        private int m_HoursToSleep = 8;

        [SerializeField, Range(0, 24)]
        [Tooltip("Max hour this character can wake up at, we don't want to be lazy :)")]
        private int m_MaxGetUpHour = 8;

        [BHeader("Sleep Conditions")]

        [SerializeField]
        [Tooltip("If enabled, this character will not be allowed to sleep during the day.")]
        private bool m_OnlySleepAtNight = true;

        [Space]

        [SerializeField]
        [Tooltip("Check for enemies before sleeping, if any of the are found, this character will be unable to sleep.")]
        private bool m_CheckForEnemies = false;

        [SerializeField, EnableIf("m_CheckForEnemies", true)]
        [Tooltip("The enemy check radius.")]
        private float m_CheckForEnemiesRadius;

        [SerializeField, EnableIf("m_CheckForEnemies", true)]
        [Tooltip("The enemy layer, any object with this layer will be considered an enemy.")]
        private LayerMask m_EnemiesLayerMask;

        [BHeader("Misc")]

        [SerializeField]
        [Tooltip("Sound that will be played when attempting to sleep unsuccessfully.")]
        private StandardSound m_CantSleepSound;

        [Space]

        [SerializeField]
        private UnityEvent m_OnSleepStart;

        [SerializeField]
        private UnityEvent m_OnSleepEnd;

        private IPauseHandler m_PauseHandler;
        private IWieldablesController m_WieldableController;
        private ISleepModule m_SleepModule;

        private bool m_SleepActive;
        private ISleepingPlace m_SleepingPlace;

        private Vector3 m_LastSleepPosition = Vector3.zero;
        private Quaternion m_LastSleepRotation = Quaternion.identity;


        public override void OnInitialized()
        {
            GetModule(out m_PauseHandler);
            GetModule(out m_WieldableController);
            GetModule(out m_SleepModule);
        }

        public void Sleep(ISleepingPlace sleepingPlace)
        {
            if (m_SleepActive) return;

            if (m_OnlySleepAtNight && WorldManagerBase.Instance.GetTimeOfDay() == TimeOfDay.Day)
            {
                MessageDisplayerUI.PushMessage("You can only sleep at night!", Color.red);
                Character.AudioPlayer.PlaySound(m_CantSleepSound);
                return;
            }

            if (m_CheckForEnemies && HasEnemiesNearby(sleepingPlace.gameObject.transform))
            {
                MessageDisplayerUI.PushMessage("Can't sleep with enemies nearby!", Color.red);
                Character.AudioPlayer.PlaySound(m_CantSleepSound);
                return;
            }

            m_SleepingPlace = sleepingPlace;
            StartCoroutine(C_Sleep());
        }

        private bool HasEnemiesNearby(Transform checkPoint) 
        {
            foreach (var collider in Physics.OverlapSphere(checkPoint.position, m_CheckForEnemiesRadius, m_EnemiesLayerMask, QueryTriggerInteraction.Ignore))
            {
                if (!Character.HasCollider(collider))
                {
                    if (collider.GetComponent<Hitbox>() != null)
                        return true;
                }
            }

            return false;
        }

        private IEnumerator C_Sleep()
        {
            m_OnSleepStart?.Invoke();

            // Pause the player
            m_PauseHandler.RegisterLocker(this, PlayerPauseParams.Default);

            // Holster active wiedable
            m_WieldableController.TryEquipWieldable(null, null, 1.5f);

            // Do the sleep effects
            m_SleepModule.DoSleepEffects(m_SleepingPlace, m_GoToSleepDuration);

            yield return new WaitForSeconds(m_GoToSleepDuration + m_SleepDelay);

            int hoursToSleep = GetHoursToSleep();
            GameTime timeToSleep = new GameTime(hoursToSleep, 0f, 0f);
            WorldManagerBase.Instance.PassTime(hoursToSleep / 24f, m_SleepDuration);

            m_SleepActive = true;
            onSleepStart?.Invoke(timeToSleep);

            float passedTime = 0f;
            while (passedTime <= m_SleepDuration)
            {
                passedTime += Time.deltaTime;
                yield return null;
            }

            m_LastSleepPosition = Character.transform.position;

            m_SleepActive = false;
            onSleepEnd?.Invoke(timeToSleep);

            yield return new WaitForSeconds(m_WakeUpDelay);

            // Do the wake up effects
            m_SleepModule.DoWakeUpEffects(m_WakeUpDuration);

            yield return new WaitForSeconds(m_WakeUpDuration);

            // Unpause the player
            m_PauseHandler.UnregisterLocker(this);

            m_OnSleepEnd?.Invoke();
        }

        private int GetHoursToSleep() 
        {
            int hoursToSleep;
            int currentHour = (int)(WorldManagerBase.Instance.GetNormalizedTime() * 24);

            if (currentHour <= 24 && currentHour > 12)
                hoursToSleep = 24 - currentHour + m_MaxGetUpHour;
            else if (currentHour < 12 && currentHour <= m_MaxGetUpHour)
                hoursToSleep = m_MaxGetUpHour - currentHour;
            else
                hoursToSleep = m_HoursToSleep;

            hoursToSleep = Mathf.Clamp(hoursToSleep, 0, m_HoursToSleep);

            return hoursToSleep;
        }
    }
}