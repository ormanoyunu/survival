using UnityEditor;
using UnityEngine;

namespace SurvivalTemplatePro
{
    public abstract class DatabaseObjectDrawer : PropertyDrawer
    {
        protected abstract string[] AllObjectNames { get; set; }
        protected abstract string[] AllObjectNamesFull { get; set; }

        private bool m_Initialized;


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (Selection.objects.Length > 1)
                return;

            if (!m_Initialized)
                Initialize(property);

            if (AllObjectNames == null)
            {
                EditorGUI.HelpBox(position, "Error!", MessageType.Error);
                return;
            }

            if (property.propertyType == SerializedPropertyType.String)
                EnumPopupWithString(position, property, label);
            else if (property.propertyType == SerializedPropertyType.Integer)
                EnumPopupWithID(position, property, label);
            else
                DoErrorMessage(position);
        }

        protected virtual void EnumPopupWithString(Rect position, SerializedProperty property, GUIContent label)
        {
            int selectedItem = STPEditorHelper.IndexOfString(property.stringValue, AllObjectNames);
            selectedItem = EditorGUI.Popup(position, label.text, selectedItem, AllObjectNamesFull);
            property.stringValue = STPEditorHelper.StringAtIndex(selectedItem, AllObjectNames);
        }

        protected virtual void EnumPopupWithID(Rect position, SerializedProperty property, GUIContent label)
        {
            int selectedItem = IndexOfObject(property.intValue);
            selectedItem = EditorGUI.Popup(position, label.text, selectedItem, AllObjectNamesFull);
            property.intValue = IdOfObject(selectedItem);
        }

        private void Initialize(SerializedProperty property) 
        {
            GetDataFromDatabase(property);
            SetInitialIntValue(property);

            if (AllObjectNamesFull == null)
                AllObjectNamesFull = AllObjectNames;

            m_Initialized = true;
        }

        private void SetInitialIntValue(SerializedProperty property) 
        {
            if (property.propertyType == SerializedPropertyType.Integer)
                property.intValue = IsValidId(property.intValue) ? IdOfObject(0) : property.intValue;
        } 

        protected abstract void GetDataFromDatabase(SerializedProperty property);

        protected virtual int IndexOfObject(int id) => 0;
        protected virtual int IdOfObject(int index) => 0;
        private bool IsValidId(int id) => id <= -1 && id >= 1;

        protected abstract void DoErrorMessage(Rect position);
    }
}