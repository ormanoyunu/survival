using UnityEditor;
using UnityEngine;

namespace SurvivalTemplatePro
{
    public abstract class PopupIdElementsDrawer : PropertyDrawer
    {
        protected SerializedProperty Property { get; private set; }
        protected bool Initialized { get; private set; }


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (Selection.objects.Length > 1)
                return;

            Property = property;

            if (!Initialized)
                Initialize();

            EditorGUI.BeginProperty(position, label, property);

            int selectedItem = IndexOfElement(GetIdOfThis());
            selectedItem = EditorGUI.Popup(position, label.text, selectedItem, GetElementNamesFullPath());
            SetIdOfThis(IdOfElement(selectedItem));

            EditorGUI.EndProperty();
        }

        private void Initialize()
        {
            // Set initial id
            SetIdOfThis(IsValidId(GetIdOfThis()) ? IdOfElement(0) : GetIdOfThis());

            Initialized = true;
        }

        protected abstract string[] GetElementNames();
        protected abstract string[] GetElementNamesFullPath();

        protected virtual int GetIdOfThis() => Property.FindPropertyRelative("m_Value").intValue;
        protected virtual void SetIdOfThis(int id) => Property.FindPropertyRelative("m_Value").intValue = id;

        protected abstract int IndexOfElement(int id);
        protected abstract int IdOfElement(int index);

        private bool IsValidId(int id) => id <= -1 && id >= 1;
    }
}