using SurvivalTemplatePro.BodySystem;
using SurvivalTemplatePro.Surfaces;
using UnityEngine;

namespace SurvivalTemplatePro
{
    public class FootstepsManager : CharacterBehaviour
    {
        [SerializeField]
        private LayerMask m_GroundMask;

        [SerializeField, Range(0.01f, 1f)]
        private float m_RaycastDistance = 0.3f;

        [SerializeField, Range(0.01f, 0.5f)]
        private float m_RaycastRadius = 0.3f;

        [SerializeField, Range(0f, 25f)]
        [Tooltip("If the impact speed is higher than this threeshold, an effect will be played.")]
        private float m_FallImpactThreeshold = 6f;

        [Space]

        [SerializeField, Range(0f, 1f)]
        private float m_WalkVolume = 0.35f;

        [SerializeField, Range(0f, 1f)]
        private float m_CrouchVolume = 0.2f;

        [SerializeField, Range(0f, 1f)]
        private float m_RunVolume = 1f;

        private HumanoidFoot m_LastFootDown;
        private ICharacterMover m_Mover;


        public override void OnInitialized()
        {
            GetModule(out m_Mover);

            m_Mover.onStepCycleEnded += PlayFootstepEffect;
            m_Mover.onFallImpact += PlayFallImpactEffects;
        }

        public void PlayFootstepEffect()
        {
            m_LastFootDown = (m_LastFootDown == HumanoidFoot.Left ? HumanoidFoot.Right : HumanoidFoot.Left);

            float volumeFactor = m_WalkVolume;

            bool isRunning = m_Mover.ActiveMotions.Has(CharMotionMask.Run);
            bool isCrouched = m_Mover.ActiveMotions.Has(CharMotionMask.Crouch);

            if (isRunning)
                volumeFactor = m_RunVolume;
            else if (isCrouched)
                volumeFactor = m_CrouchVolume;

            if (CheckGround(out RaycastHit hitInfo))
            {
                SurfaceEffects footstepType = isRunning ? SurfaceEffects.HardFootstep : SurfaceEffects.SoftFootstep;
                SurfaceManager.SpawnEffect(hitInfo, footstepType, volumeFactor);
            }
        }

        public void PlayFallImpactEffects(float impactSpeed)
        {
            if (Mathf.Abs(impactSpeed) >= m_FallImpactThreeshold)
            {
                if (CheckGround (out RaycastHit hitInfo))
                    SurfaceManager.SpawnEffect(hitInfo, SurfaceEffects.FallImpact, 1f);
            }
        }

        private bool CheckGround(out RaycastHit hitInfo)
        {
            Ray ray = new Ray(transform.position + Vector3.up * 0.3f, Vector3.down);

            bool hitSomething = Physics.Raycast(ray, out hitInfo, m_RaycastDistance, m_GroundMask, QueryTriggerInteraction.Ignore);

            if (!hitSomething)
                hitSomething = Physics.SphereCast(ray, m_RaycastRadius, out hitInfo, m_RaycastDistance, m_GroundMask, QueryTriggerInteraction.Ignore);

            return hitSomething;
        }
    }
}