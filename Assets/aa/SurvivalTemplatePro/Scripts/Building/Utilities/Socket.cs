using System;
using System.Collections.Generic;
using UnityEngine;

namespace SurvivalTemplatePro.BuildingSystem
{
    public class Socket : MonoBehaviour
    {
        #region Internal
        [Serializable]
        public class PieceOffset
        {
            public Buildable Buildable { get => m_Buildable; set => m_Buildable = value; }

            public Vector3 PositionOffset { get => m_PositionOffset; set => m_PositionOffset = value; }
            public Quaternion RotationOffset => Quaternion.Euler(m_RotationOffset);
            public Vector3 RotationOffsetEuler { get => m_RotationOffset; set => m_RotationOffset = value; }

            [SerializeField]
            private Buildable m_Buildable;

            [SerializeField]
            private Vector3 m_PositionOffset = Vector3.one;

            [SerializeField]
            private Vector3 m_RotationOffset;


            public PieceOffset GetMemberwiseClone() => (PieceOffset)MemberwiseClone();
        }

        public class SpaceOccupier
        {
            public SocketType OccupiedSpace { get; private set; }
            public Buildable Occupier { get; private set; }


            public SpaceOccupier(SocketType occupiedSpace, Buildable occupier)
            {
                OccupiedSpace = occupiedSpace;
                Occupier = occupier;
            }
        }
        #endregion

        public SocketType OccupiedSpaces { get => m_OccupiedSpaces; set => m_OccupiedSpaces = value; }
		public Buildable Buildable => m_ParentBuildable;
		public List<PieceOffset> PieceOffsets { get => m_PieceOffsets; set => m_PieceOffsets = value; }
		public float Radius { get => m_Radius; set => m_Radius = value; }

        [SerializeField]
		private List<PieceOffset> m_PieceOffsets;
	
		[SerializeField]
		private float m_Radius = 1f;

        #if UNITY_EDITOR
        [SerializeField]
        [HideInInspector]
        private int m_SelectedBuildableOffset;
        #endif

        private Buildable m_ParentBuildable;
        private SocketType m_OccupiedSpaces;
		private List<SpaceOccupier> m_Occupiers = new List<SpaceOccupier>();


        private void Awake()
        {
            var sphere = gameObject.AddComponent<SphereCollider>();
            sphere.isTrigger = true;
            sphere.radius = Radius;

            m_ParentBuildable = GetComponentInParent<Buildable>();
        }

        public PieceOffset GetBuildableOffset(string name)
        {
            PieceOffset offset = null;

            for(int i = 0;i < m_PieceOffsets.Count;i++)
            {
                if(m_PieceOffsets[i].Buildable != null && m_PieceOffsets[i].Buildable.PlaceableName == name)
                {
                    offset = m_PieceOffsets[i];
                    break;
                }
            }

            return offset;
        }
			
		public bool HasSpaceForBuildable(LayerMask mask, Buildable buildable)
		{
			// Get the objects that overlap this socket.
			var overlappingStuff = Physics.OverlapSphere(transform.position, Radius, mask, QueryTriggerInteraction.Ignore);

            for (int i = 0; i < overlappingStuff.Length; i++)
            {
                if (!m_ParentBuildable.HasCollider(overlappingStuff[i]))
                    return false;

                if (m_ParentBuildable != buildable)
                {
                    if (!m_ParentBuildable.ParentStructure.HasCollider(overlappingStuff[i]) && overlappingStuff[i] as TerrainCollider == null)
                        return false;
                }
                else
                {
                    if (!m_ParentBuildable.HasCollider(overlappingStuff[i]) && overlappingStuff[i] as TerrainCollider == null)
                        return false;
                }
            }

			return true;
		}

		public void OccupySpaces(Buildable buildable)
		{
            if (!m_OccupiedSpaces.Has(buildable.SpacesToOccupy))
                m_OccupiedSpaces |= buildable.SpacesToOccupy;
		}

        public bool SupportsBuildable(Buildable buildable)
		{
			for (int i = 0; i < m_PieceOffsets.Count; i++)
			{
				if (m_PieceOffsets[i] != null && m_PieceOffsets[i].Buildable != null && m_PieceOffsets[i].Buildable.PlaceableName == buildable.PlaceableName && !m_OccupiedSpaces.Has(buildable.NeededSpace))
					return true;
			}

			return false;
		}

		private void OnDrawGizmos()
		{
			var oldMatrix = Gizmos.matrix;

			Gizmos.color = new Color(0f, 1f, 0f, 0.8f);
			Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one * 0.25f);

			Gizmos.DrawCube(Vector3.zero, Vector3.one);

			Gizmos.matrix = oldMatrix;
		}

		private void OnDrawGizmosSelected()
		{
			Gizmos.color = new Color(0.5f, 0.5f, 0.5f, 0.35f);
			Gizmos.DrawSphere(transform.position, m_Radius);
		}
    }
}