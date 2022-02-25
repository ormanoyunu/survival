using UnityEngine;

namespace SurvivalTemplatePro
{
    public class ChildOfConstraint : MonoBehaviour
    {
        public Transform Parent { get => m_Parent; set => m_Parent = value; }

        [SerializeField]
        private Transform m_Parent;

        [SerializeField]
        private bool m_CustomOffset = true;

        [SerializeField, EnableIf("m_CustomOffset", true)]
        private Vector3 m_PositionOffset;

        [SerializeField, EnableIf("m_CustomOffset", true)]
        private Vector3 m_RotationOffset;


        private void Awake()
        {
            if (!m_CustomOffset)
            {
                m_PositionOffset = m_Parent.InverseTransformPoint(transform.position);
                m_RotationOffset = (Quaternion.Inverse(m_Parent.rotation) * transform.rotation).eulerAngles;
            }
        }

        private void LateUpdate()
        {
            if (m_Parent != null)
            {
                transform.position = m_Parent.position + m_Parent.TransformVector(m_PositionOffset);
                transform.rotation = m_Parent.rotation * Quaternion.Euler(m_RotationOffset);
            }
        }
    }
}