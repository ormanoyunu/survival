using System;
using UnityEngine;
using UnityEngine.Events;

namespace SurvivalTemplatePro
{
    [RequireComponent(typeof(Collider))]
    public class TriggerEventHandler : MonoBehaviour
    {
        public event Action<Collider> TriggerEnter;
        public event Action<Collider> TriggerStay;
        public event Action<Collider> TriggerExit;

        [SerializeField]
        private UnityEvent m_TriggerEnter;

        [SerializeField]
        private UnityEvent m_TriggerStay;

        [SerializeField]
        private UnityEvent m_TriggerExit;


        private void OnTriggerEnter(Collider other)
        {
            m_TriggerEnter?.Invoke();
            TriggerEnter?.Invoke(other);
        }

        private void OnTriggerStay(Collider other)
        {
            m_TriggerStay?.Invoke();
            TriggerStay?.Invoke(other);
        }

        private void OnTriggerExit(Collider other)
        {
            m_TriggerExit?.Invoke();
            TriggerExit?.Invoke(other);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            GetComponent<Collider>().isTrigger = true;
        }
#endif
    }
}