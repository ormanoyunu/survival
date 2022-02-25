using UnityEngine;
using UnityEngine.Events;

namespace SurvivalTemplatePro.MovementSystem
{
    public class CharacterMover : CharacterBehaviour, ICharacterMover
    {
        public CharMotionMask ActiveMotions
        {
            get => m_ActiveMotions;
            private set
            {
                if (m_ActiveMotions != value)
                    m_ActiveMotions = value;
            }
        }

        public float StepCycle { get; private set; }
        public ICharacterMotor Motor { get; private set; }
        public float VelocityMod { get; set; } = 1f;
        public CharMotionMask BlockedMotions { get; set; }

        public event UnityAction<CharMotionMask, bool> onMotionChanged;
        public event UnityAction<float> onFallImpact;
        public event UnityAction onStepCycleEnded;

        [BHeader("Core Movement")]

        [SerializeField, Range(0f, 20f)]
        [Tooltip("How fast can this character achieve max state velocity.")]
        private float m_Acceleration = 5f;

        [SerializeField, Range(0f, 20f)]
        [Tooltip("How fast will the character stop when there's no input (a high value will make the movement more snappy).")]
        private float m_Damping = 8f;

        [SerializeField, Range(0f, 1f)]
        [Tooltip("How well can this character move while airborne. ")]
        private float m_AirborneControl = 0.15f;

        [SerializeField, Range(0f, 3f)]
        [Tooltip("How much distance does this character need to cover to be considered a step.")]
        private float m_StepLength = 1.2f;

        [SerializeField, Range(0f, 10f)]
        [Tooltip("The forward speed of this character.")]
        private float m_ForwardSpeed = 2.5f;

        [SerializeField, Range(0f, 10f)]
        [Tooltip("The backward speed of this character.")]
        private float m_BackSpeed = 2.5f;

        [SerializeField, Range(0f, 10f)]
        [Tooltip("The sideway speed of this character.")]
        private float m_SideSpeed = 2.5f;

