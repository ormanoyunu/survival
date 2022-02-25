using UnityEditor;
using UnityEngine;

namespace SurvivalTemplatePro
{
    [CustomEditor(typeof(Character), true)]
    public class CharacterEditor : Editor
    {
        private ICharacterModule[] m_Modules;
        private bool m_FoldoutActive = false; 

        private static Color m_GUIColor = new Color(0.8f, 0.8f, 0.8f, 1f);
        private static GUIStyle m_GUIStyle;
        private const string m_ModulesTxt = " Modules";


        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (m_Modules != null && m_Modules.Length > 0 && !Application.isPlaying && Application.isEditor)
            {
                if (m_GUIStyle == null && STPEditorGUI.MiniGreyLabel != null)
                {
                    m_GUIStyle = new GUIStyle(STPEditorGUI.MiniGreyLabel);
                    m_GUIStyle.fontSize = 12;
                }

                EditorGUILayout.Space();

                int index = 1;

                m_FoldoutActive = EditorGUILayout.Foldout(m_FoldoutActive, m_FoldoutActive ? m_ModulesTxt : m_ModulesTxt + "...", true, STPEditorGUI.FoldOutStyle);

                if (m_FoldoutActive)
                {
                    STPEditorGUI.Separator();

                    GUI.color = m_GUIColor;

                    foreach (var module in m_Modules)
                    {
                        GUILayout.Label($" {index}: {module.GetType().Name}".DoUnityLikeNameFormat(), m_GUIStyle);
                        index++;
                    }
                }
            }
        }
         
        private void OnEnable()
        {
            var character = target as Character;

            if (character != null)
                m_Modules = character.GetComponentsInChildren<ICharacterModule>();
        }
    }
}