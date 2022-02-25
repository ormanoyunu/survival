using UnityEngine;
using System;

namespace SurvivalTemplatePro
{
	[Serializable]
	public class InputMotionModule
	{
		[Range(0.01f, 2f)]
		public float LookInputMultiplier = 1f;

		[Range(0f, 1000f)]
		public float MaxLookInput = 20f;
	}

	[Serializable]
	public class SwayMotionModule
	{
		public bool Enabled = true;

		[Space]

		public Vector3 LookPositionSway;
		public Vector3 LookRotationSway;

		public Vector2 FallSway;

		[Range(0f, 100f)]
		public float MaxFallSway = 25f;

		[Space]

		public Vector3 StrafePositionSway;
		public Vector3 StrafeRotationSway;
	}

	[Serializable]
	public class ForceMotionModule
	{
		public bool Enabled = true;

		[Space(3f)]

		public SpringForce PositionForce;
		public SpringForce RotationForce;
	}

	[Serializable]
	public class NoiseMotionModule
	{
		public bool Enabled;

		[Space(3f)]

		[Range(0f, 1f)]
		public float MaxJitter = 0f;

		[Range(0f, 5f)]
		public float NoiseSpeed = 1f;

		public Vector3 PositionAmplitude;
		public Vector3 RotationAmplitude;
	}
}
