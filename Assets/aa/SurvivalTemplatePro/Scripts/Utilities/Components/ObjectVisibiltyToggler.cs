using UnityEngine;

namespace SurvivalTemplatePro
{
    public class ObjectVisibiltyToggler : MonoBehaviour
    {
        [SerializeField]
        private Behaviour[] m_BehavioursToToggle;

        [SerializeField]
        private Collider[] m_CollidersToToggle;

        [SerializeField]
        private GameObject[] m_GameObjectsToToggle;


        public void EnableObjects()
        {
            foreach (var behaviour in m_BehavioursToToggle)
                behaviour.enabled = true;

            foreach (var col in m_CollidersToToggle)
                col.enabled = true;

            foreach (var gameObj in m_GameObjectsToToggle)
                gameObj.SetActive(true);
        }

        public void DisableObjects() 
        {
            foreach (var behaviour in m_BehavioursToToggle)
                behaviour.enabled = false;

            foreach (var col in m_CollidersToToggle)
                col.enabled = false;

            foreach (var gameObj in m_GameObjectsToToggle)
                gameObj.SetActive(false);
        }
    }
}