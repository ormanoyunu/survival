using UnityEditor;
using UnityEngine;

namespace SurvivalTemplatePro
{
    [CustomPropertyDrawer(typeof(ShowIfAttribute))]
	public class ShowIfDrawer : PropertyDrawer
	{
		/// <summary>
		/// A helper property to check for RangeAttribute.
		/// </summary>
		private RangeAttribute rangeAttribute
		{
			get
			{
				var attributes = fieldInfo.GetCustomAttributes(typeof(RangeAttribute), true);
				return attributes != null && attributes.Length > 0 ? (RangeAttribute)attributes[0] : null;
			}
		}


		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			property.isExpanded = true;

			if (CanShow(property))
			{
				if (property.hasChildren)
				{
					foreach (SerializedProperty child in property)
					{
						position.height = EditorGUI.GetPropertyHeight(child, true);

						STPEditorHelper.FixedPropertyField(fieldInfo, position, child, null, true);

						position.y = position.yMax + EditorGUIUtility.standardVerticalSpacing;
					}
				}
				else
				{
					STPEditorHelper.FixedPropertyField(fieldInfo, position, property, label, true);
				}
			}
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			property.isExpanded = true;

			if (!CanShow(property))
				// HACK. if we don't use this, extra spacing height will be added by Unity.
				return -EditorGUIUtility.standardVerticalSpacing;
			else
			{
				float height = EditorGUI.GetPropertyHeight(property, true);
				
				// Remove the parent height, because we don't show it
				if(property.hasChildren)
					height -= (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);

				return height;
			}
		}

		private bool CanShow(SerializedProperty property)
		{
			var attr = (ShowIfAttribute)attribute;

			string parentPath = property.GetParentPath();
			SerializedProperty conditionProperty = property.serializedObject.FindProperty(parentPath + (parentPath != string.Empty ? "." : "") + attr.m_ConditionProperty);

			if (conditionProperty != null)
			{
				if(conditionProperty.propertyType == SerializedPropertyType.Boolean)
					return attr.m_RequiredBool == conditionProperty.boolValue;
				else if(conditionProperty.propertyType == SerializedPropertyType.ObjectReference)
					return attr.m_RequiredBool == (conditionProperty.objectReferenceValue != null);
				else if(conditionProperty.propertyType == SerializedPropertyType.Integer)
					return attr.m_RequiredInt == conditionProperty.intValue;
				else if(conditionProperty.propertyType == SerializedPropertyType.Enum)
					return attr.m_RequiredInt == conditionProperty.intValue;
				else if(conditionProperty.propertyType == SerializedPropertyType.Float)
					return attr.m_RequiredFloat == conditionProperty.floatValue;
				else if(conditionProperty.propertyType == SerializedPropertyType.String)
					return attr.m_RequiredString == conditionProperty.stringValue;
				else if(conditionProperty.propertyType == SerializedPropertyType.Vector3)
					return attr.m_RequiredVector3 == conditionProperty.vector3Value;
			}

			return false;
		}
	}
}