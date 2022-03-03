using SurvivalTemplatePro.Surfaces;
using Random = UnityEngine.Random;
using System.Collections;
using UnityEngine;
using System;
using SurvivalTemplatePro.InventorySystem;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SurvivalTemplatePro
{
    [RequireComponent(typeof(Collider), typeof(Rigidbody))]
	public class ShaftedProjectile : MonoBehaviour, IProjectile
	{
		#region Internal
		[Serializable]
		public class TwangSettings
		{
			public bool Enabled;

			public Vector3 MovementPivot;

			[Range(0f, 10f)]
			public float Duration = 1f;

			[Range(0f, 500f)]
			public float Range = 18f;

			public Vector2 RandomRotation;

			public SoundPlayer Audio;
		}
		#endregion

		public Rigidbody Rigidbody => m_Rigidbody;
		public Collider Collider => m_Collider;

		[SerializeField]
		private LayerMask m_Mask = (LayerMask)172545;

		[SerializeField]
		private Collider m_Collider;
		
		[SerializeField]
		private Rigidbody m_Rigidbody;

		[SerializeField]
		private TrailRenderer m_Trail;

		[BHeader("Damage")]

		[SerializeField, Range(0f, 200f)]
		private float m_MaxDamageSpeed = 50f;

		[SerializeField, Range(0f, 200f)]
		private float m_MaxDamage = 100f;

		[SerializeField]
		private AnimationCurve m_DamageCurve = new AnimationCurve(
			new Keyframe(0f, 1f),
			new Keyframe(0.8f, 0.5f),
			new Keyframe(1f, 0f));

		[SerializeField]
		private float m_ImpactForce = 15f;

		[BHeader("Penetration")]

		[SerializeField]
		private float m_PenetrationOffset = 0.2f;

		[SerializeField]
		private TwangSettings m_TwangSettings;

		[BHeader("Audio")]

		[SerializeField]
		private AudioSource m_AudioSource;

		[SerializeField]
		private SurfaceEffects m_PenetrationEffect;

		[Space]

		[SerializeField]
		private ItemPickup m_Pickup;

		private ICharacter m_Launcher;
		private bool m_Done;
		private bool m_Launched;

		private Transform m_TwangPivot;
		private Collider m_PenetratedSurface;

		private Vector3 m_PenetrationPositionOffset;
		private Quaternion m_PenetrationRotationOffset;


		public void Launch(ICharacter launcher)
		{
			if (m_Launcher != null)
			{
				Debug.LogWarningFormat(this, "Already launched this projectile!", name);
				return;
			}

			m_Launcher = launcher;
			m_Launched = true;

			if (m_Trail != null)
				m_Trail.emitting = true;

			if (m_Pickup != null)
				m_Pickup.InteractionEnabled = false;

			m_Collider.enabled = false;
		}

		public void AttachItem(Item itemToAttach) 
		{
			if (m_Pickup != null)
				m_Pickup.LinkWithItem(itemToAttach);
		}

		public void CheckForSurfaces(Vector3 position, Vector3 direction)
		{
			m_Rigidbody.rotation = Quaternion.LookRotation(m_Rigidbody.velocity);

			if (Physics.Raycast(position, direction, out RaycastHit hitInfo, 2f, m_Mask, QueryTriggerInteraction.Ignore) && !m_Launcher.HasCollider(hitInfo.collider))
			{
                // If the object is damageable...
                //zombilere hasar burada gelicek
                Debug.Log("okla zombi hasarı");
				if (hitInfo.collider.TryGetComponent(out IDamageReceiver receiver))
				{
					float currentSpeed = m_Rigidbody.velocity.magnitude;
					float damageMod = m_DamageCurve.Evaluate(1f - currentSpeed / m_MaxDamageSpeed);
					float damage = m_MaxDamage * damageMod;

					receiver.HandleDamage(new DamageInfo(-damage, DamageType.Stab, hitInfo.point, direction, m_ImpactForce, m_Launcher));
				}

				// If the object is a rigidbody, apply an impact force ( bi impact uyguluyo bunu belki kaldırmamız gerekebilir.)
				if (hitInfo.rigidbody != null)
					hitInfo.rigidbody.AddForceAtPosition(transform.forward * m_ImpactForce, hitInfo.point, ForceMode.Impulse);

				// Spawn impact effect
				SurfaceManager.SpawnEffect(hitInfo, m_PenetrationEffect, 1f);

				// Stick the projectile in the object (objeye yapışması)
				OnSurfacePenetrated(hitInfo);

				m_Done = true;
			}
		}

		public void OnSurfacePenetrated(RaycastHit hitInfo)
		{
			m_Rigidbody.isKinematic = true;
			m_Collider.enabled = true;

			Physics.IgnoreCollision(m_Collider, hitInfo.collider);

			transform.position = hitInfo.point + transform.forward * m_PenetrationOffset;

			if (m_Trail != null)
				m_Trail.emitting = false;

			m_PenetratedSurface = hitInfo.collider;
			m_PenetrationPositionOffset = m_PenetratedSurface.transform.InverseTransformPoint(transform.position);
			m_PenetrationRotationOffset = Quaternion.Inverse(m_PenetratedSurface.transform.rotation) * transform.rotation;

			MatchWithPenetratedSurface();

			if (m_TwangSettings.Enabled)
				StartCoroutine(C_DoTwang());

			if (m_Pickup != null)
				m_Pickup.InteractionEnabled = true;
		}

		private void Awake()
		{
			m_Collider = GetComponent<Collider>();
			m_Rigidbody = GetComponent<Rigidbody>();

			if (m_Trail != null)
				m_Trail.emitting = false;
		}

		private void FixedUpdate()
		{
			if (m_Launched && !m_Done)
				CheckForSurfaces(transform.position, transform.forward);

			if (m_Done)
				MatchWithPenetratedSurface();
		}

		private void MatchWithPenetratedSurface()
		{
			if (m_PenetratedSurface != null && m_PenetratedSurface.enabled)
			{
				transform.position = m_PenetratedSurface.transform.position + m_PenetratedSurface.transform.TransformVector(m_PenetrationPositionOffset);
				transform.rotation = m_PenetratedSurface.transform.rotation * m_PenetrationRotationOffset;
			}
			else
			{
				m_PenetratedSurface = null;
				m_Collider.enabled = false;
				m_Rigidbody.isKinematic = false;
				m_Done = false;
			}
		}
        //okun çarptıktan sonra sallanması.
		private IEnumerator C_DoTwang()
		{
			m_TwangPivot = new GameObject("Shafted Projectile Pivot").transform;
			m_TwangPivot.position = transform.position + m_TwangSettings.MovementPivot.LocalToWorld(transform);
			m_TwangPivot.rotation = transform.rotation;

			var previousParent = transform.parent;

			if (previousParent != null)
				m_TwangPivot.SetParent(previousParent, true);

			transform.SetParent(m_TwangPivot, true);

			float stopTime = Time.time + m_TwangSettings.Duration;
			float range = m_TwangSettings.Range;
			float currentVelocity = 0f;

			m_TwangSettings.Audio.Play(m_AudioSource);

			Quaternion localRotation = m_TwangPivot.localRotation;

			Vector2 randomRotationRange = m_TwangSettings.RandomRotation;

			Quaternion randomRotation = Quaternion.Euler(new Vector2(
				Random.Range(-randomRotationRange.x, randomRotationRange.x), 
				Random.Range(-randomRotationRange.y, randomRotationRange.y)));

			while (Time.time < stopTime)
			{
				m_TwangPivot.localRotation = localRotation * randomRotation * Quaternion.Euler(Random.Range(-range, range), Random.Range(-range, range), 0f);
				range = Mathf.SmoothDamp(range, 0f, ref currentVelocity, stopTime - Time.time);

				yield return null;
			}
		}

		private void OnDestroy()
		{
			if (m_TwangPivot != null)
				Destroy(m_TwangPivot.gameObject);
		}

		#if UNITY_EDITOR
		private void OnDrawGizmosSelected()
		{
			if (m_TwangSettings.Enabled)
			{
				Vector3 twangPivotPosition = transform.position + transform.TransformVector(m_TwangSettings.MovementPivot);

				Gizmos.color = new Color(1f, 0f, 0f, 0.85f);
				Gizmos.DrawSphere(twangPivotPosition, 0.03f);

						Vector3 sceneCamPosition = SceneView.currentDrawingSceneView.camera.transform.position;
				Vector3 sceneCamForward = SceneView.currentDrawingSceneView.camera.transform.forward;

				// Make sure we don't draw the label when not looking at it
				if (Vector3.Dot(sceneCamForward, twangPivotPosition - sceneCamPosition) >= 0f)
					Handles.Label(twangPivotPosition, "Twang Pivot");
			}
		}

        private void OnValidate()
        {
            if (m_Collider == null)
				m_Collider = GetComponent<Collider>();

			if (m_Rigidbody == null)
				m_Rigidbody = GetComponent<Rigidbody>();

			if (m_Trail == null)
				m_Trail = GetComponent<TrailRenderer>();
		}
#endif
    }
}