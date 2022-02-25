using UnityEngine;

namespace SurvivalTemplatePro.ResourceGathering
{
    public class TreeChopBehaviour : GatherableBehaviour
    {
        #region Internal
        [System.Serializable]
        private class ChoppingSegment
        {
            public bool Enabled { get; private set; } = true;
            public Vector3 ObjectPosition => m_Object.transform.position;

            [HideInInspector]
            public Vector3 Normal;

            [SerializeField]
            private GameObject m_Object;


            public void Enable()
            {
                if (m_Object != null)
                    m_Object.SetActive(true);

                Enabled = true;
            }

            public void Disable()
            {
                if (m_Object != null)
                    m_Object.SetActive(false);

                Enabled = false;
            }
        }
        #endregion

        [SerializeField]
        [Tooltip("Center")]
        private Transform m_ChoppingPivot;

        [SerializeField]
        private ChoppingSegment[] m_ChoppingSegments;

        private Vector3 m_SegmentsCenter;
        private int m_ChoppedSegmentCount;


        private void Start() => PrepareChoppingSegments();

        public override void DoHitEffects(DamageInfo dmgInfo)
        {
            if (m_ChoppingSegments.Length == 0)
                return;

            int disabledSegments = m_ChoppingSegments.Length - (int)((Gatherable.Health / 100f) * m_ChoppingSegments.Length);
            int amountToChop = disabledSegments - m_ChoppedSegmentCount;

            if (amountToChop > 0)
            {
                for (int i = 0; i < amountToChop; i++)
                {
                    int indexToSelect = 0;

                    float largestAngle = 0f;
                    Vector3 chopPointNormal = (dmgInfo.HitPoint - m_SegmentsCenter).normalized;

                    for (int j = 0; j < m_ChoppingSegments.Length; j++)
                    {
                        if (!m_ChoppingSegments[j].Enabled)
                            continue;

                        float angle = Vector3.Angle(chopPointNormal, m_ChoppingSegments[j].Normal);

                        if (angle > largestAngle)
                        {
                            largestAngle = angle;
                            indexToSelect = j;
                        }
                    }

                    m_ChoppingSegments[indexToSelect].Disable();
                }

                m_ChoppedSegmentCount += amountToChop;
            }
        }

        public override void DoDestroyEffects(DamageInfo dmgInfo)
        {
            foreach (var segment in m_ChoppingSegments)
                segment.Disable();
        }

        private void PrepareChoppingSegments()
        {
            if (m_ChoppingSegments.Length != 0)
            {
                m_SegmentsCenter = m_ChoppingPivot.position;

                // Activate the first version of the segment (the unchopped version)
                foreach (var segment in m_ChoppingSegments)
                {
                    segment.Enable();

                    if (m_ChoppingPivot != null)
                        segment.Normal = (m_ChoppingPivot.position - segment.ObjectPosition).normalized;
                }
            }
        }
    }
}