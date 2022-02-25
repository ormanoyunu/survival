using UnityEditor;
using UnityEngine;

namespace SurvivalTemplatePro
{
    [CustomPropertyDrawer(typeof(BoxGroupAttribute))]
    public class BoxGroupDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Color prevColor = GUI.color;
            GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, 0.52f);

            var rect = position;
            rect.height = EditorGUIUtility.singleLineHeight;

            GUI.Box(EditorGUI.IndentedRect(rect), "");

            rect.x += 4;
            EditorGUI.LabelField(rect, label, EditorStyles.miniBoldLabel);

            GUI.color = prevColor;

            rect.x -= 4;

            rect = new Rect(
                rect.x,
                rect.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing,
                rect.width,
                rect.height
                );

            // Children
            foreach (SerializedProperty child in property)
            {
                rect.height = EditorGUI.GetPropertyHeight(child);
                EditorGUI.PropertyField(rect, child);
                rect.y = rect.yMax + EditorGUIUtility.standardVerticalSpacing;
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float totalHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            foreach(SerializedProperty child in property)
                totalHeight += EditorGUI.GetPropertyHeight(child) + EditorGUIUtility.standardVerticalSpacing;

            return totalHeight;
        }
    }
}