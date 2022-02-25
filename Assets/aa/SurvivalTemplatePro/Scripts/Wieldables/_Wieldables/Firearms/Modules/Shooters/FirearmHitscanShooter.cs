using SurvivalTemplatePro.Surfaces;
using UnityEngine;

namespace SurvivalTemplatePro.WieldableSystem
{
    public class FirearmHitscanShooter : FirearmShooterBehaviour
    {
        #region Internal
        [System.Serializable]
        public class RayImpact
        {
            [SerializeField, Range(0f, 1000f)]
            [Tooltip("The damage at close range.")]
            private float m_MaxDamage = 15f;

            [SerializeField, Range(0f, 1000f)]
            [Tooltip("The impact impulse that will be transfered to the rigidbodies at contact.")]
            private float m_MaxImpulse = 15f;

            [SerializeField]
            [Tooltip("How damage and impulse lowers over distance.")]
            private AnimationCurve m_DistanceCurve = new AnimationCurve(
                new Keyframe(0f, 1f),
                new Keyframe(0.8f, 0.5f),
                new Keyframe(1f, 0f));


            /// <returns> Damage at specified distance.</returns>
            public float GetDamageAtDistance(float distance, float maxDistance) => ApplyCurveToValue(m_MaxDamage, distance, maxDistance);

            /// <returns> Impulse at specified distance.</returns>
            public float GetImpulseAtDistance(float distance, float maxDistance) => ApplyCurveToValue(m_MaxImpulse, distance, maxDistance);

            private float ApplyCurveToValue(float value, float distance, float maxDistance)
            {
                float maxDistanceAbsolute = Mathf.Abs(maxDistance);
                float distanceClamped = Mathf.Clamp(distance, 0f, maxDistanceAbsolute);

                return value * m_DistanceCurve.Evaluate(distanceClamped / maxDistanceAbsolute);
            }
        }
        #endregion

        public override int AmmoPerShot => m_AmmoPerShot;

        [BHeader("Ammo")]

        [SerializeField, Range(0, 10)]
        private int m_AmmoPerShot = 1;

        [BHeader("Ray")]

        [SerializeField, Tooltip("The layers that will be affected when you fire.")]
        private LayerMask m_RayMask = (LayerMask)172545;

        [SerializeField]
        private DamageType m_DamageType = DamageType.Bullet;

        [SerializeField, Range(1, 30)]
        [Tooltip("The amount of rays that will be sent in the world")]
        private int m_RayCount = 1;

        [SerializeField, Range(0f, 100f)]
        private float m_RaySpread = 1f;

        [SerializeField, Range(0f, 1000f)]
        private float m_MaxDistance = 100f;

        [SerializeField]
        private RayImpact m_RayImpact;

        [BHeader("Effects")]

        [SerializeField]
        private WieldableEffectBehaviour m_TracerEffect;

        [SerializeField]
        private WieldableEffectBehaviour m_CasingEffect;

        [SerializeField]
        private WieldableEffectBehaviour m_MuzzleFlashEffect;

        [BHeader("Audio")]

        [SerializeField]
        private StandardSound m_ShootAudio;

        [SerializeField]
        private DelayedSoundRandom[] m_HandlingAudio;

        [BHeader("Local Effects")]

        [SerializeField, WieldableEffect]
        private int[] m_ShootEffects;

        // TODO: Implement
        //[SerializeField] 
        //[Tooltip("How the bullet spread will transform (in continuous use) on the duration of the magazine, the max x value(1) will be used if the whole magazine has been used")]
        //private AnimationCurve m_SpreadOverTime = new AnimationCurve(new Keyframe(0f, .8f), new Keyframe(1f, 1f));


        public override void Shoot(float value)
        {
            var character = Firearm.Character;

            for (int i = 0; i < m_RayCount; i++)
            {
                Ray ray = Firearm.GetShootRay(m_RaySpread);

                DoHitscan(Firearm.Character, ray);

                if (m_TracerEffect != null)
                    m_TracerEffect.DoEffect(character);
            }

            // Audio
            var audioPlayer = Firearm.AudioPlayer;
            audioPlayer.PlaySound(m_ShootAudio);
            audioPlayer.PlaySounds(m_HandlingAudio);

            // Shoot Effects
            if (m_CasingEffect != null)
                m_CasingEffect.DoEffect(character);

            if (m_MuzzleFlashEffect != null)
                m_MuzzleFlashEffect.DoEffect(character);

            // Local Effects
            EventManager.PlayEffects(m_ShootEffects, 1f);
        }

        private void DoHitscan(ICharacter user, Ray ray)
        {
            if (Physics.Raycast(ray, out RaycastHit hitInfo, m_MaxDistance, m_RayMask, QueryTriggerInteraction.Ignore))
            {
                bool isDynamicObject = false;

                float impulse = m_RayImpact.GetImpulseAtDistance(hitInfo.distance, m_MaxDistance);

                // Apply an impact impulse
                if (hitInfo.rigidbody != null)
                {
                    hitInfo.rigidbody.AddForceAtPosition(ray.direction * impulse, hitInfo.point, ForceMode.Impulse);
                    isDynamicObject = true;
                }

                // Do damage
                float damage = m_RayImpact.GetDamageAtDistance(hitInfo.distance, m_MaxDistance);

                if (hitInfo.collider.TryGetComponent(out IDamageReceiver receiver))
                    receiver.HandleDamage(new DamageInfo(-damage, m_DamageType, hitInfo.point, ray.direction, impulse, hitInfo.normal, user));

                SurfaceManager.SpawnEffect(hitInfo, SurfaceEffects.BulletHit, 1f, isDynamicObject);
            }
        }
    }
}
