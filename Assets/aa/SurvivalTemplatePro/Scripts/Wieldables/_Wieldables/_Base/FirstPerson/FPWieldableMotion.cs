using UnityEngine;

namespace SurvivalTemplatePro.WieldableSystem
{
	public class FPWieldableMotion : WieldableEffectsHandler
	{
		#region Internal
		[System.Serializable]
		public class RetractionMotionModule : ForceMotionModule
		{
			[Space(3f)]

			[Range(0.1f, 5f)]
			public float RetractionDistance = 0.55f;
		}

		[System.Serializable]
		public class StateSettings : WieldableEffect
		{
			[SerializeField, HideInInspector]
			public FPWieldableMotion Motion;

			public FPWieldableMotionState State;

			public override void TriggerEffect(float value) => Motion.SetCustomState(State);
		}
		#endregion

		public Transform Pivot => m_Pivot ?? transform.FindDeepChild("Pivot");

		[SerializeField, Range(0f, 100f)]
		private float m_SpringLerpSpeed = 40f;

		[SerializeField, Range(0f, 3.14f)]
		private float m_PositionBobOffset;

		[SerializeField, Range(0f, 3.14f)]
		private float m_RotationBobOffset = 0.5f;

		[BHeader("General")]

		[SerializeField]
		private InputMotionModule m_Input;

		[SerializeField]
		private SwayMotionModule m_Sway;

		[Space]

		[SerializeField]
		private ForceMotionModule m_ViewOffset;

		[SerializeField]
		private RetractionMotionModule m_RetractionOffset;

		[Space]

		[SerializeField]
		private ForceMotionModule m_FallImpactForce;

		[BHeader("Steps")]

		[SerializeField]
		private ForceMotionModule m_WalkStepForce;

		[SerializeField]
		private ForceMotionModule m_CrouchStepForce;

		[SerializeField]
		private ForceMotionModule m_RunStepForce;

		[BHeader("States")]

		[SerializeField]
		private FPWieldableMotionState m_IdleState;

		[SerializeField]
		private FPWieldableMotionState m_WalkState;

		[SerializeField]
		private FPWieldableMotionState m_RunState;

		[SerializeField]
		private FPWieldableMotionState m_CrouchState;

		[SerializeField]
		private FPWieldableMotionState m_BrokenState;

		[Space]

		[SerializeField]
		private StateSettings[] m_CustomStateSettings;

		private Transform m_Pivot;
		private Transform m_Model;

		// Springs
		private Sprind m_PositionSpring;
		private Sprind m_RotationSpring;

		private Vector3 m_StatePosition;
		private Vector3 m_StateRotation;

		// Motion states
		private FPWieldableMotionState m_CurrentState;
		private FPWieldableMotionState m_CustomState;

		// Bob
		private int m_LastFootDown;
		private float m_CurrentBobParam;

		// View Pos
		private Vector3 m_ViewPosOffset;
		private Vector3 m_ViewRotOffset;

		// Sway
		private Vector2 m_CharacterInput;
		private Vector3 m_LocalCharacterVelocity;

		// State visualization
		private FPWieldableMotionState m_StateToVisualize;
		private float m_VisualizationSpeed;
		private bool m_FirstStepTriggered;

		private IWieldable m_Wieldable;
		private ICharacter m_Character;
		private ICharacterMover m_Mover;
		private IInteractionHandler m_Interaction;
		private ILookHandler m_LookHandler;

		private const float k_RotStrength = 0.2f;
		private const float k_PosStrength = 0.01f;

		// TODO: Remove
		private IReloadHandler m_ReloadHandler;


#if UNITY_EDITOR
		public void VisualizeState(FPWieldableMotionState motionState, float speed = 3f)
		{
			m_StateToVisualize = motionState;

            m_VisualizationSpeed = speed;
			m_CurrentBobParam = 0f;
        }
#endif

		public override WieldableEffect[] GetAllEffects() => m_CustomStateSettings;
		public override void StopEffects() => ClearCustomState();

