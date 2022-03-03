using System.Collections.Generic;
using UnityEngine;

namespace SurvivalTemplatePro.ResourceGathering
{
    [RequireComponent(typeof(Collider), typeof(AudioSource))]
    public class Gatherable : MonoBehaviour, IGatherable
    {
        public GatherableDefinition Definition => m_Definition;
        public float Health => m_Health;
        public float MaxHealth => m_MaxHealth;
        public float GatherRadius => m_GatherRadius;
        public Vector3 GatherOffset => m_GatherOffset;

        [SerializeField]
        private GatherableDefinition m_Definition;

        [SerializeField, InfoBox("Prefab to SPAWN when a character interacts with this gatherable.")]
        private GatherableBehaviour m_RiggedPrefab;

        [Space]

        [SerializeField, InfoBox("Visuals to DISABLE when a character interacts with this gatherable.")]
        private GameObject m_BaseVisuals;

        [Space]

        [BHeader("Health", order = 1)]

        [SerializeField, Range(0f, 1000f)]
        private float m_MaxHealth = 100f;

        [SerializeField, Range(0f, 1000f)]
        private float m_InitialHealth = 100f;

        [BHeader("Gathering")]

        [SerializeField]
        private Vector3 m_GatherOffset;

        [SerializeField, Range(0.1f, 1f)]
        private float m_GatherRadius = 0.35f;

        private GatherableBehaviour[] m_Behaviours;

        private float m_Health;
        private bool m_BehavioursInitialized;

        private Collider m_Collider;
        private AudioSource m_AudioSource;

        private static readonly Dictionary<GatherableDefinition, List<Gatherable>> m_AllGatherables = new Dictionary<GatherableDefinition, List<Gatherable>>();


        #region Static Methods

        public static bool TryGetAllGatherablesWithDefinition(GatherableDefinition def, out List<Gatherable> gatherables)
        {
            return m_AllGatherables.TryGetValue(def, out gatherables);
        }

        private static void RegisterGatherable(Gatherable gatherable)
        {
            if (m_AllGatherables.TryGetValue(gatherable.Definition, out List<Gatherable> gatherables))
            {
                gatherables.Add(gatherable);
            }
            else if (gatherable.Definition != null)
            {
                List<Gatherable> gatherableList = new List<Gatherable>() { gatherable };
                m_AllGatherables.Add(gatherable.Definition, gatherableList);
            }
        }

        private static void UnregisterGatherable(Gatherable gatherable)
        {
            if (m_AllGatherables.TryGetValue(gatherable.Definition, out List<Gatherable> gatherableList))
                gatherableList.Remove(gatherable);
        }

        #endregion

        public DamageResult Damage(DamageInfo dmgInfo)
        {
            if (m_Health < 0.001f || dmgInfo.Damage < 0.01f)
                return DamageResult.Ignored;

            if (!m_BehavioursInitialized)
                SpawnBehaviours();

            DamageGatherable(dmgInfo);

            if (m_Health < 1f)
            {
                DestroyGatherable(dmgInfo);
                return DamageResult.Critical;
            }

            return DamageResult.Default;
        }

        public void ResetHealth()
        {
            if (m_Health - m_InitialHealth < 0.1f)
                return;

            if (m_BehavioursInitialized)
            {
                for (int i = 0; i < m_Behaviours.Length; i++)
                {
                    if (m_Behaviours[i] != null)
                        Destroy(m_Behaviours[i].gameObject);
                }

                m_Behaviours = null;
                m_BehavioursInitialized = false;
            }

            m_BaseVisuals.SetActive(true);
        }

        private void DamageGatherable(DamageInfo dmgInfo)
        {
            m_Health = Mathf.Clamp(m_Health - dmgInfo.Damage, 0f, 100f);

            for (int i = 0; i < m_Behaviours.Length; i++)
                m_Behaviours[i].DoHitEffects(dmgInfo);
        }

        private void DestroyGatherable(DamageInfo dmgInfo) 
        {
            Debug.Log("bu neyi destroy ediyo");
            m_Collider.enabled = false;

            for (int i = 0; i < m_Behaviours.Length; i++)
                m_Behaviours[i].DoDestroyEffects(dmgInfo);
        }

        private void SpawnBehaviours()
        {
            Debug.Log("tahta parçalarý burdamý üretiliyor acep");
            Debug.Log("HAYIR BURASI AÐACA HER VURDUÐUNDA ÇALIÞAN YER");
            var behaviourObject = Instantiate(m_RiggedPrefab, transform);
            m_Behaviours = behaviourObject.GetComponents<GatherableBehaviour>();

            for (int i = 0; i < m_Behaviours.Length; i++)
                m_Behaviours[i].InitializeBehaviour(this, m_AudioSource);

            m_BehavioursInitialized = true;
            m_BaseVisuals.SetActive(false);
        }

        private void Awake()
        {
            m_Health = m_InitialHealth;

            m_Collider = GetComponent<Collider>();
            m_AudioSource = GetComponent<AudioSource>();

            m_Collider.isTrigger = false;

            RegisterGatherable(this);
        }

        private void OnDestroy()
        {
            UnregisterGatherable(this);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (m_AudioSource != null)
                m_AudioSource.spatialBlend = 1f;
        }

        private void OnDrawGizmosSelected()
        {
            if (Event.current.type == EventType.Repaint)
            {
                Vector3 gatherPoint = transform.position + transform.TransformVector(m_GatherOffset);

                UnityEditor.Handles.CircleHandleCap(0, gatherPoint, Quaternion.LookRotation(Vector3.up), m_GatherRadius, EventType.Repaint);

                UnityEditor.Handles.color = new Color(1f, 0f, 0f, 0.5f);
                UnityEditor.Handles.SphereHandleCap(0, gatherPoint, Quaternion.identity, 0.1f, EventType.Repaint);
                UnityEditor.Handles.color = Color.white;

                UnityEditor.Handles.Label(gatherPoint, "Gather Position");
            }
        }
#endif
    }
}