using UnityEngine;

namespace SurvivalTemplatePro.Gameplay
{
    [ExecuteInEditMode]
    public class MoveToSpawnPoint : MonoBehaviour
    {
        public StandardPlayerSpawner Spawner => m_Spawner;
        public Vector3 Offset => m_Offset;

        [SerializeField]
        private Vector3 m_Offset;

        [SerializeField, HideInInspector]
        private StandardPlayerSpawner m_Spawner;


#if UNITY_EDITOR
        private void OnValidate()
        {
            if (m_Spawner == null)
                m_Spawner = FindObjectOfType<StandardPlayerSpawner>();
        }

        public void MoveToRandomPoint()
        {
            if (m_Spawner != null)
            {
                UnityEditor.Undo.RecordObject(transform, "MoveSpawnPoint");

                var spawnPoint = m_Spawner.GetPlayerSpawnPoint();
                transform.position = spawnPoint.Position + m_Offset;
                transform.rotation = spawnPoint.Rotation;
            }
        }
#endif
    }
}