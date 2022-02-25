using System.Collections;
using UnityEngine;

namespace SurvivalTemplatePro.ResourceGathering
{
    public class TreeFallBehaviour : GatherableBehaviour
    {
        #region Internal
        protected enum DestroyType
        {
            ThisObject,
            ParentObject
        }
        #endregion

        [BHeader("Tree Falling")]

        [SerializeField, Range(1f, 100f)]
        private float m_MaxTimeToFall = 10f;

        [SerializeField, Range(1f, 1000f)]
        private float m_TreeFallForce = 10f;

        [Space]

        [SerializeField]
        private Rigidbody m_FallingTree;

        [SerializeField]
        private Collider m_TreeStump;

        [Space]

        [SerializeField]
        private SoundPlayer m_TreeFallAudio;

        [BHeader("Tree Impact")]

        [SerializeField, Range(0f, 100f)]
        private float m_TreeImpactLogsForce = 10f;

        [SerializeField, Range(0f, 100f)]
        private float m_LogsGroundOffset = 0.5f;

        [Space]

        [SerializeField]
        private DestroyType m_DestroyType = DestroyType.ThisObject;

        [SerializeField]
        private TriggerEventHandler m_ImpactTrigger;

        [SerializeField]
        private GameObject m_LogsRoot;

        [Space]

        [SerializeField]
        private GameObject m_TreeImpactFX;

        [SerializeField]
        private SoundPlayer m_TreeImpactAudio;

        [Space]

        [SerializeField]
        private float m_ImpactShakeRadius = 15f;

        [SerializeField]
        private float m_ImpactShakeScale = 1f;

        private float m_TimeSinceFallStart = 0f;
        private bool m_IsFalling;
        private bool m_HadImpact;


        public override void DoHitEffects(DamageInfo damageInfo) { }

        /// <summary>
        /// Start tree fall
        /// </summary>
        /// <param name="dmgInfo"></param>
        public override void DoDestroyEffects(DamageInfo dmgInfo)
        {
            Vector3 direction = new Vector3(dmgInfo.HitDirection.x, 0, dmgInfo.HitDirection.z).normalized;

            m_FallingTree.GetComponent<Collider>().enabled = true;
            m_FallingTree.isKinematic = false;
            m_FallingTree.AddForce(direction * m_TreeFallForce, ForceMode.Impulse);

            m_TreeFallAudio.Play(AudioSource, 1f, SelectionType.Random);

            if (m_ImpactTrigger != null)
                m_ImpactTrigger.TriggerEnter += OnTreeImpact;

            m_TreeStump.enabled = true;
            m_TreeStump.transform.SetParent(transform.parent, true);

            m_IsFalling = true;
            m_HadImpact = false;
        }

        private void Awake()
        {
            m_FallingTree.isKinematic = true;
            m_FallingTree.GetComponent<Collider>().enabled = false;
            m_TreeStump.enabled = false;
        }

        private void Update()
        {
            if (!m_IsFalling || m_HadImpact)
                return;

            // Force start the tree impact behaviour when the time limit is up
            if (m_TimeSinceFallStart > m_MaxTimeToFall)
                OnTreeImpact(null);
            // Force start the tree impact behaviour if the velocity of the tree fall is ~0f
            else if (m_TimeSinceFallStart > m_MaxTimeToFall / 3f && m_FallingTree.velocity.sqrMagnitude == 0f)
                OnTreeImpact(null);

            m_TimeSinceFallStart += Time.deltaTime;
        }

        private void OnTreeImpact(Collider other)
        {
            if (!m_HadImpact && m_IsFalling)
                StartCoroutine(C_DelayedImpact());

            if (m_ImpactTrigger != null)
                m_ImpactTrigger.TriggerEnter -= OnTreeImpact;
        }

        private IEnumerator C_DelayedImpact() 
        {
            m_TreeImpactAudio.Play(AudioSource, 1f, SelectionType.RandomExcludeLast);

            m_LogsRoot.SetActive(true);
            m_LogsRoot.transform.SetParent(null);

            ShakeImpactEvent.RaiseEvent(new ShakeImpactEvent(transform.position, m_ImpactShakeRadius, m_ImpactShakeScale * (Mathf.Clamp(m_FallingTree.velocity.sqrMagnitude, 0f, 10f) / 10f)));

            yield return new WaitForSeconds(0.1f);

            foreach (var logRigidbody in m_LogsRoot.transform.GetComponentsInChildren<Rigidbody>())
            {
                Transform logTransform = logRigidbody.transform;

                logRigidbody.transform.position = new Vector3(logTransform.position.x, logTransform.position.y + m_LogsGroundOffset, logTransform.position.z);
                logRigidbody.AddForce(logTransform.right * Random.Range(-1f, 1f) * m_TreeImpactLogsForce, ForceMode.Impulse);

                if (m_TreeImpactFX != null)
                    Instantiate(m_TreeImpactFX, logTransform.position, logTransform.rotation);
            }

            m_HadImpact = true;
            m_IsFalling = false;

            Destroy(m_DestroyType == DestroyType.ThisObject ? gameObject : transform.parent.gameObject);
        }
    }
}
