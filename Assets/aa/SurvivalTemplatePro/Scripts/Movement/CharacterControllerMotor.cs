using UnityEngine;
using UnityEngine.Events;

namespace SurvivalTemplatePro.MovementSystem
{
    [RequireComponent(typeof(CharacterController))]
    public class CharacterControllerMotor : MonoBehaviour, ICharacterMotor
    {
        public bool IsGrounded => m_CharacterController.isGrounded;
        public Vector3 Velocity => m_CharacterController.velocity;
        public Vector3 GroundNormal => m_GroundNormal;
        public float SlopeLimit => m_CharacterController.slopeLimit;
        public float Height => m_CharacterController.height;
        public float Radius => m_CharacterController.radius;

        public event UnityAction<bool> onGroundedStateChanged;

        [SerializeField]
        private CharacterController m_CharacterController;

        [SerializeField, Range(0f, 10f)]
        private float m_PushForce = 1f;

        private Vector3 m_GroundNormal;


        public void SetHeight(float height)
        {
            m_CharacterController.height = height;
            m_CharacterController.center = Vector3.up * height * 0.5f;
        }

        public void Move(Vector3 translation)
        {
            bool wasGrounded = m_CharacterController.isGrounded;

            if (m_CharacterController.enabled)
            {
                m_CharacterController.Move(translation);

                if (wasGrounded != m_CharacterController.isGrounded)
                    onGroundedStateChanged?.Invoke(m_CharacterController.isGrounded);
            }
        }

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            //bura boş
            m_GroundNormal = hit.normal;

            if (hit.rigidbody)
            {
                float forceMagnitude = Velocity.magnitude * m_PushForce;
                Vector3 impactForce = (hit.moveDirection + Vector3.up * 0.35f) * forceMagnitude;
                hit.rigidbody.AddForceAtPosition(impactForce, hit.point);
            }
        }
    }
}