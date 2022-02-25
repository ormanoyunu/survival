using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

namespace SurvivalTemplatePro
{
    public class SceneSetupWindow : EditorWindow
    {
        private Vector2 m_CurrentScrollPosition;

        private SceneSetupInfo m_SetupInfo;
        private SerializedObject m_SetupSerializedObj;


        [MenuItem("Tools/STP/Scene Setup", false)]
        public static void Init() 
        {
            var window = GetWindow<SceneSetupWindow>(true, "Scene Setup");
            window.minSize = new Vector2(512, 512);
        }

        private void OnEnable()
        {
            var sceneSetups = Resources.LoadAll<SceneSetupInfo>("Editor/");

            foreach (var item in sceneSetups)
            {
                m_SetupInfo = item;
                m_SetupSerializedObj = new SerializedObject(m_SetupInfo);
                break;
            }
        }

        private void OnGUI()
        {
            if (m_SetupInfo != null)
            {
                m_SetupSerializedObj.Update();

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                m_CurrentScrollPosition = EditorGUILayout.BeginScrollView(m_CurrentScrollPosition);

                EditorGUILayout.HelpBox("Use this tool to quickly Clean/Setup your scenes for ''Survival Template PRO''.", MessageType.Info);

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                DrawPrefabsInfo();

                EditorGUILayout.Space();

                DrawMiscInfo();
                EditorGUILayout.EndVertical();

                if (GUILayout.Button("Reset Settings!", STPEditorGUI.StandardButtonStyle, GUILayout.MaxWidth(100f)))
                {
                    m_SetupInfo.ResetToDefault();
                    EditorUtility.SetDirty(m_SetupInfo);
                }

                GUILayout.FlexibleSpace();

                EditorGUILayout.BeginHorizontal();
                
                if (GUILayout.Button("Setup Scene", STPEditorGUI.LargeButtonStyle))
                    StartSceneSetup(false);

                if (GUILayout.Button("Revert Scene Setup", STPEditorGUI.LargeButtonStyle))
                    StartSceneSetup(true);

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.EndScrollView();
                EditorGUILayout.EndVertical();

                m_SetupSerializedObj.ApplyModifiedProperties();
            }
        }

        private void DrawPrefabsInfo()
        {
            EditorGUILayout.LabelField("General", STPEditorGUI.BoldMiniGreyLabel);
            STPEditorGUI.Separator(new Color(0.4f, 0.4f, 0.4f, 1f));

            EditorGUILayout.PropertyField(m_SetupSerializedObj.FindProperty("m_SceneSetupPrefabs"));
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(m_SetupSerializedObj.FindProperty("m_BaseObjects"));

            GUI.enabled = true;
        }

        private void DrawMiscInfo()
        {
            STPEditorGUI.Separator(new Color(0.4f, 0.4f, 0.4f, 1f));

            EditorGUILayout.PropertyField(m_SetupSerializedObj.FindProperty("m_AddSceneToBuild"));
        }

        private void StartSceneSetup(bool revertSetup) 
        {
            try
            {
                if (m_SetupInfo.AddSceneToBuildSettings) SetupBuildSettingsForScene(revertSetup);

                if (!revertSetup)
                {
                    SetupScene(m_SetupInfo.SceneSetupPrefabs);
                    EditorUtility.DisplayDialog("Scene Setup Complete", "This scene is ready to use with Survival Template Pro.", "OK");
                }
                else
                {
                    RevertScene(m_SetupInfo.SceneSetupPrefabs);
                    EditorUtility.DisplayDialog("Scene Setup Reverted", "This scene can't be used with Survival Template Pro anymore.", "OK");
                }
            }
            catch
            {
                EditorUtility.DisplayDialog("Scene Setup Failed!", "The Scene Setup was unsuccessful.", "OK");
            }    
        }

