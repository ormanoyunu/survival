using SurvivalTemplatePro.Surfaces;
using System.Collections;
using UnityEngine;

namespace SurvivalTemplatePro.WieldableSystem
{
    public class BasicMeleeSwing : MeleeSwingBehaviour
	{
		public override float SwingDuration => m_AttackThreeshold;
		public override float AttackEffort => m_AttackEffort;

		[BHeader("Attack")]

		[SerializeField]
		protected DamageType m_DamageType = DamageType.Hit;

		[SerializeField, Range(0f, 3f)]
		protected float m_AttackThreeshold = 0.3f;

		[SerializeField, Range(0f, 5f)]
		protected float m_AttackDelay = 0.2f;

		[SerializeField, Range(0f, 1f)]
		protected float m_AttackEffort = 0.05f;

		[BHeader("Object Detection")]

		[SerializeField]
		protected LayerMask m_HitMask = (LayerMask)172545;

		[SerializeField]
		protected Vector3 m_HitOffset = Vector3.zero;

		[SerializeField, Range(0f, 1f)]
		protected float m_HitRadius = 0.1f;

		[SerializeField, Range(0f, 5f)]
		protected float m_HitDistance = 0.5f;

		[BHeader("Impact")]

		[SerializeField, Range(0f, 100f)]
		protected float m_Damage = 15f;

		[SerializeField, Range(0f, 100f)]
		protected float m_ImpactForce = 30f;

		[SerializeField, Range(0f, 100f)]
		protected float m_DurabilityRemove = 2f;

		[BHeader("Audio")]

		[SerializeField]
		protected DelayedSoundRandom m_SwingAudio;

		[SerializeField]
		protected DelayedSoundRandom m_HitAudio;

		[BHeader("Local Effects")]

		[SerializeField, WieldableEffect]
		protected int[] m_SwingEffects;

		[SerializeField, WieldableEffect]
		protected int[] m_SwingHitEffects;


		public override bool CanSwing() => true;

		public override void DoSwing(ICharacter user)
		{
			StartCoroutine(C_HitCheckDelayed(user));

			// Swing Audio
			Wieldable.AudioPlayer.PlaySound(m_SwingAudio);

			// Local Effects
			Wieldable.EventManager.PlayEffects(m_SwingEffects, 1f);
		}

		protected IEnumerator C_HitCheckDelayed(ICharacter user)
		{
			float timer = Time.time + m_AttackDelay;
			while (timer > Time.time)
				yield return null;

			DoHitCheck(user, GetUseRay());
		}

		protected virtual RaycastHit DoHitCheck(ICharacter user, Ray ray)
		{
			if (SphereCast(user, ray, m_HitRadius, m_HitDistance, out RaycastHit hitInfo))
			{
				bool isDynamicObject = false;

				// Apply an impact impulse
				if (hitInfo.rigidbody != null)
				{
					hitInfo.rigidbody.AddForceAtPosition(ray.direction * m_ImpactForce, hitInfo.point, ForceMode.Impulse);
					isDynamicObject = true;
				}

				if (hitInfo.collider.TryGetComponent(out IDamageReceiver receiver))
					receiver.HandleDamage(new DamageInfo(-m_Damage, m_DamageType, hitInfo.point, ray.direction, m_ImpactForce, user));

				// Surface effect
				SurfaceManager.SpawnEffect(hitInfo, SurfaceEffects.Slash, 1f, isDynamicObject);

				// Hit Audio
				Wieldable.AudioPlayer.PlaySound(m_HitAudio);

				ConsumeItemDurability(m_DurabilityRemove);

				// Local Effects
				Wieldable.EventManager.PlayEffects(m_SwingHitEffects, 1f);
			}

			return hitInfo;
		}

		protected Ray GetUseRay() => Wieldable.RayGenerator.GenerateRay(1f, m_HitOffset);

		protected bool SphereCast(ICharacter user, Ray ray, float radius, float distance, out RaycastHit hitInfo)
		{
			RaycastHit[] hits = Physics.SphereCastAll(ray, radius, distance, m_HitMask, QueryTriggerInteraction.Ignore);

			int closestHit = -1;
			float closestDistance = Mathf.Infinity;

			for (int i = 0; i < hits.Length; i++)
			{
				if (!user.HasCollider(hits[i].collider))
				{
					if (hits[i].distance < closestDistance)
					{
						closestDistance = hits[i].distance;
						closestHit = i;
					}
				}
			}

			if (closestHit != -1)
			{
				hitInfo = hits[closestHit];
				return true;
			}
			else
			{
				hitInfo = default;
				return false;
			}
		}
    }
}