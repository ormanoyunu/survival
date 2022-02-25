using UnityEngine;

namespace SurvivalTemplatePro
{
    public class PoolableObject : MonoBehaviour
    {
        public string PoolId { get => m_PoolId; }

        private bool m_Initialized;
        private string m_PoolId;


        public void Init(string poolId)
        {
            if (m_Initialized)
            {
                Debug.LogError("You are attempting to initialize a poolable object, but it's already initialized!!");
                return;
            }

            m_PoolId = poolId;
            m_Initialized = true;
        }

        public virtual void OnUse() { }
        public virtual void OnReleased() { }
    }
}