using SurvivalTemplatePro.BuildingSystem;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace SurvivalTemplatePro
{
    /// <summary>
    /// 
    /// </summary>
    [CreateAssetMenu(menuName = "Survival Template Pro/Building/Placeable Database")]
    public class PlaceableDatabase : AssetDatabase<PlaceableDatabase>
	{
        #region Internal
        [Serializable]
        public class Category
        {
            public string Name;
          
            [Space]
            public Placeable[] Placeables;
        }
        #endregion

        public const string k_CustomBuildingCategoryName = "Custom Building";

        [SerializeField, Group]
        private Category[] m_PlaceableCategories; 

        [SerializeField]
        [Space, InfoBox("Structure manager prefab, handles socket based building.")]
        private StructureManager m_Structure;

        [SerializeField]
        [Space, InfoBox("Universal 'allowed placing' material for every placeable.")]
        private Material m_PlacementAllowedMaterial;

        [SerializeField]
        [Space, InfoBox("Universal 'denied placing' material for every placeable.")]
        private Material m_PlacementDeniedMaterial;

        [Space]

        [SerializeField]
        private LayerMask m_FreePlacementMask;

        [SerializeField, Tooltip("Layer mask to detect entities (AI or players).")]
        private LayerMask m_EntityMask;

        private Dictionary<string, Category> m_PlaceableCategoryDictionary;
        private Dictionary<string, Placeable> m_PlaceablesByName;
        private Dictionary<int, Placeable> m_PlaceablesById;


        #region Initialization
        protected override void GenerateDictionaries()
        {
            GenerateCategoryDictionary();
            GeneratePlaceableDictionary();
        }


        private void GenerateCategoryDictionary()
        {
            m_PlaceableCategoryDictionary = new Dictionary<string, Category>();

            if (Instance.m_PlaceableCategoryDictionary.Count == 0)
            {
                // Generate categories dictionary
                foreach (var placCategory in m_PlaceableCategories)
                {
                    if (!m_PlaceableCategoryDictionary.ContainsKey(placCategory.Name))
                        m_PlaceableCategoryDictionary.Add(placCategory.Name, placCategory);
                    else
                        return;
                }
            }
        }

        private void GeneratePlaceableDictionary()
        {
            m_PlaceablesByName = new Dictionary<string, Placeable>();
            m_PlaceablesById = new Dictionary<int, Placeable>();

            // Generate placeables dictionary
            foreach (var category in m_PlaceableCategories)
            {
                foreach (var placeable in category.Placeables)
                {
                    if (placeable != null)
                    {
                        if (placeable.PlaceableName != string.Empty)
                        {
                            if (!m_PlaceablesByName.ContainsKey(placeable.PlaceableName))
                                m_PlaceablesByName.Add(placeable.PlaceableName, placeable);
                            else
                                return;
                        }
                        else
                        {
                            if (!m_PlaceablesByName.ContainsKey(placeable.name))
                            {
                                m_PlaceablesByName.Add(placeable.name, placeable);
                                Debug.LogWarning("Buildable with prefab '" + placeable.name + "does not have a set name.");
                            }
                            else
                                return;
                        }

                        if (!m_PlaceablesById.ContainsKey(placeable.PlaceableID))
                            m_PlaceablesById.Add(placeable.PlaceableID, placeable);
                    }
                }
            }
        }

#if UNITY_EDITOR
        protected override void RefreshIDs()
        {
            List<int> idList = new List<int>();

            foreach (var category in m_PlaceableCategories)
            {
                foreach (var placeable in category.Placeables)
                {
                    if (placeable != null)
                        idList.Add(placeable.PlaceableID);
                    else
                        idList.Add(0);
                }
            }

            int maxAssignmentTries = 50;
            int i = 0;

            foreach (var category in m_PlaceableCategories)
            {
                foreach (var placeable in category.Placeables)
                {
                    if (placeable == null)
                        return;

                    int assignmentTries = 0;
                    int assignedId = idList[i];

                    while ((assignedId == 0 || idList.Contains(assignedId) && (idList.IndexOf(assignedId) != i)) && assignmentTries < maxAssignmentTries)
                    {
                        assignedId = IdGenerator.GenerateIntegerId();
                        assignmentTries++;
                    }

                    if (assignmentTries == maxAssignmentTries)
                    {
                        Debug.LogError("Couldn't generate an unique id for placeable: " + placeable.PlaceableName);
                        return;
                    }
                    else
                    {
                        idList[i] = assignedId;
                        AssignIdToPlaceable(placeable, assignedId);
                    }

                    i++;
                }
            }
        }


        private void AssignIdToPlaceable(Placeable placeable, int id)
        {
            Type placeableType = typeof(Placeable);
            FieldInfo idField = placeableType.GetField("m_PlaceableId", BindingFlags.NonPublic | BindingFlags.Instance);

            idField.SetValue(placeable, id);
            UnityEditor.EditorUtility.SetDirty(placeable);
        }
#endif
    #endregion

        #region Placeable Methods
        public static Placeable GetPlaceableAtIndex(int index)
        {
            var placeablesCategories = Instance.m_PlaceableCategories;

            if (placeablesCategories != null && placeablesCategories.Length > 0)
            {
                int allPlaceablesCount = 0;

                for (int i = 0; i < placeablesCategories.Length; i++)
                    allPlaceablesCount += placeablesCategories[i].Placeables.Length;

                index = Mathf.Clamp(index, 0, allPlaceablesCount - 1);

                foreach (var category in placeablesCategories)
                {
                    if (index > category.Placeables.Length - 1)
                        index -= category.Placeables.Length;
                    else
                        return category.Placeables[index];
                }
            }

            return null;
        }

        public static int GetIndexOfPlaceable(int placeableId)
        {
            int index = 0;

            for (int i = 0; i < Instance.m_PlaceableCategories.Length; i++)
            {
                foreach (var placeable in Instance.m_PlaceablesById.Values)
                {
                    if (placeable.PlaceableID == placeableId)
                        return index;

                    index++;
                }
            }

            return -1;
        }

        public static Placeable GetPlaceableByName(string placeableName)
        {
            if (Instance.m_PlaceablesByName.TryGetValue(placeableName, out Placeable placeable))
                return placeable;

            return null;
        }

        public static Placeable GetPlaceableById(int placeableId)
        {
            if (Instance.m_PlaceablesById.TryGetValue(placeableId, out Placeable placeable))
                return placeable;

            return null;
        }

        public static Category GetCategory(string categoryName) 
        {
            if (Instance.m_PlaceableCategoryDictionary.TryGetValue(categoryName, out Category category))
                return category;

            return null;
        }

        public static int GetIndexInCategory(int placeableId, string categoryName)
        {
            if (Instance.m_PlaceableCategoryDictionary.TryGetValue(categoryName, out Category category))
            {
                for (int i = 0; i < category.Placeables.Length; i++)
                {
                    if (category.Placeables[i].PlaceableID == placeableId)
                        return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// 
        /// </summary>
        public static List<string> GetPlaceableNames()
        {
            List<string> names = new List<string>();

            for (int i = 0; i < Instance.m_PlaceableCategories.Length; i++)
            {
                foreach (var placeable in Instance.m_PlaceableCategories[i].Placeables)
                {
                    if (placeable.PlaceableName != string.Empty)
                        names.Add(placeable.PlaceableName);
                    else
                        names.Add(placeable.name);
                }
            }

            return names;
        }

        /// <summary>
        /// 
        /// </summary>
        public static List<string> GetPlaceableNamesFull() 
        {
            List<string> names = new List<string>();

            for (int i = 0; i < Instance.m_PlaceableCategories.Length; i++)
            {
                foreach (var placeable in Instance.m_PlaceableCategories[i].Placeables)
                {
                    if (placeable.PlaceableName != string.Empty)
                        names.Add(Instance.m_PlaceableCategories[i].Name + "/" + placeable.PlaceableName);
                    else
                        names.Add(Instance.m_PlaceableCategories[i].Name + "/" + placeable.name);
                }
            }

            return names;
        }
        #endregion

        #region Utilities
        public static StructureManager GetStructurePrefab() => Instance != null ? Instance.m_Structure : null;
        public static Material GetPlaceAllowedMaterial() => Instance != null ? Instance.m_PlacementAllowedMaterial : null;
        public static Material GetPlaceDeniedMaterial() => Instance != null ? Instance.m_PlacementDeniedMaterial : null;
        public static LayerMask GetFreePlacementMask() => Instance != null ? Instance.m_FreePlacementMask : (LayerMask)0;
        public static LayerMask GetEntityMask() => Instance != null ? Instance.m_EntityMask : (LayerMask)0;
        #endregion
    }
}