        public void SetCustomState(FPWieldableMotionState state) => m_CustomState = state;
		public void ClearCustomState() => m_CustomState = null;

		public void ResetPhysics() 
		{
			m_PositionSpring.Reset();
			m_RotationSpring.Reset();

			m_CurrentBobParam = 0f;

			m_CurrentState = null;
			m_CustomState = null;
		}

        private void Awake()
        {
			m_Wieldable = GetComponentInParent<IWieldable>();
			m_Wieldable.onEquippingStarted += OnEquipped;

			m_Pivot = transform.FindDeepChild("Pivot");
			if (m_Pivot == null)
			{
				Debug.LogError("You have no pivot under this object. A pivot is an empty game object with the name 'Pivot'");
				enabled = false;
				return;
			}

			m_Model = transform.FindDeepChild("Model");
			if (m_Model == null)
			{
				Debug.LogError("You have no model under this object. The model should have the name 'Model'");
				enabled = false;
				return;
			}

			m_Pivot.localRotation = Quaternion.identity;
			Transform prevParent = m_Pivot.parent;

			var root = new GameObject("MotionRoot");
			root.transform.SetParent(prevParent);
			root.transform.position = m_Pivot.position;
			root.transform.rotation = m_Pivot.rotation;

			m_Pivot.SetParent(root.transform, true);
			m_Model.SetParent(m_Pivot, true);

			m_PositionSpring = new Sprind(Sprind.RefreshType.OverridePosition, m_Pivot, m_Pivot.transform.localPosition, m_SpringLerpSpeed);
			m_RotationSpring = new Sprind(Sprind.RefreshType.OverrideRotation, m_Pivot, m_Pivot.transform.localEulerAngles, m_SpringLerpSpeed);
		}

		private void OnEquipped() 
		{
			if (m_Character == null)
			{
				if (m_Mover != null)
				{
					m_Mover.onStepCycleEnded -= OnStepCycleEnded;
					m_Mover.onFallImpact -= OnFallImpact;
				}

				m_Character = m_Wieldable.Character;
				m_Character.TryGetModule(out m_Interaction);
				m_Character.TryGetModule(out m_Mover);
				m_Character.TryGetModule(out m_LookHandler);

				m_Mover.onStepCycleEnded += OnStepCycleEnded;
				m_Mover.onFallImpact += OnFallImpact;

				// TODO: Remove
				m_ReloadHandler = m_Wieldable as IReloadHandler;
			}

			ResetPhysics();
		}

        private void OnValidate()
		{
			if (Application.isPlaying)
			{
				if (m_CurrentState != null && m_Wieldable.Character != null)
				{
					m_PositionSpring.Adjust(m_CurrentState.PositionSpring);
					m_RotationSpring.Adjust(m_CurrentState.RotationSpring);
				}
			}

			if (m_CustomStateSettings != null && m_CustomStateSettings.Length > 0)
			{
				foreach (var custom in m_CustomStateSettings)
					custom.Motion = this;
			}
		}

        private void FixedUpdate()
		{
			UpdateState();

			float fixedDeltaTime = Time.fixedDeltaTime;
			m_StatePosition = m_StateRotation = Vector3.zero;

			UpdateOffset(ref m_StatePosition, ref m_StateRotation);
			UpdateSway(ref m_StatePosition, ref m_StateRotation);
			UpdateViewOffset(ref m_StatePosition, ref m_StateRotation);
			UpdateRetractionOffset(ref m_StatePosition, ref m_StateRotation);
			UpdateBob(fixedDeltaTime, ref m_StatePosition, ref m_StateRotation);
			UpdateNoise(ref m_StatePosition, ref m_StateRotation);

			m_PositionSpring.AddForce(m_StatePosition);
			m_RotationSpring.AddForce(m_StateRotation);

			m_PositionSpring.FixedUpdate(fixedDeltaTime);
			m_RotationSpring.FixedUpdate(fixedDeltaTime);
		}