        private void SetupScene(SceneSetupInfo.SceneSetupPrefab[] setupPrefabs)
        {
            foreach (var setupPrefab in setupPrefabs)
            {
                bool shouldSpawn = false;

                if (setupPrefab.SetupType == SceneSetupInfo.PrefabSetupType.DontReplaceInstance)
                    shouldSpawn = (GetPrefabInstance(setupPrefab.Prefab) == null);
                else if (setupPrefab.SetupType == SceneSetupInfo.PrefabSetupType.ReplaceInstance)
                {
                    var prefabInstance = GetPrefabInstance(setupPrefab.Prefab);

                    if(prefabInstance != null)
                        DestroyImmediate(prefabInstance);

                    shouldSpawn = true;
                }

                if (shouldSpawn)
                    PrefabUtility.InstantiatePrefab(setupPrefab.Prefab);
            }

            if (m_SetupInfo.SetupBaseObjects)
            {
                GameObject tempObj;

                for (int i = 0; i < m_SetupInfo.BaseObjectNames.Length; i++)
                {
                    if (!string.IsNullOrEmpty(m_SetupInfo.BaseObjectNames[i]))
                    {
                        tempObj = GameObject.Find(m_SetupInfo.BaseObjectNames[i]);

                        if (tempObj != null && tempObj.transform.root == tempObj.transform)
                            continue;

                        new GameObject(m_SetupInfo.BaseObjectNames[i]);
                    }
                }
            }
        }

        private void RevertScene(SceneSetupInfo.SceneSetupPrefab[] setupPrefabs)
        {
            foreach (var setupPrefab in setupPrefabs)
            {
                if (setupPrefab.SetupType != SceneSetupInfo.PrefabSetupType.Ignore)
                    DestroyImmediate(GetPrefabInstance(setupPrefab.Prefab));
            }

            if (m_SetupInfo.SetupBaseObjects)
            {
                GameObject objectToDestroy;

                for (int i = 0; i < m_SetupInfo.BaseObjectNames.Length; i++)
                {
                    if (!string.IsNullOrEmpty(m_SetupInfo.BaseObjectNames[i]))
                    {
                        objectToDestroy = GameObject.Find(m_SetupInfo.BaseObjectNames[i]);

                        if (objectToDestroy != null)
                        {
                            if (objectToDestroy.transform.root == objectToDestroy.transform)
                            {
                                foreach (Transform child in objectToDestroy.transform)
                                    child.SetParent(null);

                                DestroyImmediate(objectToDestroy);
                            }
                        }
                    }
                }
            }
        }

        private GameObject GetPrefabInstance(GameObject prefab) 
        {
            GameObject[] prefabInstances = FindObjectsOfType(typeof(GameObject)).Select(g => g as GameObject).Where(g => g.name == prefab.name).ToArray();

            foreach (var instance in prefabInstances)
            {
                if (PrefabUtility.IsPartOfPrefabInstance(instance))
                {
                    if (PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(instance) == PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(prefab))
                        return instance;
                }
            }

            return null;
        }
        
        private void SetupBuildSettingsForScene(bool remove) 
        {
            List<EditorBuildSettingsScene> editorBuildSettingsScenes = new List<EditorBuildSettingsScene>();
            EditorBuildSettingsScene sceneToRemove = null;
            string currentScenePath = SceneManager.GetActiveScene().path;

            foreach (var scene in EditorBuildSettings.scenes)
            {
                // Check if the current scene is in the build settings
                if (scene.path == currentScenePath)
                {
                    if (remove)
                        sceneToRemove = scene;
                    else
                        return;
                }

                // Check if the added scenes are not duplicate/null
                if (scene != null && !editorBuildSettingsScenes.Contains(scene))
                    editorBuildSettingsScenes.Add(scene);
            }
            
            // Remove the current scene from the build settings
            if (remove)
                editorBuildSettingsScenes.Remove(sceneToRemove);
            // Add the current scene to the build settings
            if (!remove)
                editorBuildSettingsScenes.Add(new EditorBuildSettingsScene(currentScenePath, true));

            EditorBuildSettings.scenes = editorBuildSettingsScenes.ToArray();
        }
    }
}
