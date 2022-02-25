using SurvivalTemplatePro.BuildingSystem;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace SurvivalTemplatePro
{
    /// <summary>
    /// Represents an asset that stores all the Building Materials.
    /// </summary>
    [CreateAssetMenu(menuName = "Survival Template Pro/Databases/Building Materials Database")]
    public class BuildMaterialsDatabase : AssetDatabase<BuildMaterialsDatabase>
    {
        [SerializeField]
        private BuildingMaterialInfo[] m_BuildingMaterials;


        public static BuildingMaterialInfo GetBuildingMaterialAtIndex(int index)
        {
            var buildingMaterials = Instance.m_BuildingMaterials;

            if (buildingMaterials != null && buildingMaterials.Length > 0)
                return buildingMaterials[Mathf.Clamp(index, 0, buildingMaterials.Length - 1)];
            else
                return null;
        }

        public static int GetIndexOfBuildingMaterial(int id)
        {
            var buildingMaterials = Instance.m_BuildingMaterials;

            for (int i = 0; i < buildingMaterials.Length; i++)
            {
                if (buildingMaterials[i].Id == id)
                    return i;
            }

            return -1;
        }

        public static BuildingMaterialInfo GetBuildingMaterialByName(string name)
        {
            var buildingMaterials = Instance.m_BuildingMaterials;

            for (int i = 0; i < buildingMaterials.Length; i++)
            {
                if (buildingMaterials[i].Name == name)
                    return buildingMaterials[i];
            }

            return null;
        }

        public static BuildingMaterialInfo GetBuildingMaterialById(int id)
        {
            var buildingMaterials = Instance.m_BuildingMaterials;

            for (int i = 0; i < buildingMaterials.Length; i++)
            {
                if (buildingMaterials[i].Id == id)
                    return buildingMaterials[i];
            }

            return null;
        }

        public static string[] GetBuildingMaterialNames()
        {
            var buildingMaterials = Instance.m_BuildingMaterials;

            string[] names = new string[buildingMaterials.Length];

            for (int i = 0; i < names.Length; i++)
                names[i] = buildingMaterials[i].Name;

            return names;
        }

#if UNITY_EDITOR

        protected override void RefreshIDs()
        {
            if (m_BuildingMaterials == null || m_BuildingMaterials.Length == 0)
                return;

            List<int> idList = new List<int>();

            foreach (var buildingMaterial in m_BuildingMaterials)
            {
                if (buildingMaterial == null)
                    return;

                idList.Add(buildingMaterial.Id);
            }

            int maxAssignmentTries = 50;
            int i = 0;

            foreach (var buildingMaterial in m_BuildingMaterials)
            {
                int assignmentTries = 0;
                int assignedId = idList[i];

                while ((assignedId == 0 || idList.Contains(assignedId) && (idList.IndexOf(assignedId) != i)) && assignmentTries < maxAssignmentTries)
                {
                    assignedId = IdGenerator.GenerateIntegerId();
                    assignmentTries++;
                }

                if (assignmentTries == maxAssignmentTries)
                {
                    Debug.LogError("Couldn't generate an unique id for carriable: " + buildingMaterial.Name);
                    return;
                }
                else
                {
                    idList[i] = assignedId;
                    AssignIdToCarriable(buildingMaterial, assignedId);
                }

                i++;
            }
        }

        private void AssignIdToCarriable(BuildingMaterialInfo buildingMaterial, int id)
        {
            if (buildingMaterial == null)
                return;

            Type placeableType = typeof(BuildingMaterialInfo);
            FieldInfo idField = placeableType.GetField("m_Id", BindingFlags.NonPublic | BindingFlags.Instance);

            idField.SetValue(buildingMaterial, id);
        }
#endif
    }
}