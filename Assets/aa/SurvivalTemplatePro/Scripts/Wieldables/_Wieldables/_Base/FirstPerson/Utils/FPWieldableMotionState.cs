using UnityEngine;
using System;

namespace SurvivalTemplatePro.WieldableSystem
{
    [Serializable]
	public class FPWieldableMotionState
	{
		#region Internal
		[Serializable]
		public class OffsetModule
		{
			public bool Enabled = true;

			[Space(3f)]

			public Vector3 PositionOffset;
			public Vector3 RotationOffset;
		}

		[Serializable]
		public class EntryOffsetModule
		{
			public bool Enabled;

			[Space(3f)]

			public float Duration = 1f;
			public float LerpSpeed = 4f;

			public OffsetModule Offset;
		}

		[Serializable]
		public class BobModule
		{
			public bool Enabled = true;

			[Space(3f)]

			public Vector3 PositionAmplitude;
			public Vector3 RotationAmplitude;
		}
		#endregion

		public Sprind.Settings PositionSpring = Sprind.Settings.Default;
		public Sprind.Settings RotationSpring = Sprind.Settings.Default;

		[Space]

		public OffsetModule Offset;
		public BobModule Bob;
		public NoiseMotionModule Noise;

		[BHeader("State Change Forces")]

		public SpringForce EnterForce;
		public SpringForce ExitForce;
    }
}