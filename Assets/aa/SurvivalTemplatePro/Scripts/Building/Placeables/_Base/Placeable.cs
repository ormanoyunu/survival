using System;
using System.Collections.Generic;
using UnityEngine;

namespace SurvivalTemplatePro.BuildingSystem
{
    /// <summary>
    /// TODO: Rework
    /// </summary>
    public class Placeable : MonoBehaviour
    {
        #region Internal
        [Serializable]
        public class StabilitySettings
        {
            public bool CheckStability = true;

            [EnableIf("CheckStability", true)]
            public LayerMask Mask;

            [EnableIf("CheckStability", true)]
            public bool ShowBoxes;

            [EnableIf("CheckStability", true)]
            public Bounds[] Boxes;
        }

        [Serializable]
        public class TerrainCollisionSettings
        {
            public bool CheckTerrainCollision;

            [EnableIf("CheckTerrainCollision", true)]
            public bool ShowBounds;

            [EnableIf("CheckTerrainCollision", true)]
            public Bounds Bounds;
        }
        #endregion

        public int PlaceableID => m_PlaceableId;
        public string PlaceableName => m_PlaceableName;
        public string PlaceableDescription => m_PlaceableDescription;
        public Sprite PlaceableIcon => m_PlaceableIcon;

        public bool IsPlaced { get; private set; }
        public Bounds Bounds => GetWorldBounds();
        public Bounds LocalBounds => m_LocalBounds; 

        public Renderer[] Renderers { get { if(m_Renderers == null) GetRenderers(); return m_Renderers; } }
        public Collider[] Colliders { get { if(m_Colliders == null) GetColliders(); return m_Colliders; } }

        public float VerticalOffset => m_VerticalOffset;
        public bool PlaceOnBuildables => m_PlaceOnBuildables;
        public bool CheckTerrainCollision => m_TerrainCollision.CheckTerrainCollision;

        [SerializeField, HideInInspector]
        private int m_PlaceableId;

        [BHeader("General (Placeable)")]
         
        [SerializeField]
        private string m_PlaceableName = "Unnamed Placeable";

        [SerializeField, Multiline]
        private string m_PlaceableDescription;

        [SerializeField, PreviewSprite]
        private Sprite m_PlaceableIcon;

        [SerializeField, Range(0f, 10f)]
        private float m_VerticalOffset;

        [SerializeField]
        private bool m_PlaceOnBuildables = true;

        [BHeader("Bounds (Placeable))")]

        [SerializeField, Range(0f, 1f)]
        private float m_BoundsGroundOffset = 0f;

        [SerializeField, Range(0.1f, 2f)]
        private float m_BoundsScale = 1f;

        [SerializeField, HideInInspector]
        private Bounds m_RawLocalBounds;

        [SerializeField, HideInInspector]
        private Bounds m_LocalBounds;

        [BHeader("Effects (Placeable)")]

        [SerializeField]
        private SoundPlayer m_PlacementAudio;

        [SerializeField]
        private GameObject m_PlacementFX;

        [BHeader("Advanced (Placeable)")]

        [SerializeField]
        private StabilitySettings m_Stability;

        [SerializeField]
        private TerrainCollisionSettings m_TerrainCollision;

        [SerializeField]
        protected List<Collider> m_IgnoredColliders;

        [SerializeField]
        protected List<MeshRenderer> m_IgnoredRenderers;

        protected MeshRenderer[] m_Renderers;
        protected Collider[] m_Colliders;


        public void CalculateLocalBounds()
        {
            // Calculate the bounds without modifications
            m_RawLocalBounds = new Bounds(transform.position, Vector3.zero);
            var renderers = GetComponentsInChildren<MeshRenderer>();

            Quaternion initialRotation = transform.rotation;
            transform.rotation = Quaternion.identity;

            for(int i = 0;i < renderers.Length;i++)
                m_RawLocalBounds.Encapsulate(renderers[i].bounds);

            m_RawLocalBounds = new Bounds(m_RawLocalBounds.center - transform.position, m_RawLocalBounds.size);

            transform.rotation = initialRotation;

            // Calculate the bounds with modifications
            Vector3 center = m_RawLocalBounds.center;
            Vector3 extents = m_RawLocalBounds.extents;

            float upExtent = extents.y;

            Vector3 offset = Vector3.up * upExtent * m_BoundsGroundOffset;

            center += offset;
            extents -= offset;
            extents = Vector3.Scale(extents, new Vector3(m_BoundsScale, 1f, m_BoundsScale));

            m_LocalBounds = new Bounds(center, extents * 2);
        }

