using System;
using UnityEditor;
using UnityEngine;

namespace SurvivalTemplatePro
{
    public static class STPEditorGUI
    {
        public static event Action onOneSecondPassed;

        private static double m_OneSecondTimer;

        public static GUIStyle LargeTitleLabel { get; private set; }
        public static GUIStyle MediumTitleLabel { get; private set; }
        public static GUIStyle SmallTitleLabel { get; private set; }
        public static GUIStyle CenteredMiniLabel { get; private set; }
        public static GUIStyle CenteredBoldMiniLabel { get; private set; }
        public static GUIStyle MiniGreyLabel { get; private set; }
        public static GUIStyle BoldMiniGreyLabel { get; private set; }
        public static GUIStyle FoldOutStyle { get; private set; }

        public static GUIStyle StandardButtonStyle { get; private set; }
        public static GUIStyle LargeButtonStyle { get; private set; }

        public static Color HighlightColor1 { get; private set; } = new Color(1, 1, 1, 1);
        public static Color HighlightColor2 { get; private set; } = new Color(1, 1, 1, 1);

        private static GUIStyle m_SeparatorStyle;


        static STPEditorGUI()
        {
            EditorApplication.update += OnEditorUpdate;
           
            m_SeparatorStyle = new GUIStyle();
            m_SeparatorStyle.normal.background = EditorGUIUtility.whiteTexture;
            m_SeparatorStyle.stretchWidth = true;
            m_SeparatorStyle.margin = new RectOffset(0, 0, 7, 7);

            LargeTitleLabel = new GUIStyle(EditorStyles.boldLabel);
            LargeTitleLabel.fontSize = 12;
            LargeTitleLabel.normal.textColor = new Color(1, 1, 1, 0.75f);
            LargeTitleLabel.alignment = TextAnchor.MiddleCenter;

            MediumTitleLabel = new GUIStyle(LargeTitleLabel);
            MediumTitleLabel.fontSize = 11;
            MediumTitleLabel.alignment = TextAnchor.MiddleLeft;

            CenteredMiniLabel = new GUIStyle(EditorStyles.centeredGreyMiniLabel);
            CenteredMiniLabel.normal.textColor = new Color(1, 1, 1, 0.6f);
            CenteredMiniLabel.wordWrap = true;
            CenteredMiniLabel.fontSize = 11;
            CenteredMiniLabel.padding.left += 16;
            CenteredMiniLabel.padding.right += 16;

            CenteredBoldMiniLabel = new GUIStyle(CenteredMiniLabel);
            CenteredBoldMiniLabel.fontStyle = FontStyle.Bold;

            SmallTitleLabel = new GUIStyle(CenteredMiniLabel);
            SmallTitleLabel.alignment = TextAnchor.MiddleLeft;
            SmallTitleLabel.padding.left = 6;
            SmallTitleLabel.padding.right = 6;

            MiniGreyLabel = new GUIStyle(EditorStyles.centeredGreyMiniLabel);
            MiniGreyLabel.alignment = TextAnchor.MiddleLeft;

            BoldMiniGreyLabel = new GUIStyle(MiniGreyLabel);
            BoldMiniGreyLabel.fontStyle = FontStyle.Bold;

            FoldOutStyle = new GUIStyle(EditorStyles.foldout);
            FoldOutStyle.fontStyle = FontStyle.Italic;
            FoldOutStyle.normal.textColor = new Color(1, 1, 1, 0.65f);
            FoldOutStyle.fontSize = 11;

            StandardButtonStyle = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Scene).button;
            StandardButtonStyle.fontStyle = FontStyle.Normal;
            StandardButtonStyle.alignment = TextAnchor.MiddleCenter;
            StandardButtonStyle.padding = new RectOffset(5, 0, 2, 2);
            StandardButtonStyle.fontSize = 12;
            StandardButtonStyle.normal.textColor = new Color(1f, 1f, 1f, 0.85f);

            LargeButtonStyle = new GUIStyle(StandardButtonStyle);
            LargeButtonStyle.padding.top = 6;
            LargeButtonStyle.padding.bottom = 6;
        }

        private static void OnEditorUpdate()
        {
            if (EditorApplication.timeSinceStartup > m_OneSecondTimer + 1f)
            {
                onOneSecondPassed?.Invoke();
                m_OneSecondTimer = EditorApplication.timeSinceStartup;
            }
        }

        private static readonly Color splitterColor = EditorGUIUtility.isProSkin ? new Color(0.2f, 0.2f, 0.2f) : new Color(0.5f, 0.5f, 0.5f);

        public static void Separator(Color rgb, float thickness = 1)
        {
            Rect position = GUILayoutUtility.GetRect(GUIContent.none, m_SeparatorStyle, GUILayout.Height(thickness));

            if (Event.current.type == EventType.Repaint)
            {
                Color restoreColor = GUI.color;
                GUI.color = rgb;
                m_SeparatorStyle.Draw(position, false, false, false, false);
                GUI.color = restoreColor;
            }
        }

        public static void Separator(float thickness, GUIStyle splitterStyle)
        {
            Rect position = GUILayoutUtility.GetRect(GUIContent.none, splitterStyle, GUILayout.Height(thickness));

            if (Event.current.type == EventType.Repaint)
            {
                Color restoreColor = GUI.color;
                GUI.color = splitterColor;
                splitterStyle.Draw(position, false, false, false, false);
                GUI.color = restoreColor;
            }
        }

        public static void Separator(float thickness = 1) => Separator(thickness, m_SeparatorStyle);

        public static void Separator(Rect position)
        {
            if (Event.current.type == EventType.Repaint)
            {
                Color restoreColor = GUI.color;
                GUI.color = splitterColor;
                m_SeparatorStyle.Draw(position, false, false, false, false);
                GUI.color = restoreColor;
            }
        }
    }
}