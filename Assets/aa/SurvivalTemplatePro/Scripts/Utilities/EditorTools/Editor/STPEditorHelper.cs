using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace SurvivalTemplatePro
{
    public static class STPEditorHelper
    {
        public static int IndexOfString(string str, string[] allStrings)
        {
            for (int i = 0; i < allStrings.Length; i++)
            {
                if (allStrings[i] == str)
                    return i;
            }

            return 0;
        }

        public static string StringAtIndex(int i, string[] allStrings) => allStrings.Length > i ? allStrings[i] : "";

        public static void FixedPropertyField(FieldInfo propFieldInfo, Rect position, SerializedProperty prop, GUIContent label, bool children)
        {
            var multiline = GetPropertyAttribute<MultilineAttribute>(propFieldInfo);
            var range = GetPropertyAttribute<RangeAttribute>(propFieldInfo);

            if (range != null)
            {
                if (prop.propertyType == SerializedPropertyType.Float)
                    EditorGUI.Slider(position, prop, range.min, range.max, label);
                else if (prop.propertyType == SerializedPropertyType.Integer)
                    EditorGUI.IntSlider(position, prop, (int)range.min, (int)range.max, label);
                else
                    EditorGUI.PropertyField(position, prop, label);
            }
            else if (multiline != null)
            {
                if (prop.propertyType == SerializedPropertyType.String)
                {
                    var style = GUI.skin.label;
                    var size = style.CalcHeight(label, EditorGUIUtility.currentViewWidth);

                    EditorGUI.LabelField(position, label);

                    position.y += size;
                    prop.stringValue = EditorGUI.TextArea(position, prop.stringValue);
                }
                else
                    EditorGUI.PropertyField(position, prop, label, children);
            }
            else
                EditorGUI.PropertyField(position, prop, label, children);
        }

        public static T GetPropertyAttribute<T>(FieldInfo fieldInfo) where T : PropertyAttribute
        {
            var attributes = fieldInfo.GetCustomAttributes(typeof(T), true);
            return attributes != null && attributes.Length > 0 ? (T)attributes[0] : null;
        }
    }
}