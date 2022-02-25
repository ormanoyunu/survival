using UnityEditor;
using UnityEngine;

namespace SurvivalTemplatePro.WieldableSystem
{
    [CustomEditor(typeof(FPWieldableMotion))]
    public class FPWieldableMotionEditor : Editor
    {
        private FPWieldableMotion m_FPMotion;

        private const string k_ModelName = "Model";
        private const string k_PivotName = "Pivot";


        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (Application.isPlaying)
                GUI.enabled = false;

            STPEditorGUI.Separator();

            EditorGUILayout.Space();

            EditorGUILayout.HelpBox("When playing, expand the desired state and click the 'Visualize' button to edit the state's motion.", MessageType.Info);      

            if (m_FPMotion.transform.FindDeepChild("Pivot") == null)
            {
                STPEditorGUI.Separator();

                GUILayout.BeginVertical(EditorStyles.helpBox);

                EditorGUILayout.HelpBox("Pivot and/or model not found", MessageType.Error);

                if (GUILayout.Button("Create Pivot and/or Model"))
                {
                    AddNecessaryObjects();

                    EditorUtility.SetDirty(m_FPMotion.gameObject);
                    PrefabUtility.RecordPrefabInstancePropertyModifications(m_FPMotion.gameObject);
                }

                GUILayout.EndVertical();

                STPEditorGUI.Separator();
            }

            if (Application.isPlaying)
                GUI.enabled = true;
        }

        private void OnEnable()
        {
            m_FPMotion = target as FPWieldableMotion;
        }

        private void AddNecessaryObjects()
        {
            Transform modelTransform = m_FPMotion.transform.Find("Model");
            Transform pivotTransform = m_FPMotion.transform.Find("Pivot");

            if (modelTransform == null)
            {
                var modelObj = new GameObject(k_ModelName);
                modelObj.transform.SetParent(m_FPMotion.transform);
            }

            if (pivotTransform == null)
            {
                var pivotObj = new GameObject(k_PivotName);
                pivotObj.transform.SetParent(m_FPMotion.transform);
                pivotObj.AddComponent<FPWieldablePivotGizmo>();
            }
        }
    }
}