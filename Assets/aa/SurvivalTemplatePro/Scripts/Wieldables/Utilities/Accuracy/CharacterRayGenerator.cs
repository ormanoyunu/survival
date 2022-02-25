using UnityEngine;

namespace SurvivalTemplatePro.WieldableSystem
{
    /// <summary>
    /// Generates Rays based on the parent character state.
    /// (e.g. shoot direction ray will be more random when moving)
    /// </summary>
    public class CharacterRayGenerator : CharacterBehaviour, IRayGenerator
    {
        [SerializeField]
        [Tooltip("Anchor: Transform used in determining the base ray position and orientation.")]
        private Transform m_Anchor;

        [Space]

        [SerializeField, Range(0f, 3f)]
        [Tooltip("How much will walking affect (randomly spread) the final ray.")]
        private float m_WalkSpreadMod = 1.25f;

        [SerializeField, Range(0f, 3f)]
        [Tooltip("How much will running affect (randomly spread) the final ray.")]
        private float m_RunSpreadMod = 1.5f;

        [SerializeField, Range(0f, 3f)]
        [Tooltip("How much will crouching affect (randomly spread) the final ray.")]
        private float m_CrouchSpreadMod = 0.65f;

        [SerializeField, Range(0f, 3f)]
        [Tooltip("How much will being airborne affect (randomly spread) the final ray")]
        private float m_AirborneSpreadMod = 2f;

        private ICharacterMover m_Mover;


        public override void OnInitialized() => GetModule(out m_Mover);
        
        public Ray GenerateRay(float raySpreadMod, Vector3 localOffset = default)
        {
            float raySpread = GetRaySpread() * raySpreadMod;

            Vector3 raySpreadVector = m_Anchor.TransformVector(new Vector3(Random.Range(-raySpread, raySpread), Random.Range(-raySpread, raySpread), 0f));
            Vector3 rayDirection = Quaternion.Euler(raySpreadVector) * m_Anchor.forward;

            return new Ray(m_Anchor.position + m_Anchor.TransformVector(localOffset), rayDirection);
        }

        public float GetRaySpread()
        {
            float raySpread = 1f;

            if (!m_Mover.Motor.IsGrounded)
                raySpread *= m_AirborneSpreadMod;
            else if (m_Mover.ActiveMotions.Has(CharMotionMask.Run))
                raySpread *= m_RunSpreadMod;
            else if (m_Mover.ActiveMotions.Has(CharMotionMask.Crouch))
                raySpread *= m_CrouchSpreadMod;
            else if (IsWalking())
                raySpread *= m_WalkSpreadMod;

            return raySpread;
        }

        private bool IsWalking() => !m_Mover.ActiveMotions.Has(CharMotionMask.Run) && m_Mover.Motor.Velocity.sqrMagnitude > 0.1f && m_Mover.Motor.IsGrounded;
    }
}