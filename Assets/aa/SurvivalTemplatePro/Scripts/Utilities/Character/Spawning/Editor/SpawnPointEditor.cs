using UnityEditor;
using UnityEngine;

namespace SurvivalTemplatePro.Gameplay
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(SpawnPoint))]
    public class SpawnPointEditor : Editor
    {
        private SpawnPoint m_Target;


        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            STPEditorGUI.Separator();

            if (GUILayout.Button("Snap to ground"))
                m_Target.SnapToGround();
        }

        private void OnEnable()
        {
            m_Target = serializedObject.targetObject as SpawnPoint;
        }
    }
}