		private void Update()
		{
			float deltaTime = Time.deltaTime;

			m_RotationSpring.Update(deltaTime);
			m_PositionSpring.Update(deltaTime);
		}

		private void UpdateState()
		{
			if (m_StateToVisualize != null)
				TrySetState(m_StateToVisualize);
			else if (m_CustomState != null)
				TrySetState(m_CustomState);
			else
			{
				if (m_Mover.ActiveMotions.Has(CharMotionMask.Run))
					TrySetState(m_RunState);
				else if (m_Wieldable.ItemDurability != null && m_Wieldable.ItemDurability.Float <= 0.001f)
					TrySetState(m_BrokenState);
				else if (m_Mover.ActiveMotions.Has(CharMotionMask.Crouch))
					TrySetState(m_CrouchState);
				else if (IsWalking())
					TrySetState(m_WalkState);
				else
					TrySetState(m_IdleState);
			}
        }

        private void TrySetState(FPWieldableMotionState state)
		{
			if (m_CurrentState != state)
			{
				// Exit force
				if (m_CurrentState != null)
					m_RotationSpring.AddForce(m_CurrentState.ExitForce * 10f);

				m_CurrentState = state;

				m_PositionSpring.Adjust(state.PositionSpring);
				m_RotationSpring.Adjust(state.RotationSpring);

				// Enter force
				if (m_CurrentState != null)
					m_RotationSpring.AddForce(m_CurrentState.EnterForce * 10f);
			}
		}

		private void UpdateBob(float deltaTime, ref Vector3 position, ref Vector3 rotation)
		{
			if (!m_CurrentState.Bob.Enabled)
				return;

			if (m_StateToVisualize != null)
			{
				m_CurrentBobParam += deltaTime * m_VisualizationSpeed * 2;

				if (!m_FirstStepTriggered && m_CurrentBobParam >= Mathf.PI)
				{
					m_FirstStepTriggered = true;
					ApplyStepForce();
				}

				if (m_CurrentBobParam >= Mathf.PI * 2)
				{
					m_CurrentBobParam -= Mathf.PI * 2;
					m_FirstStepTriggered = false;
					ApplyStepForce();
				}
			}
			else
			{
				m_CurrentBobParam = m_Mover.StepCycle * Mathf.PI;

				if (m_LastFootDown != 0)
					m_CurrentBobParam += Mathf.PI;
			}

			Vector3 posBobAmplitude = Vector3.zero;
			Vector3 rotBobAmplitude = Vector3.zero;

			// Update position bob
			posBobAmplitude.x = m_CurrentState.Bob.PositionAmplitude.x * -0.01f;
			position.x += Mathf.Cos(m_CurrentBobParam + m_PositionBobOffset) * posBobAmplitude.x;

			posBobAmplitude.y = m_CurrentState.Bob.PositionAmplitude.y * 0.01f;
			position.y += Mathf.Cos(m_CurrentBobParam * 2 + m_PositionBobOffset) * posBobAmplitude.y;

			posBobAmplitude.z = m_CurrentState.Bob.PositionAmplitude.z * 0.01f;
			position.z += Mathf.Cos(m_CurrentBobParam + m_PositionBobOffset) * posBobAmplitude.z;

			// Update rotation bob
			rotBobAmplitude.x = m_CurrentState.Bob.RotationAmplitude.x * k_RotStrength;
			rotation.x += Mathf.Cos(m_CurrentBobParam * 2 + m_RotationBobOffset) * rotBobAmplitude.x;

			rotBobAmplitude.y = m_CurrentState.Bob.RotationAmplitude.y * k_RotStrength;
			rotation.y += Mathf.Cos(m_CurrentBobParam + m_RotationBobOffset) * rotBobAmplitude.y;

			rotBobAmplitude.z = m_CurrentState.Bob.RotationAmplitude.z * k_RotStrength;
			rotation.z += Mathf.Cos(m_CurrentBobParam + m_RotationBobOffset) * rotBobAmplitude.z;
		}

