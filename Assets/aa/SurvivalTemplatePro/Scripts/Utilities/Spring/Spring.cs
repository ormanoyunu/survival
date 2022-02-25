using System;
using UnityEngine;

namespace SurvivalTemplatePro
{
    public class Sprind
    {
		#region Internal
		public enum RefreshType
		{
			OverridePosition,
			AddToPosition,
			OverrideRotation,
			AddToRotation
		}

		[Serializable]
		public struct Settings
		{
			public static Settings Default { get { return new Settings() { Stiffness = Vector3.one * 90f, Damping = Vector3.one * 10f }; } }

			public Vector3 Stiffness;
			public Vector3 Damping;
		}
		#endregion

		public Vector3 Stiffness => m_Stiffness;
		public Vector3 Position => m_Position;

		private readonly RefreshType m_Type;
		private readonly Transform m_Transform;
		private readonly float m_LerpSpeed = 10f;

		private float m_FixedDeltaTime;

		private Vector3 m_Stiffness;
		private Vector3 m_Damping;

		private Vector3 m_TargetPosition;
		private Vector3 m_Position;
		private Vector3 m_RestPosition;
		private Vector3 m_Velocity;

		private readonly Vector3[] m_DistributedForce = new Vector3[30];


		public Sprind(RefreshType type, Transform transform, Vector3 restVector = default, float lerpSpeed = 10f, Settings data = default)
		{
			m_Type = type;
			m_Transform = transform;
			m_LerpSpeed = lerpSpeed;

			m_RestPosition = restVector;

			m_Stiffness = data.Stiffness;
			m_Damping = data.Damping;
		}

		public void Reset()
		{
			m_TargetPosition = Vector3.zero;
			m_Velocity = Vector3.zero;

			UpdateSpring();

			for (int i = 0;i < 30;i++)
				m_DistributedForce[i] = Vector3.zero;
		}

		public void Adjust(Vector3 stiffness, Vector3 damping)
		{
			m_Stiffness = stiffness;
			m_Damping = damping;
		}

		public void Adjust(Settings data)
		{
			m_Stiffness = data.Stiffness;
			m_Damping = data.Damping;
		}

		public void FixedUpdate(float deltaTime) 
		{
			m_FixedDeltaTime = deltaTime;

			// Handle distributed forces.
			if (m_DistributedForce[0] != Vector3.zero)
			{
				AddForce(m_DistributedForce[0]);

				for (int i = 0; i < 30; i++)
				{
					m_DistributedForce[i] = i < 29 ? m_DistributedForce[i + 1] : Vector3.zero;
					if (m_DistributedForce[i] == Vector3.zero)
						break;
				}
			}

			UpdateSpring();
			UpdatePosition();
		}

		public void Update(float deltaTime)
		{
			if (m_LerpSpeed > 0f)
				m_Position = Vector3.Lerp(m_Position, m_TargetPosition, deltaTime * m_LerpSpeed);
			else
				m_Position = m_TargetPosition;

			UpdateTransform();
		}

		public void AddForce(SpringForce force)
		{
			if (force.Distribution > 1)
				AddDistributedForce(force.Force, force.Distribution);
			else
				AddForce(force.Force);
		}

		public void AddForce(Vector3 forceVector)
		{
			m_Velocity += forceVector;

			UpdatePosition();
		}

		public void AddForce(Vector3 forceVector, int distribution)
		{
			if(distribution > 1)
				AddDistributedForce(forceVector, distribution);
			else
				AddForce(forceVector);
		}

		public void AddDistributedForce(Vector3 force, int distribution)
		{
			distribution = Mathf.Clamp(distribution, 1, 20);

			AddForce(force / distribution);

			for(int i = 0;i < Mathf.RoundToInt(distribution) - 1;i++)
				m_DistributedForce[i] += force / distribution;
		}

		private void UpdateSpring()
		{
			m_Velocity += Vector3.Scale((m_RestPosition - m_TargetPosition), m_Stiffness * m_FixedDeltaTime);
			m_Velocity = Vector3.Scale(m_Velocity, Vector3.one - m_Damping * m_FixedDeltaTime);
		}

		private void UpdatePosition()
		{
			m_TargetPosition = m_TargetPosition + m_Velocity * m_FixedDeltaTime;

			if (float.IsNaN(m_TargetPosition.x))
				m_TargetPosition.x = 0f;

			if (float.IsNaN(m_TargetPosition.y))
				m_TargetPosition.y = 0f;

			if (float.IsNaN(m_TargetPosition.z))
				m_TargetPosition.z = 0f;
		}

		private void UpdateTransform()
		{
            switch (m_Type)
            {
                case RefreshType.OverridePosition:
					m_Transform.localPosition = m_Position;
					break;
                case RefreshType.AddToPosition:
					m_Transform.localPosition += m_Position;
					break;
                case RefreshType.OverrideRotation:
					m_Transform.localEulerAngles = m_Position;
					break;
                case RefreshType.AddToRotation:
					m_Transform.localEulerAngles += m_Position;
					break;
            }
        }
    }
}