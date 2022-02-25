using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SurvivalTemplatePro.BuildingSystem
{
    public enum BuildablePreviewType { Simple, Structure }

    public class BuildablePreview : MonoBehaviour
    {
        public static List<BuildablePreview> AllPreviewsInScene = new List<BuildablePreview>();

        public bool PreviewEnabled { get; private set; }
        public Vector3 Center { get; private set; }

        public List<BuildRequirement> BuildRequirements { get; private set; } = new List<BuildRequirement>();
        public List<Buildable> AllBuildables { get; private set; } = new List<Buildable>();

        public event UnityAction onBuildComplete;
        public event UnityAction<int, int> onMaterialAdded;

        private BuildablePreviewType m_PreviewType;

        private StructureManager m_Structure;
        private Buildable m_Buildable;

        private Collider[] m_CollisionResults = new Collider[10];


        public void UpdatePreview()
        {
            CalculateRequiredItems();
            CalculateCenter();

            // Keep a list of all buildables
            AllBuildables.Clear();

            if (m_PreviewType == BuildablePreviewType.Simple)
                AllBuildables.Add(m_Buildable);
            else
                AllBuildables.AddRange(m_Structure.Buildables);
        }

        public void EnablePreview()
        {
            m_Structure = GetComponent<StructureManager>();
            m_Buildable = GetComponent<Buildable>();

            m_PreviewType = m_Structure != null ? BuildablePreviewType.Structure : BuildablePreviewType.Simple;
            PreviewEnabled = true;

            if (m_PreviewType == BuildablePreviewType.Structure)
            {
                m_Structure.OnPartAdded += PartAdded;
                m_Structure.SetActivationState(BuildableActivationState.Preview);
            }
            else if (m_PreviewType == BuildablePreviewType.Simple)
                m_Buildable.SetActivationState(BuildableActivationState.Preview);

            PartAdded(null);
            RegisterPreview(this);
        }

        public void DisablePreview()
        {
            if(m_PreviewType == BuildablePreviewType.Structure)
            {
                m_Structure.SetActivationState(BuildableActivationState.Placed);
                m_Structure.OnPartAdded -= PartAdded;
                m_Structure = null;
            }
            else if(m_PreviewType == BuildablePreviewType.Simple)
            {
                m_Buildable.SetActivationState(BuildableActivationState.Placed);
                m_Buildable = null;
            }

            PreviewEnabled = false;
            UnRegisterPreview(this);
        }

        public bool TryAddBuildMaterial(BuildingMaterialInfo matInfo)
        {
            if (!CanAddBuildMaterials())
                return false;

            for (int i = 0;i < BuildRequirements.Count;i++)
            {
                BuildRequirement buildReq = BuildRequirements[i];

                if (buildReq.BuildingMaterialId == matInfo.Id)
                {
                    if (buildReq.CurrentAmount < buildReq.RequiredAmount)
                    {
                        buildReq.CurrentAmount++;
                        MaterialAdded(i, 1);

                        matInfo.UseSound.PlayAtPosition(transform.position);

                        return true;
                    }
                    else
                        return false;
                }
            }

            return false;
        }

        public bool CanAddBuildMaterials()
        {
            bool entityIsInsidePreview = false;

            foreach (Buildable buildable in AllBuildables)
            {
                int colliderCount = Physics.OverlapBoxNonAlloc(
                    buildable.Bounds.center,
                    buildable.Bounds.extents,
                    m_CollisionResults,
                    buildable.transform.rotation,
                    PlaceableDatabase.GetEntityMask(),
                    QueryTriggerInteraction.Ignore);

                if (colliderCount > 0)
                {
                    entityIsInsidePreview = true;
                    break;
                }
            }

            return !entityIsInsidePreview;
        }

        public void UpdateColorBasedOnCollisions()
        {
            bool canAddMaterials = CanAddBuildMaterials();

            foreach(Buildable buildable in AllBuildables)
                buildable.MaterialChanger.SetOverrideMaterial(canAddMaterials ? PlaceableDatabase.GetPlaceAllowedMaterial() : PlaceableDatabase.GetPlaceDeniedMaterial());
        }

        public void SetAllowedColor()
        {
            foreach(Buildable buildable in AllBuildables)
                buildable.MaterialChanger.SetOverrideMaterial(PlaceableDatabase.GetPlaceAllowedMaterial());
        }

        public void SetDeniedColor()
        {
            foreach(Buildable buildable in AllBuildables)
                buildable.MaterialChanger.SetOverrideMaterial(PlaceableDatabase.GetPlaceDeniedMaterial());
        }

        public void Cancel()
        {
            UnRegisterPreview(this);
            Destroy(gameObject);
        }

        private void CalculateRequiredItems()
        {
            BuildRequirements.Clear();

            if (m_PreviewType == BuildablePreviewType.Structure)
            {
                for (int i = 0; i < m_Structure.Buildables.Count; i++)
                    CalculateRequiredItemsForBuildable(m_Structure.Buildables[i]);
            }
            else
                CalculateRequiredItemsForBuildable(m_Buildable);
        }

        private void CalculateRequiredItemsForBuildable(Buildable buildable)
        {
            for (int i = 0; i < buildable.BuildRequirements.Length; i++)
            {
                BuildRequirement buildMaterial = FindBuildMaterial(buildable.BuildRequirements[i].BuildingMaterialId);

                if (buildMaterial != null)
                    buildMaterial.RequiredAmount += buildable.BuildRequirements[i].RequiredAmount;
                else
                    BuildRequirements.Add(new BuildRequirement(buildable.BuildRequirements[i].BuildingMaterialId, buildable.BuildRequirements[i].RequiredAmount, 0));
            }
        }

        private void MaterialAdded(int materialIdx, int count)
        {
            onMaterialAdded?.Invoke(materialIdx, count);

            bool buildComplete = true;

            for (int i = 0;i < BuildRequirements.Count;i++)
            {
                BuildRequirement buildReq = BuildRequirements[i];

                if (buildReq.RequiredAmount > buildReq.CurrentAmount)
                    buildComplete = false;
            }

            if (buildComplete)
            {
                if (m_PreviewType == BuildablePreviewType.Structure)
                    m_Structure.SetActivationState(BuildableActivationState.Placed);
                else
                    m_Buildable.SetActivationState(BuildableActivationState.Placed);

                onBuildComplete?.Invoke();

                Destroy(this);
            }
        }

        private BuildRequirement FindBuildMaterial(int matId)
        {
            for (int i = 0;i < BuildRequirements.Count;i++)
            {
                if (BuildRequirements[i].BuildingMaterialId == matId)
                    return BuildRequirements[i];
            }

            return null;
        }

        private void CalculateCenter()
        {
            if(m_PreviewType == BuildablePreviewType.Structure)
            {
                Center = Vector3.zero;

                for(int i = 0;i < m_Structure.Buildables.Count;i++)
                    Center += m_Structure.Buildables[i].transform.position;

                Center /= m_Structure.Buildables.Count;
            }
            else
                Center = m_Buildable.transform.position;
        }

        private void PartAdded(Buildable buildable)
        {
            CalculateRequiredItems();
            CalculateCenter();
        }

        private void OnDestroy() => UnRegisterPreview(this);

        private static void RegisterPreview(BuildablePreview preview)
        {
            if (!AllPreviewsInScene.Contains(preview))
                AllPreviewsInScene.Add(preview);
        }

        private static void UnRegisterPreview(BuildablePreview preview)
        {
            if (AllPreviewsInScene.Contains(preview))
                AllPreviewsInScene.Remove(preview);
        }
    }
}