using UnityEngine;

namespace SurvivalTemplatePro
{
	#region Internal
	[System.Serializable]
	public class ObjectDropSettings
	{
		public Vector3 PositionOffset = Vector3.zero;
		public Vector3 RotationOffset = Vector3.zero;

		[Range(0f, 1f)]
		public float RotationRandomness = 0.3f;

		[Range(0f, 100f)]
		public float DropSpeed = 8f;

		[Range(0f, 100f)]
		public float CharacterVelocityFactor = 0.3f;

		[Range(0f, 1000f)]
		public float DropAngularFactor = 150f;

		[Space]

		public StandardSound DropAudio;
	}
    #endregion

    public abstract class ObjectDropHandler : CharacterBehaviour
    {
		[SerializeField]
		[Tooltip("The layer mask that will be used in checking for obstacles when items are dropped.")]
		private LayerMask m_DropObstacleMask = new LayerMask();

		[SerializeField]
		[Tooltip("Dropped items are spawned relative to this transform")]
		private Transform m_DropTransform;

		[SerializeField]
		[Tooltip("The force mode that will be added to the rigidbody of the dropped item.")]
		private ForceMode m_DropForceMode;


		public GameObject DropObject(ObjectDropSettings dropSettings, GameObject objectToDrop, float dropHeightMod)
		{
			if (objectToDrop == null)
				return null;

			Vector3 dropOffset;
			Quaternion dropRotation = Quaternion.Lerp(Quaternion.LookRotation(m_DropTransform.forward) * Quaternion.Euler(dropSettings.RotationOffset), Random.rotationUniform, dropSettings.RotationRandomness);
			bool dropNearObstacle = false;

			// Get pickup spawn offsets
			if (Physics.Raycast(m_DropTransform.position, m_DropTransform.forward, dropSettings.PositionOffset.z * 1.1f, m_DropObstacleMask))
			{
				dropOffset = m_DropTransform.position + Character.transform.TransformVector(new Vector3(0f, dropSettings.PositionOffset.y * dropHeightMod, -0.2f));
				dropNearObstacle = true;
			}
			else
			{
				dropOffset = m_DropTransform.position + m_DropTransform.TransformVector(new Vector3(dropSettings.PositionOffset.x, dropSettings.PositionOffset.y * dropHeightMod, dropSettings.PositionOffset.z));
			}

			// Spawn the pickup
			GameObject droppedObject = Instantiate(objectToDrop, dropOffset, dropRotation);

			// Setup the rigidbody of the pickup
			if (!dropNearObstacle)
			{
				if (droppedObject.TryGetComponent(out Rigidbody rigidbody))
				{
					rigidbody.isKinematic = false;

					Vector3 velocity = Character.Mover.Motor.Velocity * dropSettings.CharacterVelocityFactor;
					velocity = new Vector3(velocity.x, Mathf.Abs(velocity.y), velocity.z);

					Vector3 forceVector = m_DropTransform.forward * dropSettings.DropSpeed + velocity;
					Vector3 torqueVector = dropRotation.eulerAngles * dropSettings.DropAngularFactor;

					rigidbody.AddForce(forceVector, m_DropForceMode);
					rigidbody.AddTorque(torqueVector);
				}
			}

			// Play the drop audio
			Character.AudioPlayer.PlaySound(dropSettings.DropAudio);

			return droppedObject;
		}
    }
}