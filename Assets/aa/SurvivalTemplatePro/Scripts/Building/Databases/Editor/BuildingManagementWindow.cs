using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SurvivalTemplatePro.BuildingSystem
{
    public class BuildingManagementWindow : EditorWindow
    {
        private int m_SelectedDatabase;
        private Vector2 m_ScrollPos = Vector2.zero;

        private Editor m_CachedInspector;

        private readonly List<ScriptableObject> m_Databases = new List<ScriptableObject>();
        private string[] m_DatabaseNames;


        [MenuItem("Tools/STP/Building Management", false, priority = 8)]
        public static void Init()
        {
            GetWindow<BuildingManagementWindow>(true, "Building");
        }

        private void OnEnable()
        {
            LoadDatabases();

            if (m_Databases != null && m_Databases.Count > 0)
            {
                m_SelectedDatabase = 0;
                CreateInspectorForSelectedSO();
            }
        }

        private void OnProjectChange()
        {
            LoadDatabases();
        }

        private void OnGUI()
        {
            if (m_Databases != null && m_Databases.Count > 0)
            {
                var previousSelectedSurf = m_SelectedDatabase;
                m_SelectedDatabase = GUILayout.Toolbar(m_SelectedDatabase, m_DatabaseNames);

                if (m_SelectedDatabase != previousSelectedSurf)
                    CreateInspectorForSelectedSO();

                // GUI
                EditorGUILayout.Space();

                m_ScrollPos = EditorGUILayout.BeginScrollView(m_ScrollPos);

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                EditorGUILayout.LabelField(m_DatabaseNames[m_SelectedDatabase], STPEditorGUI.CenteredBoldMiniLabel);

                m_CachedInspector.DrawDefaultInspector();

                EditorGUILayout.EndVertical();

                EditorGUILayout.EndScrollView();
            }
        }

        private void LoadDatabases()
        {
            ScriptableObject placeableDatabase = Resources.LoadAll<PlaceableDatabase>("")[0];
            ScriptableObject materialsDatabase = Resources.LoadAll<BuildMaterialsDatabase>("")[0];

            m_Databases.Add(placeableDatabase);
            m_Databases.Add(materialsDatabase);

            m_DatabaseNames = new string[m_Databases.Count];

            for (int i = 0; i < m_Databases.Count; i++)
                m_DatabaseNames[i] = m_Databases[i].name.DoUnityLikeNameFormat();
        }

        private void CreateInspectorForSelectedSO()
        {
            UnityEditor.Editor.CreateCachedEditor(m_Databases[m_SelectedDatabase], null, ref m_CachedInspector);
        }
    }
}