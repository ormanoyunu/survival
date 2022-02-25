using UnityEngine;

namespace SurvivalTemplatePro.UISystem
{
    public class TutorialMessage : MonoBehaviour
    {
        [SerializeField]
        private float m_TransitionSpeed = 1f;

        [SerializeField]
        private float m_MaxDistance;

        [SerializeField, HideInInspector]
        private TextMesh[] m_TextMesh;

        private Color[] m_TextMeshColors;


        private void Start()
        {
            if (m_TextMesh.Length == 0) 
                m_TextMesh = GetComponentsInChildren<TextMesh>();

            m_TextMeshColors = new Color[m_TextMesh.Length];

            for (int i = 0; i < m_TextMesh.Length; i++)
                m_TextMeshColors[i] = m_TextMesh[i].color;
        }

        private void LateUpdate()
        {
            var mainCamera = Camera.main;

            if (mainCamera == null)
                return;

            float angle = Vector3.Angle(gameObject.transform.forward, Camera.main.transform.forward);
            float distance = Vector3.Distance(mainCamera.transform.position, transform.position);
            ChangeTextMeshColors(angle < 90 && distance < m_MaxDistance);
        }

        private void ChangeTextMeshColors(bool enable)
        {
            if (enable)
            {
                for (int i = 0; i < m_TextMesh.Length; i++)
                {
                    m_TextMesh[i].color = Color.Lerp(m_TextMesh[i].color, m_TextMeshColors[i], Time.deltaTime * m_TransitionSpeed);
                }
            }
            else
            {
                Color transparentColor = Color.black;
                transparentColor.a = 0;

                for (int i = 0; i < m_TextMesh.Length; i++)
                {
                    m_TextMesh[i].color = Color.Lerp(m_TextMesh[i].color, transparentColor, Time.deltaTime * m_TransitionSpeed);
                }
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (m_TextMesh.Length == 0)
                m_TextMesh = GetComponentsInChildren<TextMesh>();
        }
#endif
    }
}
