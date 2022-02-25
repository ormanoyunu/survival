using UnityEditor;
using UnityEngine;

namespace SurvivalTemplatePro.InventorySystem
{
    [CustomPropertyDrawer(typeof(CraftingSettings))]
    public class ItemCraftSettingsDrawer : PropertyDrawer
    {
        private const int kPadding = 4;


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            property.isExpanded = true;

            SerializedProperty hasBlueprintProp = property.FindPropertyRelative("m_HasBlueprint");

            GUI.Box(EditorGUI.IndentedRect(position), "");

            if (hasBlueprintProp.boolValue)
            {
                Color previousGUIColor = GUI.color;
                GUI.color = new Color(0.7f, 0.7f, 0.7f, 0.7f);
                GUI.Box(EditorGUI.IndentedRect(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight + kPadding * 2)), "");
                GUI.color = previousGUIColor;
            }

            Rect rect = position;
            rect.x += kPadding;
            rect.y += kPadding;
            rect.width = 16;
            rect.height = EditorGUIUtility.singleLineHeight;

            hasBlueprintProp.boolValue = EditorGUI.Toggle(rect, hasBlueprintProp.boolValue);

            rect.x += 16;
            rect.width = 128f;

            EditorGUI.LabelField(rect, "Has Blueprint");

            if (hasBlueprintProp.boolValue)
            {
                SerializedProperty blueprintProp = property.FindPropertyRelative("m_Blueprint");
                SerializedProperty isCraftableProp = property.FindPropertyRelative("m_IsCraftable");
                SerializedProperty craftDurationProp = property.FindPropertyRelative("m_CraftDuration");
                SerializedProperty allowDismantleProp = property.FindPropertyRelative("m_AllowDismantle");
                SerializedProperty dismantleEfficiencyprop = property.FindPropertyRelative("m_DismantleEfficiency");

                rect = new Rect(
                    position.x + kPadding,
                    position.y + EditorGUIUtility.singleLineHeight + kPadding * 2 + EditorGUIUtility.standardVerticalSpacing,
                    position.width - kPadding * 2,
                    EditorGUI.GetPropertyHeight(blueprintProp, true) + kPadding);

                //rect.y = rect.yMax + 4f;
                rect.height = EditorGUI.GetPropertyHeight(blueprintProp, true);

                EditorGUI.PropertyField(rect, blueprintProp);

                rect.y = rect.yMax + 4f;
                rect.height = EditorGUIUtility.singleLineHeight;

                EditorGUI.PropertyField(rect, isCraftableProp);

                rect.y = rect.yMax + EditorGUIUtility.standardVerticalSpacing;

                if (!isCraftableProp.boolValue)
                    GUI.enabled = false;

                EditorGUI.PropertyField(rect, craftDurationProp, new GUIContent("Craft Duration (seconds)"));

                if (!isCraftableProp.boolValue)
                    GUI.enabled = true;

                rect.y = rect.yMax + EditorGUIUtility.standardVerticalSpacing;

                EditorGUI.PropertyField(rect, allowDismantleProp);

                rect.y = rect.yMax + EditorGUIUtility.standardVerticalSpacing;

                if (!allowDismantleProp.boolValue)
                    GUI.enabled = false;

                EditorGUI.PropertyField(rect, dismantleEfficiencyprop);

                if (!allowDismantleProp.boolValue)
                    GUI.enabled = true;
            }
        }

        public override bool CanCacheInspectorGUI(SerializedProperty property) => false;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty isCraftableProp = property.FindPropertyRelative("m_IsCraftable");

            return isCraftableProp.boolValue ? EditorGUI.GetPropertyHeight(property, true) : EditorGUIUtility.singleLineHeight + kPadding * 2;
        }
    }
}