		private void UpdateOffset(ref Vector3 position, ref Vector3 rotation)
		{
			// TODO: Remove
			if (m_ReloadHandler != null && m_ReloadHandler.IsReloading)
				return;

			if (!m_CurrentState.Offset.Enabled)
				return;

			position += m_CurrentState.Offset.PositionOffset * k_PosStrength;
			rotation += m_CurrentState.Offset.RotationOffset * k_RotStrength * 10f;
		}

		private void UpdateViewOffset(ref Vector3 position, ref Vector3 rotation)
		{
			if (!m_ViewOffset.Enabled || !m_Mover.Motor.IsGrounded)
				return;

			float angle = m_LookHandler.LookAngle.x;

			Vector3 targetViewOffsetPos = Vector3.zero;
			Vector3 targetViewOffsetRot = Vector3.zero;

			if (Mathf.Abs(angle) > 30f)
			{
				float angleFactor = 1f - Mathf.Min((70f - Mathf.Abs(angle)), 70f) / 40f;
				targetViewOffsetPos = m_ViewOffset.PositionForce.Force * angleFactor / 100;
				targetViewOffsetRot = m_ViewOffset.RotationForce.Force * angleFactor;
			}

			m_ViewPosOffset = Vector3.Lerp(m_ViewPosOffset, targetViewOffsetPos, 1f);
			m_ViewRotOffset = Vector3.Lerp(m_ViewRotOffset, targetViewOffsetRot, 1f);

			position += m_ViewPosOffset;
			rotation += m_ViewRotOffset;
		}

		private void UpdateRetractionOffset(ref Vector3 position, ref Vector3 rotation)
		{
			if (!m_RetractionOffset.Enabled)
				return;

			if (m_Mover.Motor.IsGrounded && m_Interaction.HoverInfo != null && m_Interaction.HoveredObjectDistance < m_RetractionOffset.RetractionDistance)
            {
                position += m_RetractionOffset.PositionForce.Force * k_PosStrength;
                rotation += m_RetractionOffset.RotationForce.Force;
            }
        }
			
		private void UpdateSway(ref Vector3 position, ref Vector3 rotation)
		{
			if (!m_Sway.Enabled)
				return;

			m_CharacterInput = m_LookHandler.CurrentInput * 0.5f;
			m_CharacterInput = new Vector2(-m_CharacterInput.y, m_CharacterInput.x);

			m_CharacterInput *= m_Input.LookInputMultiplier;
			m_CharacterInput = Vector2.ClampMagnitude(m_CharacterInput, m_Input.MaxLookInput);

			m_LocalCharacterVelocity = transform.InverseTransformVector(m_Mover.Motor.Velocity);

			if (Mathf.Abs(m_LocalCharacterVelocity.y) < 1.5f)
				m_LocalCharacterVelocity.y = 0f;

			// Look position sway
			position += new Vector3(
				m_CharacterInput.x * m_Sway.LookPositionSway.x * k_PosStrength,
				m_CharacterInput.y * m_Sway.LookPositionSway.y * -k_PosStrength,
				m_CharacterInput.y * m_Sway.LookPositionSway.z * -k_PosStrength);

			// Look rotation sway
			rotation += new Vector3(
				m_CharacterInput.y * m_Sway.LookRotationSway.x,
				m_CharacterInput.x * -m_Sway.LookRotationSway.y,
				m_CharacterInput.x * -m_Sway.LookRotationSway.z);

			// Strafe position sway
			position += new Vector3(
				m_LocalCharacterVelocity.x * m_Sway.StrafePositionSway.x,
				-Mathf.Abs(m_LocalCharacterVelocity.x * m_Sway.StrafePositionSway.y),
				-m_LocalCharacterVelocity.z * m_Sway.StrafePositionSway.z) * k_PosStrength;

			// Strafe rotation sway
			rotation += new Vector3(
				-Mathf.Abs(m_LocalCharacterVelocity.x * m_Sway.StrafeRotationSway.x),
				-m_LocalCharacterVelocity.x * m_Sway.StrafeRotationSway.y,
				m_LocalCharacterVelocity.x * m_Sway.StrafeRotationSway.z) * k_RotStrength;

			// Falling
			if (!m_Mover.Motor.IsGrounded)
			{
				Vector2 rotationFallSway = m_Sway.FallSway * m_Mover.Motor.Velocity.y;
				rotationFallSway = Vector2.ClampMagnitude(rotationFallSway, m_Sway.MaxFallSway);

				m_RotationSpring.AddForce(rotationFallSway);
			}
		}

