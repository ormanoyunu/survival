using UnityEngine;

namespace SurvivalTemplatePro.WieldableSystem
{
    public class WieldableRigidbodyEffect : WieldableObjectSpawnEffect
	{
        private enum VelocityAlignment { Local, View }

		[BHeader("Rigidbody")]

		[SerializeField]
		private VelocityAlignment m_VelocityAlignment = VelocityAlignment.Local;

		[SerializeField, Range(0, 100f)]
		private float m_Speed = 2f;

		[SerializeField]
		private Vector3 m_VelocityJitter = Vector3.one * 0.2f;

		[SerializeField, Range(0,100f)]
		private float m_Spin = 0.3f;


        protected override PoolableObject SpawnEffect()
        {
            var obj = base.SpawnEffect();

			if (obj.gameObject.TryGetComponent(out Rigidbody objRb))
			{
				Vector3 velocityJitter = new Vector3(Random.Range(-m_VelocityJitter.x, m_VelocityJitter.x), Random.Range(-m_VelocityJitter.y, m_VelocityJitter.y), Random.Range(-m_VelocityJitter.z, m_VelocityJitter.z));

				Transform rotationParent = m_VelocityAlignment == VelocityAlignment.Local ? transform : m_Character.View;
				Vector3 velocity = (m_Character.Mover.Motor.Velocity * 0.1f) + rotationParent.TransformVector(Vector3.forward * m_Speed + velocityJitter);

				float spinDirection = Random.Range(0, 2) == 0 ? 1f : -1f;
				float spinAmount = Random.Range(0.7f, 1.3f) * m_Spin;

				objRb.velocity = velocity;
				objRb.maxAngularVelocity = 10000f;
				objRb.angularVelocity = Vector3.one * spinAmount * spinDirection;
			}

			return obj;
		}
    }
}