using UnityEditor;
using UnityEngine;

namespace SurvivalTemplatePro.Gameplay
{
    [CustomEditor(typeof(MoveToSpawnPoint))]
    public class MoveToSpawnPointEditor : Editor
    {
        private MoveToSpawnPoint m_Target;


        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (m_Target == null)
            {
                EditorGUILayout.HelpBox("No player spawner could be found!", MessageType.Error, true);
                return;
            }

            STPEditorGUI.Separator();

            if (GUILayout.Button("Move to random spawn point"))
                m_Target.MoveToRandomPoint();
        }

        private void OnEnable()
        {
            m_Target = serializedObject.targetObject as MoveToSpawnPoint;
        }
    }
}