using UnityEngine;
using UnityEngine.Events;

namespace SurvivalTemplatePro
{
    [RequireComponent(typeof(Collider))]
    public class CollisionEventHandler : MonoBehaviour
    {
        [SerializeField]
        private UnityEvent m_CollisionEnter;

        [SerializeField]
        private UnityEvent m_CollisionStay;

        [SerializeField]
        private UnityEvent m_CollisionExit;


        private void OnCollisionEnter(Collision collision)
        {
            m_CollisionEnter?.Invoke();
        }

        private void OnCollisionStay(Collision collision)
        {
            m_CollisionStay?.Invoke();
        }

        private void OnCollisionExit(Collision collision)
        {
            m_CollisionExit?.Invoke();
        }
    }
}