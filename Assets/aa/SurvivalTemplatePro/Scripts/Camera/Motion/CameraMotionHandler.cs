using SurvivalTemplatePro.BodySystem;
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SurvivalTemplatePro.CameraSystem
{
    /// <summary>
    /// Handles the additive motion of a camera.
    /// </summary>
    public class CameraMotionHandler : CharacterBehaviour, ICameraMotionHandler
    {
        #region Internal
        [Serializable]
        private class FallImpactMotionModule : ForceMotionModule
        {
            public Vector2 FallImpactRange;
        }
        #endregion

        [SerializeField]
        [Tooltip("The camera that will be rotated using a spring.")]
        private Transform m_Camera;

        [SerializeField]
        [Tooltip("The camera root that will be rotated through bobbing.")]
        private Transform m_CameraRoot;

        [BHeader("Motion Settings")]

        [SerializeField, Range(0f, 200f)]
        [Tooltip("How smooth should the motion spring movement be.")]
        private float m_MotionLerpSpeed;

        [SerializeField, Range(0.1f, 20f)]
        [Tooltip("How fast should the camera bob be.")]
        private float m_BobTransitionSpeed = 2f;

        [Space]

        [SerializeField]
        [Tooltip("Input settings.")]
        private InputMotionModule m_Input;

        [SerializeField]
        [Tooltip("Sway forces (applied when moving the camera).")]
        private SwayMotionModule m_Sway;

        [SerializeField]
        [Tooltip("Fall impact forces.")]
        private FallImpactMotionModule m_FallImpact;

        [Space]

        [SerializeField]
        [Tooltip("Idle camera motion state.")]
        private CameraMotionState m_IdleState;

        [SerializeField]
        [Tooltip("Walk camera motion state.")]
        private CameraMotionState m_WalkState;

        [SerializeField]
        [Tooltip("Run camera motion state.")]
        private CameraMotionState m_RunState;

        [SerializeField]
        [Tooltip("Crouch camera motion stat")]
        private CameraMotionState m_CrouchState;

        [BHeader("Shake Settings")]

        [SerializeField, Range(0f, 200f)]
        [Tooltip("How smooth should the shake spring movement be.")]
        private float m_ShakeLerpSpeed;

        [SerializeField]
        [Tooltip("The explosion shake settings.")]
        private ShakeSettings m_ExplosionShake;

        [SerializeField]
        [Tooltip("The default shake spring force settings (Stiffness and Damping).")]
        private Sprind.Settings m_ShakeSpringSettings = Sprind.Settings.Default;

        [BHeader("Custom Force Settings")]

        [SerializeField, Range(0f, 200f)]
        [Tooltip("How smooth should the force spring movement be.")]
        private float m_ForceLerpSpeed;

        [SerializeField]
        [Tooltip("The default spring force settings (Stiffness and Damping), when external forces are being applied.")]
        private Sprind.Settings m_DefaultForceSpringSettings = Sprind.Settings.Default;

        // Springs
        private Sprind m_RotationMotionSpring;
        private Sprind m_PositionMotionSpring;
        private Sprind m_PositionShakeSpring;
        private Sprind m_RotationShakeSpring;
        private Sprind m_CustomForceSpring;

        // States
        private CameraMotionState m_CurrentState;
        private CameraMotionState m_CustomState;
        private Vector3 m_StatePosition;
        private Vector3 m_StateRotation;

        private Vector2 m_LookInput;
        private Vector3 m_SwayVelocity;

        // Headbob
        private HumanoidFoot m_LastFootDown;
        private Vector3 m_PositionBobAmplitude;
        private Vector3 m_RotationBobAmplitude;

        private Vector3 m_CustomPositionBobAmplitude;
        private Vector3 m_CustomRotationBobAmplitude;

        private CameraBob m_CustomHeadbob;
        private float m_CustomHeadbobSpeed;

        // Shakes
        private readonly List<SpringShake> m_Shakes = new List<SpringShake>();

        private ILookHandler m_LookHandler;
        private ICharacterMover m_Mover;


        public override void OnInitialized()
        {
            GetModule(out m_Mover);
            GetModule(out m_LookHandler);

            // Initalize the springs
            m_PositionMotionSpring = new Sprind(Sprind.RefreshType.OverridePosition, m_Camera, default, m_MotionLerpSpeed);
            m_RotationMotionSpring = new Sprind(Sprind.RefreshType.OverrideRotation, m_Camera, default, m_MotionLerpSpeed);

            m_PositionShakeSpring = new Sprind(Sprind.RefreshType.AddToPosition, m_Camera, default, m_ShakeLerpSpeed, m_ShakeSpringSettings);
            m_RotationShakeSpring = new Sprind(Sprind.RefreshType.AddToRotation, m_Camera, default, m_ShakeLerpSpeed, m_ShakeSpringSettings);

            m_CustomForceSpring = new Sprind(Sprind.RefreshType.AddToRotation, m_Camera, default, m_ForceLerpSpeed, m_DefaultForceSpringSettings);

            ShakeImpactEvent.onShakeEvent += OnShakeEvent;

            m_Mover.onStepCycleEnded += OnStepCycleEnd;
            m_Mover.onFallImpact += DoFallImpact;
        }

        private void OnDestroy()
        {
            ShakeImpactEvent.onShakeEvent -= OnShakeEvent;
        }

        public void AddRotationForce(SpringForce force) => m_CustomForceSpring.AddForce(force);
        public void AddRotationForce(Vector3 recoilForce, int distribution = 1) => m_CustomForceSpring.AddForce(recoilForce, distribution);

        public void SetCustomForceSpringSettings(Sprind.Settings settings) => m_CustomForceSpring.Adjust(settings);
        public void ClearCustomForceSpringSettings() => m_CustomForceSpring.Adjust(m_DefaultForceSpringSettings);

        public void SetCustomHeadbob(CameraBob headbob, float speed)
        {
            m_CustomHeadbob = headbob;
            m_CustomHeadbobSpeed = speed;
        }

        public void ClearCustomHeadbob() => m_CustomHeadbob = null;

        public void SetCustomState(CameraMotionState state) => m_CustomState = state;
        public void ClearCustomState() => m_CustomState = null;

        public void DoShake(ShakeSettings shake, float scale = 1f)
        {
            if (shake.Duration > 0.01f)
                m_Shakes.Add(new SpringShake(shake, m_PositionShakeSpring, m_RotationShakeSpring, scale));
        }

        private void FixedUpdate()
        {
            if (!IsInitialized || m_Mover == null)
                return;

            UpdateState();

            float fixedDeltaTime = Time.fixedDeltaTime;
            m_StatePosition = Vector3.zero;
            m_StateRotation = Vector3.zero;

            UpdateNoise(ref m_StatePosition, ref m_StateRotation);
            UpdateSway(ref m_StatePosition, ref m_StateRotation);

            m_PositionMotionSpring.AddForce(m_StatePosition);
            m_RotationMotionSpring.AddForce(m_StateRotation);
            UpdateShakes();

            m_PositionMotionSpring.FixedUpdate(fixedDeltaTime);
            m_RotationMotionSpring.FixedUpdate(fixedDeltaTime);
            m_CustomForceSpring.FixedUpdate(fixedDeltaTime);
            m_PositionShakeSpring.FixedUpdate(fixedDeltaTime);
            m_RotationShakeSpring.FixedUpdate(fixedDeltaTime);
        }

        private void LateUpdate()
        {
            if (!IsInitialized || m_CurrentState == null)
                return;

            float deltaTime = Time.deltaTime;
            Vector3 cameraPosition = Vector3.zero;
            Vector3 cameraRotation = Vector3.zero;

            UpdateHeadbobs(deltaTime, false, ref cameraPosition, ref cameraRotation);
            UpdateCustomHeadbob(deltaTime, ref cameraPosition, ref cameraRotation);

            m_CameraRoot.localPosition = cameraPosition;
            m_CameraRoot.localEulerAngles = cameraRotation;

            m_PositionMotionSpring.Update(deltaTime);
            m_RotationMotionSpring.Update(deltaTime);
            m_CustomForceSpring.Update(deltaTime);
            m_PositionShakeSpring.Update(deltaTime);
            m_RotationShakeSpring.Update(deltaTime);
        }

        private void UpdateState()
        {
            if (m_CustomState != null)
            {
                TrySetState(m_CustomState);
                return;
            }

            if (m_Mover.ActiveMotions.Has(CharMotionMask.Run))
                TrySetState(m_RunState);
            else if (m_Mover.ActiveMotions.Has(CharMotionMask.Crouch))
                TrySetState(m_CrouchState);
            else if (IsWalking())
                TrySetState(m_WalkState);
            else
                TrySetState(m_IdleState);
        }

        private void TrySetState(CameraMotionState state)
        {
            if (m_CurrentState != state)
            {
                // Exit force
                if (m_CurrentState != null)
                    m_RotationMotionSpring.AddForce(m_CurrentState.ExitForce * 10f);

                m_CurrentState = state;

                m_PositionMotionSpring.Adjust(m_CurrentState.PositionSpring);
                m_RotationMotionSpring.Adjust(m_CurrentState.RotationSpring);

                // Enter force
                if (m_CurrentState != null)
                    m_RotationMotionSpring.AddForce(m_CurrentState.EnterForce * 10f);
            }
        }

        private void UpdateHeadbobs(float deltaTime, bool additive, ref Vector3 position, ref Vector3 rotation)
        {
            CameraBob activeBob = null;

            if (m_CustomHeadbob == null)
                activeBob = m_CurrentState.Headbob;

            // Calculate the bob "time" based on the player's move cycle
            float bobTime = m_Mover.StepCycle * Mathf.PI;

            if (m_LastFootDown == HumanoidFoot.Right)
                bobTime = Mathf.PI - bobTime;

            // Transition smoothly between different bob amplitudes
            m_PositionBobAmplitude = Vector3.Lerp(m_PositionBobAmplitude, activeBob == null ? Vector3.zero : activeBob.PositionAmplitude, deltaTime * m_BobTransitionSpeed);
            m_RotationBobAmplitude = Vector3.Lerp(m_RotationBobAmplitude, activeBob == null ? Vector3.zero : activeBob.RotationAmplitude, deltaTime * m_BobTransitionSpeed);

            // Use the cosine function to smooth out the beginning and ending of the bob animation
            var positionBob = new Vector3(Mathf.Cos(bobTime), Mathf.Cos(bobTime * 2), Mathf.Cos(bobTime));
            var rotationBob = new Vector3(Mathf.Cos(bobTime * 2), Mathf.Cos(bobTime), Mathf.Cos(bobTime));

            // Multiply the current amplitude and cosine values to get the final bob vectors
            positionBob = Vector3.Scale(positionBob, m_PositionBobAmplitude);
            rotationBob = Vector3.Scale(rotationBob, m_RotationBobAmplitude);

            // Finally, move and rotate the camera using the bob values
            if (additive)
            {
                position += positionBob;
                rotation += rotationBob;
            }
            else
            {
                position = positionBob;
                rotation = rotationBob;
            }
        }

        private void UpdateCustomHeadbob(float deltaTime, ref Vector3 position, ref Vector3 rotation)
        {
            float bobTime = Time.time * m_CustomHeadbobSpeed;

            // Transition smoothly between different bob amplitudes
            m_CustomPositionBobAmplitude = Vector3.Lerp(m_CustomPositionBobAmplitude, m_CustomHeadbob == null ? Vector3.zero : m_CustomHeadbob.PositionAmplitude, deltaTime * m_BobTransitionSpeed);
            m_CustomRotationBobAmplitude = Vector3.Lerp(m_CustomRotationBobAmplitude, m_CustomHeadbob == null ? Vector3.zero : m_CustomHeadbob.RotationAmplitude, deltaTime * m_BobTransitionSpeed);

            // Use the cosine function to smooth out the beginning and ending of the bob animation
            var positionBob = new Vector3(Mathf.Cos(bobTime), Mathf.Cos(bobTime * 2), Mathf.Cos(bobTime));
            var rotationBob = new Vector3(Mathf.Cos(bobTime * 2), Mathf.Cos(bobTime), Mathf.Cos(bobTime));

            // Multiply the current amplitude and cosine values to get the final bob vectors
            positionBob = Vector3.Scale(positionBob, m_CustomPositionBobAmplitude);
            rotationBob = Vector3.Scale(rotationBob, m_CustomRotationBobAmplitude);

            // Finally, move and rotate the camera using the bob values
            position += positionBob;
            rotation += rotationBob;
        }

        private void UpdateSway(ref Vector3 position, ref Vector3 rotation)
        {
            if (!m_Sway.Enabled)
                return;

            // Looking Sway
            m_LookInput = m_LookHandler.CurrentInput;

            m_LookInput *= m_Input.LookInputMultiplier;
            m_LookInput = Vector2.ClampMagnitude(m_LookInput, m_Input.MaxLookInput);

            position += new Vector3(
                m_LookInput.x * m_Sway.LookPositionSway.x * 0.125f,
                m_LookInput.y * m_Sway.LookPositionSway.y * -0.125f,
                m_LookInput.y * m_Sway.LookPositionSway.z * -0.125f);

            rotation += new Vector3(
                m_LookInput.x * m_Sway.LookRotationSway.x,
                m_LookInput.y * -m_Sway.LookRotationSway.y,
                m_LookInput.y * -m_Sway.LookRotationSway.z);

            // Strafing Sway
            m_SwayVelocity = transform.InverseTransformVector(m_Mover.Motor.Velocity);

            if (Mathf.Abs(m_SwayVelocity.y) < 1.5f)
                m_SwayVelocity.y = 0f;

            position += new Vector3(
                m_SwayVelocity.x * m_Sway.StrafePositionSway.x,
                -Mathf.Abs(m_SwayVelocity.x * m_Sway.StrafePositionSway.y),
                -m_SwayVelocity.z * m_Sway.StrafePositionSway.z) * 0.125f;

            rotation += new Vector3(
                -Mathf.Abs(m_SwayVelocity.x * m_Sway.StrafeRotationSway.x),
                -m_SwayVelocity.x * m_Sway.StrafeRotationSway.y,
                m_SwayVelocity.x * m_Sway.StrafeRotationSway.z);

            // Falling Sway
            if (!m_Mover.Motor.IsGrounded)
            {
                Vector2 rotationFallSway = m_Sway.FallSway * m_SwayVelocity.y;
                m_RotationMotionSpring.AddForce(rotationFallSway);
            }
        }

        private void UpdateNoise(ref Vector3 position, ref Vector3 rotation)
        {
            if (!m_CurrentState.Noise.Enabled)
                return;

            float jitter = Random.Range(0, m_CurrentState.Noise.MaxJitter);
            float speed = Time.time * m_CurrentState.Noise.NoiseSpeed;

            position.x += (Mathf.PerlinNoise(jitter, speed) - 0.5f) * m_CurrentState.Noise.PositionAmplitude.x;
            position.y += (Mathf.PerlinNoise(jitter + 1f, speed) - 0.5f) * m_CurrentState.Noise.PositionAmplitude.y;
            position.z += (Mathf.PerlinNoise(jitter + 2f, speed) - 0.5f) * m_CurrentState.Noise.PositionAmplitude.z;

            rotation.x += (Mathf.PerlinNoise(jitter, speed) - 0.5f) * m_CurrentState.Noise.RotationAmplitude.x * 3f;
            rotation.y += (Mathf.PerlinNoise(jitter + 1f, speed) - 0.5f) * m_CurrentState.Noise.RotationAmplitude.y * 3f;
            rotation.z += (Mathf.PerlinNoise(jitter + 2f, speed) - 0.5f) * m_CurrentState.Noise.RotationAmplitude.z * 3f;
        }

        private void DoFallImpact(float impactVelocity)
        {
            if (!m_FallImpact.Enabled)
                return;

            float impactVelocityAbs = Mathf.Abs(impactVelocity);

            if (impactVelocityAbs > m_FallImpact.FallImpactRange.x)
            {
                float multiplier = Mathf.Clamp01(impactVelocityAbs / m_FallImpact.FallImpactRange.y);

                m_PositionMotionSpring.AddForce(m_FallImpact.PositionForce * multiplier);
                m_RotationMotionSpring.AddForce(m_FallImpact.RotationForce * multiplier);
            }
        }

        private void OnStepCycleEnd() => m_LastFootDown = (m_LastFootDown == HumanoidFoot.Left ? HumanoidFoot.Right : HumanoidFoot.Left);
        private bool IsWalking() => !m_Mover.ActiveMotions.Has(CharMotionMask.Run) && m_Mover.Motor.Velocity.sqrMagnitude > 0.1f && m_Mover.Motor.IsGrounded;

        private void UpdateShakes()
        {
            if (m_Shakes.Count == 0)
                return;

            int i = 0;

            while (true)
            {
                if (m_Shakes[i].IsDone)
                    m_Shakes.RemoveAt(i);
                else
                {
                    m_Shakes[i].Update();
                    i++;
                }

                if (i >= m_Shakes.Count)
                    break;
            }
        }

        private void OnShakeEvent(ShakeImpactEvent impact)
        {
            if (impact == null)
                return;

            float distToImpactSqr = (transform.position - impact.Position).sqrMagnitude;
            float impactRadiusSqr = impact.Radius * impact.Radius;

            if (impactRadiusSqr - distToImpactSqr > 0f)
            {
                float distanceFactor = 1f - Mathf.Clamp01(distToImpactSqr / impactRadiusSqr);
                DoShake(m_ExplosionShake, distanceFactor * impact.Scale);
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (Application.isPlaying && m_PositionMotionSpring != null)
            {
                m_PositionMotionSpring.Adjust(m_CurrentState.PositionSpring);
                m_RotationMotionSpring.Adjust(m_CurrentState.RotationSpring);
                m_PositionShakeSpring.Adjust(m_ShakeSpringSettings);
                m_RotationShakeSpring.Adjust(m_ShakeSpringSettings);
                m_CustomForceSpring.Adjust(m_DefaultForceSpringSettings);
            }
        }
#endif
    }
}