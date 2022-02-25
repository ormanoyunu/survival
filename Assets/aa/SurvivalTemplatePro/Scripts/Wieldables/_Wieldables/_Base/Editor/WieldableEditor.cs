using UnityEditor;
using UnityEngine;

namespace SurvivalTemplatePro.WieldableSystem
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Wieldable), true)]
    public class WieldableEditor : Editor
    {
        protected Wieldable m_Wieldable;


        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (Application.isPlaying || !Application.isEditor)
                return;

            STPEditorGUI.Separator();

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Show Model"))
                m_Wieldable.SetVisibility(true);

            if (GUILayout.Button("Hide Model"))
                m_Wieldable.SetVisibility(false);

            GUILayout.EndHorizontal();
        }

        protected virtual void OnEnable() => m_Wieldable = target as Wieldable;
    }
}
