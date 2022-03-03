using SurvivalTemplatePro.ResourceGathering;
using SurvivalTemplatePro.Surfaces;
using UnityEngine;

namespace SurvivalTemplatePro.WieldableSystem
{
	public class TreeChopMeleeSwing : BasicMeleeSwing
    {
		[BHeader("Tree Chop")]

		[SerializeField, Range(0f, 10f)]
		private float m_MaxChopDistance = 0.35f;


		public override bool CanSwing()
		{
			if (SphereCast(Wieldable.Character, GetUseRay(), m_HitRadius, m_HitDistance, out RaycastHit hitInfo))
				return CheckAngleWithTree(hitInfo.collider.GetComponent<IGatherable>());

			return false;
		}

		protected override RaycastHit DoHitCheck(ICharacter user, Ray ray)
		{
			if (SphereCast(user, ray, m_HitRadius, m_HitDistance, out RaycastHit hitInfo))
			{
				if (hitInfo.collider.TryGetComponent(out IGatherable gatherable) && CheckAngleWithTree(gatherable))
				{
                    Debug.Log("ağaç şeyi");
                    //Debug.Log(hitInfo.transform.parent);
					DamageInfo treeDamage = new DamageInfo(11.11f, ray.origin + ray.direction * 0.5f + Vector3.Cross(Vector3.up, ray.direction) * 0.25f, ray.direction, m_ImpactForce);
					gatherable.Damage(treeDamage);

					// Spawn some effects for the chop impact
					SurfaceManager.SpawnEffect(hitInfo, SurfaceEffects.Stab, 1f);

					// TODO: Spawn custom effect
					// Vector3 chopImpactPos = GetChopPoint(m_ProximityTree) + Vector3.Cross(Vector3.up, ray.direction) * 0.15f;
					// SurfaceManager.SpawnEffect(chopImpactPos, chopImpactPos, Quaternion.LookRotation(Vector3.Cross(hitInfo.normal, Vector3.up)));

					// Hit Audio
					Wieldable.AudioPlayer.PlaySound(m_HitAudio);

					ConsumeItemDurability(m_DurabilityRemove);

					// Local Effects
					Wieldable.EventManager.PlayEffects(m_SwingHitEffects, 1f);

                    return hitInfo;
				}

                Debug.Log(hitInfo.collider.name + "istediğimiz yerden");
                Debug.Log("a");

            }

			return hitInfo;
		}

		private bool CheckAngleWithTree(IGatherable gatherable)
		{
			if (gatherable != null)
			{
				var tree = gatherable.transform;
				Ray useRay = GetUseRay();

				return Mathf.Abs((useRay.origin + useRay.direction).y - (tree.position + gatherable.GatherOffset).y) < m_MaxChopDistance;
			}

			return false;
		}
	}
}