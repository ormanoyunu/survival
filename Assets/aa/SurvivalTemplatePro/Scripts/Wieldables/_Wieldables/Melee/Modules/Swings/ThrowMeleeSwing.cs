using System.Collections;
using UnityEngine;

namespace SurvivalTemplatePro.WieldableSystem
{
    public class ThrowMeleeSwing : MeleeSwingBehaviour
	{
		[SerializeField]
		private ShaftedProjectile m_Projectile = null;

		[Space]

		[SerializeField, Range(0f, 100f)]
		private float m_SpawnDirectionSpread = 1f;

		[SerializeField, Range(0f, 5f)]
		[Tooltip("Time to spawn the projectile")]
		private float m_SpawnDelay = 1f;

		[SerializeField]
		private Vector3 m_SpawnPositionOffset = Vector3.zero;

		[SerializeField]
		private Vector3 m_SpawnRotationOffset = Vector3.zero;

		[SerializeField, Range(0f, 100f)]
		private float m_ThrowVelocity = 50f;

		[SerializeField, Range(0f, 100f)]
		private float m_ThrowTorque = 0f;

		[Space]

		[SerializeField, Range(0f, 100f)]
		private float m_DurabilityRemove = 2f;

		[SerializeField]
		private bool m_RemoveFromInventory = true;

		[Space]

		[SerializeField]
		private Transform[] m_ObjectsToDisableOnThrow;

		[BHeader("Audio")]

		[SerializeField]
		private DelayedSound[] m_ThrowAudio = null;

		[BHeader("Local Effects")]

		[SerializeField, WieldableEffect]
		private int[] m_ThrowStartEffects;

		[SerializeField, WieldableEffect]
		private int[] m_ThrowEffects;

		private IAimHandler m_AimHandler;


        public override bool CanSwing() => m_AimHandler != null && m_AimHandler.IsAiming;

        public override void DoSwing(ICharacter user)
        {
			// Force End Aiming
			m_AimHandler.EndAiming();

			StartCoroutine(C_SpawnWithDelay(user));

			// Play throw audio
			Wieldable.AudioPlayer.PlaySounds(m_ThrowAudio);

			// Local Effects
			Wieldable.EventManager.PlayEffects(m_ThrowStartEffects, 1f);
		}

        private IEnumerator C_SpawnWithDelay(ICharacter user)
		{
			yield return new WaitForSeconds(m_SpawnDelay);

			ThrowPolearm(user, Wieldable.RayGenerator.GenerateRay(m_SpawnDirectionSpread));

			// Remove Item from the user's inventory
			if (m_RemoveFromInventory)
				user.Inventory.RemoveItem(Wieldable.AttachedItem);
		}

		private void ThrowPolearm(ICharacter user, Ray ray)
		{
			Vector3 position = ray.origin + user.View.TransformVector(m_SpawnPositionOffset);
			Quaternion rotation = Quaternion.LookRotation(ray.direction) * Quaternion.Euler(m_SpawnRotationOffset);

			ShaftedProjectile projectile = Instantiate(m_Projectile, position, rotation);

			// Launch the projectile...
			if (projectile != null)
			{
				projectile.Rigidbody.velocity = (projectile.transform.forward * m_ThrowVelocity) + user.Mover.Motor.Velocity;
				projectile.Rigidbody.angularVelocity = Random.onUnitSphere * m_ThrowTorque;

				projectile.Launch(user);
				projectile.AttachItem(Wieldable.AttachedItem);
				projectile.CheckForSurfaces(ray.origin, ray.direction);
			}

			// Disable Objects
			for (int i = 0; i < m_ObjectsToDisableOnThrow.Length; i++)
				m_ObjectsToDisableOnThrow[i].localScale = Vector3.zero;

			// Consume durability
			ConsumeItemDurability(m_DurabilityRemove);

			// Local Effects
			Wieldable.EventManager.PlayEffects(m_ThrowEffects, 1f);
		}

		protected override void Awake() 
		{
			base.Awake();

			m_AimHandler = GetComponent<IAimHandler>();
            Wieldable.onEquippingStarted += OnEquipStart;
		}

        private void OnEquipStart()
        {
            for (int i = 0; i < m_ObjectsToDisableOnThrow.Length; i++)
				m_ObjectsToDisableOnThrow[i].localScale = Vector3.one;
		}
    }
}