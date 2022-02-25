using UnityEditor;
using UnityEngine;

namespace SurvivalTemplatePro.WorldManagement
{
    [CustomEditor(typeof(WorldManagement.WorldManager))]
    public class WorldManagerEditor : Editor
    {
        private WorldManagement.WorldManager m_WorldManager;


        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            STPEditorGUI.Separator();
            EditorGUILayout.Space();

            GUILayout.Label("Debugging", EditorStyles.boldLabel);

            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Debug info will be shown at play-time!", MessageType.Info);
                return;
            }

            m_WorldManager.DisplayDebugInfo();

            GUI.changed = true;
        }

        private void OnEnable()
        {
            m_WorldManager = serializedObject.targetObject as WorldManagement.WorldManager;
        }
    }
}