        public virtual bool CanPlace() => true;

        // TODO: Make this private and use it in CanPlace() ???
        public bool IsBlockedByTerrain()
        {
            if(!m_TerrainCollision.CheckTerrainCollision)
                return false;

            Collider[] overlappingStuff = Physics.OverlapBox(transform.position + transform.TransformVector(m_TerrainCollision.Bounds.center), m_TerrainCollision.Bounds.extents, transform.rotation, Physics.AllLayers, QueryTriggerInteraction.Ignore);

            for (int i = 0;i < overlappingStuff.Length;i++)
            {
                if (overlappingStuff[i] as TerrainCollider != null)
                    return true;
            }

            return false;
        }

        // TODO: Make this private and use it in CanPlace() ???
        public bool HasSupport(out Collider[] colliders)
        {
            bool allOk = true;
            List<Collider> colliderS = new List<Collider>();

            for(int o = 0;o < m_Stability.Boxes.Length;o++)
            {
                Bounds stabilityBox = m_Stability.Boxes[o];

                var colsInRadius = Physics.OverlapBox(transform.position + transform.TransformVector(stabilityBox.center), stabilityBox.extents, transform.rotation, m_Stability.Mask);

                for(int i = 0;i < colsInRadius.Length;i++)
                {
                    if(!colsInRadius[i].isTrigger && !HasCollider(colsInRadius[i]))
                    {
                        colliderS.Add(colsInRadius[i]);
                        continue;
                    }

                    allOk = false;
                }
            }

            colliders = colliderS.ToArray();
            return allOk;
        }

        public bool HasCollider(Collider col)
        {
            for (int i = 0;i < m_Colliders.Length;i++)
            {
                if (m_Colliders[i] == col)
                    return true;
            }

            return false;
        }

        protected virtual void Awake()
        {
            GetRenderers();
            GetColliders();
        }

        protected virtual void OnDrawGizmosSelected()
        {
            var oldMatrix = Gizmos.matrix;
            Gizmos.color = Color.blue;

            // Draw the terrain protection box
            Gizmos.color = Color.yellow;

            if(m_TerrainCollision.ShowBounds)
            {
                Gizmos.matrix = Matrix4x4.TRS(transform.position + transform.TransformVector(m_TerrainCollision.Bounds.center), transform.rotation, m_TerrainCollision.Bounds.size);
                Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
            }

            // Draw the stability box
            Gizmos.color = Color.red;

            if(m_Stability.ShowBoxes)
            {
                for(int i = 0;i < m_Stability.Boxes.Length;i++)
                {
                    Bounds stabilityBox = m_Stability.Boxes[i];

                    Gizmos.matrix = Matrix4x4.TRS(transform.position + transform.TransformVector(stabilityBox.center), transform.rotation, stabilityBox.size);
                    Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
                }
            }

            Gizmos.matrix = oldMatrix;
        }

        public virtual void Place()
        {
            for (int i = 0;i < m_Colliders.Length;i++)
                m_Colliders[i].enabled = true;

            m_PlacementAudio.Play2D();

            if (m_PlacementFX != null)
                Instantiate(m_PlacementFX, transform.position, Quaternion.identity, null);

            IsPlaced = true;
        }

#if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            if (m_RawLocalBounds.size == Vector3.zero)
                CalculateLocalBounds();
        }
#endif

        private void GetRenderers()
        {
            List<MeshRenderer> rendererList = new List<MeshRenderer>();
            GetComponentsInChildren<MeshRenderer>(true, rendererList);

            // Remove the renderers we want to ignore
            rendererList.RemoveAll((MeshRenderer r) => { return m_IgnoredRenderers.Contains(r); });

            m_Renderers = rendererList.ToArray();
        }

        private void GetColliders()
        {
            List<Collider> colliderList = new List<Collider>();
            GetComponentsInChildren<Collider>(colliderList);

            // Remove the colliders we want to ignore
            colliderList.RemoveAll((Collider col) => { return m_IgnoredColliders.Contains(col); });

            // Disable collisions between colliders attached on this object
            foreach (var col1 in colliderList)
            {
                foreach (var col2 in colliderList)
                {
                    if (col1 != col2)
                        Physics.IgnoreCollision(col1, col2);
                }
            }

            m_Colliders = colliderList.ToArray();
        }

        private Bounds GetWorldBounds()
        {
            Bounds worldBounds = m_LocalBounds;
            worldBounds.center = transform.position + transform.TransformVector(worldBounds.center);

            return worldBounds;
        }
    }
}