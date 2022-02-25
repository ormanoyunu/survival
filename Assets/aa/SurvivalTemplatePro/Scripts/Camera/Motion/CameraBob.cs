using System;
using UnityEngine;

namespace SurvivalTemplatePro.CameraSystem
{
    [Serializable]
	public class CameraBob
	{
		public Vector3 PositionAmplitude => m_PositionAmplitude;
		public Vector3 RotationAmplitude => m_RotationAmplitude;

		[SerializeField]
		private Vector3 m_PositionAmplitude;

		[SerializeField]
		private Vector3 m_RotationAmplitude;
	}
}