        [SerializeField]
        [Tooltip("Lowers/Increases the moving speed of the character when moving on sloped surfaces (e.g. lower speed when walking up a hill")]
        private AnimationCurve m_SlopeSpeedMod = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 1f));

        [SerializeField, Range(0f, 10f)]
        [Tooltip("A small force applied to the character to make it stick to the ground when moving on a descending terrain/collider")]
        private float m_AntiBumpFactor = 1f;

        [BHeader("Running")]

        [SerializeField]
        [Tooltip("Is this character allowed to run?")]
        private bool m_EnableRunning;

        [SerializeField, Range(1f, 10f)]
        [Tooltip("The max running speed.")]
        private float m_RunSpeed = 4.5f;

        [SerializeField, Range(0f, 3f)]
        [Tooltip("Step length specific to running.")]
        private float m_RunStepLength = 1.9f;

        [BHeader("Jumping")]

        [SerializeField]
        [Tooltip("Is this character allowed to jump?")]
        private bool m_EnableJumping;

        [SerializeField, Range(0f, 3f)]
        [Tooltip("The max height of a jump.")]
        private float m_JumpHeight = 1f;

        [SerializeField, Range(0f, 1.5f)]
        [Tooltip("How often can this character jump (in seconds).")]
        private float m_JumpTimer = 0.3f;

        [BHeader("Crouching")]

        [SerializeField]
        [Tooltip("Is this character allowed to crouch?")]
        private bool m_EnableCrouching;

        [SerializeField, Range(0f, 1f)]
        [Tooltip("The velocity mod for crouching (multiplied with the base speed: forward, back speed etc.)")]
        private float m_CrouchSpeedMod = 0.6f;

        [SerializeField, Range(0f, 3f)]
        [Tooltip("Step length specific to crouch walking.")]
        private float m_CrouchStepLength = 0.8f;

        [SerializeField, Range(0f, 2f)]
        [Tooltip("The controllers height when crouching.")]
        private float m_CrouchHeight = 1f;

        [SerializeField, Range(0f, 1f)]
        [Tooltip("How long does it take to crouch.")]
        private float m_CrouchDuration = 0.3f;

        [BHeader("Sliding")]

        [SerializeField]
        [Tooltip("Should this character slide when standing on slopes?")]
        private bool m_EnableSliding;

        [SerializeField, Range(20f, 90f)]
        [Tooltip("The angle at which the character will start to slide.")]
        private float m_SlideThreshold = 32f;

        [SerializeField, Range(0f, 5f)]
        [Tooltip("The max sliding speed.")]
        private float m_SlideSpeed = 0.5f;

        [BHeader("Misc")]

        [SerializeField, Range(0f, 100f)]
        [Tooltip("The strength of the gravity.")]
        private float m_Gravity = 20f;

        [SerializeField]
        [Tooltip("Layers that are considered obstacles.")]
        private LayerMask m_ObstacleCheckMask;

        private CharMotionMask m_ActiveMotions;

        private Vector3 m_SimulatedVelocity;
        private Vector3 m_SlideVelocity;

        private bool m_PreviouslyGrounded;
        private float m_LastLandTime;

        private float m_NextTimeCanCrouch;

        private float m_DistMovedSinceLastCycleEnded;
        private float m_CurrentStepLength;

        private float m_DefaultMotorHeight;


        public void Move(Vector3 moveInput, CharMotionMask motionInputs)
        {
            float deltaTime = Time.deltaTime;
            moveInput = Vector3.ClampMagnitude(moveInput, 1f);

            // Running inputs
            if (motionInputs.Has(CharMotionMask.Run) && !ActiveMotions.Has(CharMotionMask.Run))
                TryRun(moveInput);
            else if (!motionInputs.Has(CharMotionMask.Run) && ActiveMotions.Has(CharMotionMask.Run))
                StopRun();

            // Crouching inputs
            if (motionInputs.Has(CharMotionMask.Crouch) && !ActiveMotions.Has(CharMotionMask.Crouch))
                TryCrouch();
            else if (motionInputs.Has(CharMotionMask.Crouch) && ActiveMotions.Has(CharMotionMask.Crouch))
                TryStandUp();

            // Jump inputs
            if (motionInputs.Has(CharMotionMask.Jump) && !ActiveMotions.Has(CharMotionMask.Jump))
                TryJump(ref m_SimulatedVelocity);

            Vector3 targetVelocity = GetTargetVelocity(moveInput, m_SimulatedVelocity);

            if (!Motor.IsGrounded)
                UpdateAirborneMovement(targetVelocity, deltaTime, ref m_SimulatedVelocity);
            else if (!ActiveMotions.Has(CharMotionMask.Jump))
            {
                UpdateStepCycle(deltaTime);
                UpdateGroundedMovement(moveInput, targetVelocity, deltaTime, ref m_SimulatedVelocity);
            }

            Vector3 translation = m_SimulatedVelocity * deltaTime;

            if (Motor.IsGrounded && !ActiveMotions.Has(CharMotionMask.Jump))
                translation.y = -m_AntiBumpFactor;

            Motor.Move(translation);

            m_PreviouslyGrounded = Motor.IsGrounded;
        }

        public void ResetStateAndVelocity()
        {
            m_ActiveMotions = 0;

            m_SimulatedVelocity = m_SlideVelocity = Vector3.zero;
            m_LastLandTime = m_NextTimeCanCrouch = 0f;
            m_PreviouslyGrounded = true;
        }

        private void Awake()
        {
            Motor = Character.GetComponentInChildren<ICharacterMotor>();
            Motor.onGroundedStateChanged += OnGroundedStateChanged;

            m_DefaultMotorHeight = Motor.Height;
        }

        private void UpdateGroundedMovement(Vector3 moveInput, Vector3 targetVelocity, float deltaTime, ref Vector3 velocity)
        {
            // Make sure to lower the speed when moving on steep surfaces.
            float surfaceAngle = Vector3.Angle(Vector3.up, Motor.GroundNormal);
            targetVelocity *= m_SlopeSpeedMod.Evaluate(surfaceAngle / Motor.SlopeLimit) * VelocityMod;

            // Calculate the rate at which the current speed should increase / decrease. 
            // If the player doesn't press any movement button, use the "m_Damping" value, otherwise use "m_Acceleration".
            float targetAccel = targetVelocity.sqrMagnitude > 0f ? m_Acceleration : m_Damping;

            velocity = Vector3.Lerp(velocity, targetVelocity, targetAccel * deltaTime);

            if (ActiveMotions.Has(CharMotionMask.Run) && !CanRun(moveInput))
                StopRun();

            if (m_EnableSliding)
            {
                // Sliding...
                if (surfaceAngle > m_SlideThreshold)
                {
                    Vector3 slideDirection = (Motor.GroundNormal + Vector3.down);
                    m_SlideVelocity += slideDirection * m_SlideSpeed * deltaTime;
                }
                else
                    m_SlideVelocity = Vector3.Lerp(m_SlideVelocity, Vector3.zero, deltaTime * 10f);

                velocity += m_SlideVelocity;
            }
        }

        private void UpdateStepCycle(float deltaTime)
        {       
            // Advance step
            m_DistMovedSinceLastCycleEnded += Motor.Velocity.magnitude * deltaTime;

            // Which step length should be used?
            float targetStepLength = m_StepLength;

            if (ActiveMotions.Has(CharMotionMask.Crouch))
                targetStepLength = m_CrouchStepLength;
            else if (ActiveMotions.Has(CharMotionMask.Run))
                targetStepLength = m_RunStepLength;

            m_CurrentStepLength = Mathf.MoveTowards(m_CurrentStepLength, targetStepLength, deltaTime * 0.6f);

            // If the step cycle is complete, reset it, and send a notification.
            if (m_DistMovedSinceLastCycleEnded > m_CurrentStepLength)
            {
                m_DistMovedSinceLastCycleEnded -= m_CurrentStepLength;
                onStepCycleEnded?.Invoke();
            }

            StepCycle = m_DistMovedSinceLastCycleEnded / m_CurrentStepLength;
        }

        private void UpdateAirborneMovement(Vector3 targetVelocity, float deltaTime, ref Vector3 velocity)
        {
            if(m_PreviouslyGrounded && !ActiveMotions.Has(CharMotionMask.Jump))
                velocity.y = 0f;

            // Modify the current velocity by taking into account how well we can change direction when not grounded (see "m_AirControl" tooltip).
            velocity += targetVelocity * m_Acceleration * m_AirborneControl * deltaTime;

            // Apply gravity.
            velocity.y -= m_Gravity * deltaTime;
        }

        private bool TryRun(Vector3 moveInput)
        {
            if (CanRun(moveInput))
            {
                ActiveMotions = ActiveMotions.SetFlag(CharMotionMask.Run);
                onMotionChanged?.Invoke(CharMotionMask.Run, true);

                return true;
            }

            return false;
        }

        private bool CanRun(Vector3 moveInput) 
        {
            bool enabled = m_EnableRunning && !BlockedMotions.Has(CharMotionMask.Run);
            bool movingTooSlow = Motor.Velocity.sqrMagnitude < 2f || moveInput.sqrMagnitude < 0.1f;
            bool isCrouched = ActiveMotions.Has(CharMotionMask.Crouch);

            if (!Motor.IsGrounded || !enabled || movingTooSlow || isCrouched) 
                return false;

            Vector3 moveDirectionLocal = transform.InverseTransformVector(moveInput);
            bool wantsToMoveBack = moveDirectionLocal.z < 0f;
            bool wantsToMoveOnlySideways = Mathf.Abs(moveDirectionLocal.x) > 0.99f;

            return !wantsToMoveBack && !wantsToMoveOnlySideways;
        }

        private void StopRun()
        {
            ActiveMotions = ActiveMotions.UnsetFlag(CharMotionMask.Run);
            onMotionChanged?.Invoke(CharMotionMask.Run, false);
        }

        private bool TryJump(ref Vector3 velocity)
        {
            // If crouched, stop crouching first
            if (ActiveMotions.Has(CharMotionMask.Crouch))
            {
                TryStandUp();
                return false;
            }

            bool canJump =
                m_EnableJumping &&
                Motor.IsGrounded &&
                !BlockedMotions.Has(CharMotionMask.Jump) && 
                Time.time > m_LastLandTime + m_JumpTimer;

            if (!canJump)
                return false;

            float jumpHeight = m_JumpHeight * VelocityMod;

            if (jumpHeight < 0.1f)
                return false;

            float jumpSpeed = Mathf.Sqrt(2 * m_Gravity * jumpHeight);
            velocity = new Vector3(velocity.x, jumpSpeed, velocity.z);

            ActiveMotions = ActiveMotions.SetFlag(CharMotionMask.Jump);
            onMotionChanged?.Invoke(CharMotionMask.Jump, true);

            return true;
        }

        private void StopJump()
        {
            ActiveMotions = ActiveMotions.UnsetFlag(CharMotionMask.Jump);
            onMotionChanged?.Invoke(CharMotionMask.Jump, false);
        }

        private bool TryCrouch()
        {
            bool canCrouch =
                m_EnableCrouching &&
                Time.time > m_NextTimeCanCrouch &&
                Motor.IsGrounded;

            if (canCrouch)
            {
                if (ActiveMotions.Has(CharMotionMask.Run))
                    StopRun();

                Motor.SetHeight(m_CrouchHeight);
                m_NextTimeCanCrouch = Time.time + m_CrouchDuration;

                ActiveMotions = ActiveMotions.SetFlag(CharMotionMask.Crouch);
                onMotionChanged?.Invoke(CharMotionMask.Crouch, true);
            }

            return canCrouch;
        }

        private bool TryStandUp()
        {
            bool obstacleAbove = DoCollisionCheck(true, m_DefaultMotorHeight - Motor.Height + 0.05f);
            bool canStopCrouching = Time.time > m_NextTimeCanCrouch && !obstacleAbove;

            if (canStopCrouching)
            {
                Motor.SetHeight(m_DefaultMotorHeight);
                m_NextTimeCanCrouch = Time.time + m_CrouchDuration;

                ActiveMotions = ActiveMotions.UnsetFlag(CharMotionMask.Crouch);
                onMotionChanged?.Invoke(CharMotionMask.Crouch, false);
            }

            return canStopCrouching;
        }

        private Vector3 GetTargetVelocity(Vector3 moveDirection, Vector3 currentVelocity)
        {
            bool wantsToMove = moveDirection.sqrMagnitude > 0f;
            moveDirection = (wantsToMove ? moveDirection : currentVelocity.normalized);

            float desiredSpeed = 0f;

            if (wantsToMove)
            {
                // Set the default speed (forward)
                desiredSpeed = m_ForwardSpeed;

                Vector3 moveDirectionLocal = transform.InverseTransformVector(moveDirection);

                // Sideways movement
                if (Mathf.Abs(moveDirectionLocal.x) > 0f)
                    desiredSpeed = m_SideSpeed;

                // Back movement
                if (moveDirectionLocal.z < 0f)
                    desiredSpeed = m_BackSpeed;

                // Running
                if (ActiveMotions.Has(CharMotionMask.Run))
                {
                    // If the player wants to move forward or sideways, apply the run speed multiplier.
                    if(desiredSpeed == m_ForwardSpeed || desiredSpeed == m_SideSpeed)
                        desiredSpeed = m_RunSpeed;
                }

                // If we're crouching...
                if (ActiveMotions.Has(CharMotionMask.Crouch))
                    desiredSpeed *= m_CrouchSpeedMod;
            }

            return moveDirection * desiredSpeed;
        }

        private void OnGroundedStateChanged(bool isGrounded)
        {
            if (!m_PreviouslyGrounded && isGrounded)
                onFallImpact?.Invoke(Mathf.Abs(m_SimulatedVelocity.y));

            if (isGrounded && ActiveMotions.Has(CharMotionMask.Jump))
            {
                m_SimulatedVelocity = Vector3.ClampMagnitude(m_SimulatedVelocity, 1f);
                StopJump();
            }

            if (!isGrounded && ActiveMotions.Has(CharMotionMask.Run))
                StopRun();

            m_LastLandTime = Time.time;
        }

        public bool DoCollisionCheck(bool checkAbove, float maxDistance)
        {
            Vector3 rayOrigin = Motor.transform.position + (checkAbove ? Vector3.up * Motor.Height / 2: Vector3.up * Motor.Height);
            Vector3 rayDirection = checkAbove ? Vector3.up : Vector3.down;

            return Physics.SphereCast(new Ray(rayOrigin, rayDirection), Motor.Radius, maxDistance, m_ObstacleCheckMask, QueryTriggerInteraction.Ignore);
        }
    }
}