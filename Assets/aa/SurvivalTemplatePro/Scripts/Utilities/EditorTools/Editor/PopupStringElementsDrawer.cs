using UnityEditor;
using UnityEngine;

namespace SurvivalTemplatePro
{
    public abstract class PopupStringElementsDrawer : PropertyDrawer
    {
        protected SerializedProperty Property { get; private set; }


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (Selection.objects.Length > 1)
                return;

            Property = property;

            EditorGUI.BeginProperty(position, label, property);

            int selectedItem = STPEditorHelper.IndexOfString(GetNameOfThis(), GetElementNames());
            selectedItem = EditorGUI.Popup(position, label.text, selectedItem, GetElementNamesFullPath());
            SetNameOfThis(STPEditorHelper.StringAtIndex(selectedItem, GetElementNames()));

            EditorGUI.EndProperty();
        }

        protected abstract string[] GetElementNames();
        protected abstract string[] GetElementNamesFullPath();

        protected virtual string GetNameOfThis() => Property.FindPropertyRelative("m_Value").stringValue;
        protected virtual void SetNameOfThis(string name) => Property.FindPropertyRelative("m_Value").stringValue = name;
    }
}
