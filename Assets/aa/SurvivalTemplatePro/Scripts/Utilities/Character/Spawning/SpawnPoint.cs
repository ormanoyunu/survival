using UnityEngine;

namespace SurvivalTemplatePro.Gameplay
{
    public class SpawnPoint : MonoBehaviour
    {
        public virtual SpawnPointInfo GetSpawnPoint()
        {
            float yAngle = Random.Range(0f, 360f);
            Quaternion rotation = Quaternion.Euler(0f, yAngle, 0f);

            return new SpawnPointInfo(transform.position, rotation);
        }

        [SerializeField]
        private bool m_AlwaysDrawGizmo;


#if UNITY_EDITOR
        public void SnapToGround() 
        {
            // Snaps the spawn point position to the ground.
            if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out RaycastHit hitInfo, 10f))
                transform.position = hitInfo.point + Vector3.up * 0.1f;
            else if (Physics.Raycast(transform.position + Vector3.up * 3f, Vector3.down, out hitInfo, 10f))
                transform.position = hitInfo.point + Vector3.up * 0.1f;
        }

        private void OnDrawGizmos()
        {
            if (!m_AlwaysDrawGizmo)
                return;

            DrawGizmo();
        }

        private void OnDrawGizmosSelected()
        {
            if (m_AlwaysDrawGizmo)
                return;

            DrawGizmo();
        }

        private void DrawGizmo() 
        {
            var prevColor = Gizmos.color;
            Gizmos.color = Color.green;

            float gizmoWidth = 0.5f;
            float gizmoHeight = 1.8f;

            Gizmos.DrawCube(new Vector3(transform.position.x, transform.position.y + gizmoHeight / 2, transform.position.z), new Vector3(gizmoWidth, gizmoHeight, gizmoWidth));

            Gizmos.color = prevColor;
        }
#endif
    }
}