﻿using UnityEngine;

namespace SurvivalTemplatePro
{
    public class PlayerLookFollow : CharacterBehaviour
    {
        [SerializeField]
        private Transform m_Parent;

        [SerializeField]
        private bool m_CustomOffset;

        [SerializeField, EnableIf("m_CustomOffset", true)]
        private Vector3 m_PositionOffset;

        [SerializeField, EnableIf("m_CustomOffset", true)]
        private Vector3 m_RotationOffset;


        public override void OnInitialized()
        {
            if (m_Parent == null)
            {
                Debug.LogError("No parent!");
                return;
            }

            if (!m_CustomOffset)
            {
                m_PositionOffset = m_Parent.InverseTransformPoint(transform.position);
                m_RotationOffset = (Quaternion.Inverse(m_Parent.rotation) * transform.rotation).eulerAngles;
            }

            Character.GetModule<ILookHandler>().onPostViewUpdate += UpdatePosition;
        }

        private void UpdatePosition()
        {
            transform.position = m_Parent.position + m_Parent.TransformVector(m_PositionOffset);
            transform.rotation = m_Parent.rotation * Quaternion.Euler(m_RotationOffset);
        }
    }
}