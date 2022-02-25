using System.Collections;
using UnityEngine;

namespace SurvivalTemplatePro
{
    public class LerpSleepModule : MonoBehaviour, ISleepModule
    {
        [SerializeField]
        private Transform m_Camera;

        [SerializeField]
        private Easings.Function m_EasingFunction = Easings.Function.QuadraticEaseInOut;

        private Vector3 m_BeforeSleepPosition;
        private Quaternion m_BeforeSleepRotation;
        private Transform m_OriginalCameraParent;

        private Easer m_Easer;


        public void DoSleepEffects(ISleepingPlace sleepingPlace, float duration)
        {
            m_BeforeSleepRotation = m_Camera.rotation;
            m_BeforeSleepPosition = m_Camera.position;

            StopAllCoroutines();
            StartCoroutine(C_RotateAndPositionTransform(sleepingPlace.SleepPosition, sleepingPlace.SleepRotation, true, duration));
        }

        public void DoWakeUpEffects(float duration)
        {
            StopAllCoroutines();
            StartCoroutine(C_RotateAndPositionTransform(m_BeforeSleepPosition, m_BeforeSleepRotation, false, duration));
        }

        private void Start()
        {
            m_Easer = new Easer(m_EasingFunction, 1f);
            m_OriginalCameraParent = m_Camera.transform.parent;
        }

        private IEnumerator C_RotateAndPositionTransform(Vector3 targetPosition, Quaternion targetRotation, bool sleep, float duration)
        {
            Quaternion startRotation = m_Camera.rotation;
            Vector3 startPosition = m_Camera.position;

            transform.rotation = startRotation;
            transform.position = startPosition;

            if (sleep)
                m_Camera.transform.parent = transform;

            m_Easer.Duration = duration;
            m_Easer.Reset();

            while (m_Easer.InterpolatedValue < 1f)
            {
                m_Easer.Update(Time.deltaTime);

                transform.rotation = Quaternion.Slerp(startRotation, targetRotation, m_Easer.InterpolatedValue);
                transform.position = Vector3.Lerp(startPosition, targetPosition, m_Easer.InterpolatedValue);

                yield return null;
            }

            transform.rotation = targetRotation;
            transform.position = targetPosition;

            if (!sleep)
                m_Camera.transform.parent = m_OriginalCameraParent;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (m_Easer != null)
                m_Easer.Function = m_EasingFunction;
        }
#endif
    }
}