		private void UpdateNoise(ref Vector3 position, ref Vector3 rotation)
		{
			if (!m_CurrentState.Noise.Enabled)
				return;

			float jitter = Random.Range(0, m_CurrentState.Noise.MaxJitter);
			float speed = Time.time * m_CurrentState.Noise.NoiseSpeed;

			position.x += (Mathf.PerlinNoise(jitter, speed) - 0.5f) * m_CurrentState.Noise.PositionAmplitude.x * k_PosStrength;
			position.y += (Mathf.PerlinNoise(jitter + 1f, speed) - 0.5f) * m_CurrentState.Noise.PositionAmplitude.y * k_PosStrength;
			position.z += (Mathf.PerlinNoise(jitter + 2f, speed) - 0.5f) * m_CurrentState.Noise.PositionAmplitude.z * k_PosStrength;

			rotation.x += (Mathf.PerlinNoise(jitter, speed) - 0.5f) * m_CurrentState.Noise.RotationAmplitude.x * 3f;
			rotation.y += (Mathf.PerlinNoise(jitter + 1f, speed) - 0.5f) * m_CurrentState.Noise.RotationAmplitude.y * 3f;
			rotation.z += (Mathf.PerlinNoise(jitter + 2f, speed) - 0.5f) * m_CurrentState.Noise.RotationAmplitude.z * 3f;
		}

		private void OnFallImpact(float impactSpeed)
		{
			if (!m_FallImpactForce.Enabled || !gameObject.activeSelf)
				return;

			m_PositionSpring.AddDistributedForce(m_Pivot.InverseTransformVector(m_FallImpactForce.PositionForce.Force) * impactSpeed * k_PosStrength, m_FallImpactForce.PositionForce.Distribution);
			m_RotationSpring.AddDistributedForce(m_FallImpactForce.RotationForce.Force * impactSpeed, m_FallImpactForce.RotationForce.Distribution);
		}

		private void OnStepCycleEnded()
		{
			if (!gameObject.activeSelf)
				return;

			ApplyStepForce();
			m_LastFootDown = m_LastFootDown == 0 ? 1 : 0;
		}

		private void ApplyStepForce()
		{
			if (!gameObject.activeSelf)
				return;

			ForceMotionModule stepForce = null;

			if (IsWalking() || m_StateToVisualize == m_WalkState)
				stepForce = m_WalkStepForce;
			else if(m_Mover.ActiveMotions.Has(CharMotionMask.Crouch) || m_StateToVisualize == m_CrouchState)
				stepForce = m_CrouchStepForce;
			else if (m_Mover.ActiveMotions.Has(CharMotionMask.Run) || m_StateToVisualize == m_RunState)
				stepForce = m_RunStepForce;

			if (stepForce != null && stepForce.Enabled && m_CustomState == null)
			{
				m_PositionSpring.AddForce(stepForce.PositionForce.Force * 0.2f, stepForce.PositionForce.Distribution);
				m_RotationSpring.AddForce(stepForce.RotationForce.Force, stepForce.RotationForce.Distribution);
			}
		}

		private bool IsWalking() => !m_Mover.ActiveMotions.Has(CharMotionMask.Run) && m_Mover.Motor.Velocity.sqrMagnitude > 0.1f && m_Mover.Motor.IsGrounded;
	}
}