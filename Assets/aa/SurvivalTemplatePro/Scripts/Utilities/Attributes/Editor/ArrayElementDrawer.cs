using UnityEditor;
using UnityEngine;

namespace SurvivalTemplatePro
{
    [CustomPropertyDrawer(typeof(ArrayElementAttribute))]
	public class ArrayElementDrawer : PropertyDrawer
	{
        private static GUIStyle m_PrefixStyle = EditorStyles.label;
            //new GUIStyle() { alignment = TextAnchor.MiddleLeft, fontStyle = FontStyle.Normal, fontSize = 11, normal = new GUIStyleState() { textColor = new Color(1f, 1f, 1f, 0.8f) } };

        private static GUIStyle s_SuffixStyle = 
            new GUIStyle() { alignment = TextAnchor.MiddleRight, fontStyle = FontStyle.Normal, fontSize = 10, normal = new GUIStyleState() { textColor = new Color(1f, 1f, 1f, 0.8f) } };


		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) 
		{
            if (property.propertyType != SerializedPropertyType.Float && property.propertyType != SerializedPropertyType.Integer)
            {
                EditorGUI.PropertyField(position, property, label);
                return;
            }

            var attr = attribute as ArrayElementAttribute;

            // Prefix
            EditorGUI.LabelField(position, attr.Prefix, m_PrefixStyle);

            // Property
            Rect rect = new Rect(position);

            rect.x += attr.LeftIndent;
            rect.xMax -= (attr.LeftIndent + attr.RightIndent);

            // Draw field depending on type
            if (property.propertyType == SerializedPropertyType.Integer)
                property.intValue = EditorGUI.IntField(rect, property.intValue);
            else if (property.propertyType == SerializedPropertyType.Float)
                property.floatValue = EditorGUI.FloatField(rect, property.floatValue);

            position.xMax -= 6f;

            // Suffix
            EditorGUI.LabelField(position, attr.Suffix, s_SuffixStyle);
        }
	}
}