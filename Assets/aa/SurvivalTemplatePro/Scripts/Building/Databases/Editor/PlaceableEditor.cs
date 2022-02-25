using UnityEditor;
using UnityEngine;

namespace SurvivalTemplatePro.BuildingSystem
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Placeable), true)]
    public class PlaceableObjectEditor : Editor
    {
        private static bool m_ShowGizmos = true;
        private static Color m_GizmosColor = new Color(1f, 0f, 0f, 0.5f);


        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            STPEditorGUI.Separator();

            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical("Box");

            if(GUILayout.Button("Calculate Bounds"))
                (target as Placeable).CalculateLocalBounds();

            EditorGUILayout.Space();

            m_ShowGizmos = EditorGUILayout.Toggle("Show Gizmos", m_ShowGizmos);

            if(m_ShowGizmos)
                m_GizmosColor = EditorGUILayout.ColorField("Gizmos Color", m_GizmosColor);

            EditorGUILayout.EndVertical();
        }

        private void OnSceneGUI()
        {
            if(m_ShowGizmos && Event.current.type == EventType.Repaint)
            {
                Placeable placeable = target as Placeable;
                Transform transform = placeable.transform;

                Handles.color = m_GizmosColor;
                Handles.matrix = Matrix4x4.TRS(transform.position + placeable.LocalBounds.center.LocalToWorld(transform), transform.rotation, placeable.LocalBounds.size);
                Handles.CubeHandleCap(0, Vector3.zero, Quaternion.identity, 1f, EventType.Repaint);
            }
        }
    }
}