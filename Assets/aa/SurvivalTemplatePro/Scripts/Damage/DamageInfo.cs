using UnityEngine;

namespace SurvivalTemplatePro
{
    /// <summary>
    /// 
    /// </summary>
    public struct DamageInfo
	{
		/// <summary> </summary>
		public float Damage { get; set; }

		/// <summary> </summary>
		public Hitbox Hitbox { get; set; }

		/// <summary> </summary>
		public ICharacter Source { get; }

		public DamageType DamageType { get; }

		/// <summary> </summary>
		public Vector3 HitPoint { get; }

		/// <summary> </summary>
		public Vector3 HitDirection { get; }

		/// <summary> </summary>
		public float HitImpulse { get; }

		/// <summary> </summary>
		public Vector3 HitNormal { get; }


		public DamageInfo(float damage, ICharacter source = null, Hitbox hitbox = null)
		{
			Damage = Mathf.Abs(damage);

			DamageType = DamageType.Generic;

			HitPoint = Vector3.zero;
			HitDirection = Vector3.zero;
			HitImpulse = 0f;
			HitNormal = Vector3.zero;

			Source = source;
			Hitbox = hitbox;
		}

		public DamageInfo(float damage, DamageType damageType, ICharacter source = null, Hitbox hitbox = null)
		{
			Damage = Mathf.Abs(damage);

			DamageType = damageType;

			HitPoint = Vector3.zero;
			HitDirection = Vector3.zero;
			HitImpulse = 0f;

			HitNormal = Vector3.zero;

			Source = source;
			Hitbox = hitbox;
		}

		public DamageInfo(float damage, Vector3 hitPoint, Vector3 hitDirection, float hitImpulse, ICharacter source = null, Hitbox hitbox = null)
		{
			Damage = Mathf.Abs(damage);

			DamageType = DamageType.Generic;

			HitPoint = hitPoint;
			HitDirection = hitDirection;
			HitImpulse = hitImpulse;

			HitNormal = Vector3.zero;

			Source = source;
			Hitbox = hitbox;
		}

		public DamageInfo(float damage, DamageType damageType, Vector3 hitPoint, Vector3 hitDirection, float hitImpulse, ICharacter source = null, Hitbox hitbox = null)
		{
			Damage = Mathf.Abs(damage);

			DamageType = damageType;

			HitPoint = hitPoint;
			HitDirection = hitDirection;
			HitImpulse = hitImpulse;

			HitNormal = Vector3.zero;

			Source = source;
			Hitbox = hitbox;
		}

		public DamageInfo(float damage, Vector3 hitPoint, Vector3 hitDirection, float hitImpulse, Vector3 hitNormal, ICharacter source = null, Hitbox hitbox = null)
		{
			Damage = Mathf.Abs(damage);

			DamageType = DamageType.Generic;

			HitPoint = hitPoint;
			HitDirection = hitDirection;
			HitImpulse = hitImpulse;

			HitNormal = hitNormal;

			Source = source;
			Hitbox = hitbox;
		}

		public DamageInfo(float damage, DamageType damageType, Vector3 hitPoint, Vector3 hitDirection, float hitImpulse, Vector3 hitNormal, ICharacter source = null, Hitbox hitbox = null)
		{
			Damage = Mathf.Abs(damage);

			DamageType = damageType;

			HitPoint = hitPoint;
			HitDirection = hitDirection;
			HitImpulse = hitImpulse;

			HitNormal = hitNormal;

			Source = source;
			Hitbox = hitbox;
		}
	}

	public enum DamageType
	{
		Generic,
		Cut,
		Hit,
		Stab,
		Bullet
	}
}