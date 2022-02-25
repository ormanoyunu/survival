using SurvivalTemplatePro.WieldableSystem;
using System;
using UnityEditor;
using UnityEngine;

namespace SurvivalTemplatePro
{
    [CustomPropertyDrawer(typeof(WieldableEffectAttribute))]
    public class WieldableEffectDrawer : PropertyDrawer
    {
        private static IWieldableEffectsManager m_EffectsManager;
        private static string[] m_EffectHandlerNames;
        private static string[] m_EffectNames;


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (m_EffectNames == null || m_EffectNames.Length == 0)
                DefaultInspector(position, property, label);

            if (property.propertyType == SerializedPropertyType.Integer)
            {
                if (TryGetEffectsManager(property, out m_EffectsManager) && m_EffectsManager != null)
                {
                    GetEffectNames();

                    if (WieldableEffectAttribute.PlayType == WieldableEffectPlayType.PlayEffect)
                        DoPlayEffectPopup(position, property, label);
                    else if (WieldableEffectAttribute.PlayType == WieldableEffectPlayType.StopEffect)
                        DoDisableEffectPopup(position, property, label);

                    return;
                }
            }

            ErrorMessage(position);
        }

        private void GetEffectNames() 
        {
            if (WieldableEffectAttribute.PlayType == WieldableEffectPlayType.PlayEffect)
                m_EffectNames = m_EffectsManager.GetAllEffectNames();
            else if (WieldableEffectAttribute.PlayType == WieldableEffectPlayType.StopEffect)
                m_EffectHandlerNames = m_EffectsManager.GetAllEffectHandlerNames();
        }

        private void DoPlayEffectPopup(Rect position, SerializedProperty property, GUIContent label)
        {
            if (m_EffectNames == null || m_EffectNames.Length == 0)
                return;

            int selectedItem = m_EffectsManager.IndexOfEffectWithId(property.intValue);
            selectedItem = Mathf.Clamp(selectedItem, 0, m_EffectNames.Length - 1);
            selectedItem = EditorGUI.Popup(position, label.text, selectedItem, m_EffectNames);
            property.intValue = Animator.StringToHash(m_EffectNames[selectedItem]);
        }

        private void DoDisableEffectPopup(Rect position, SerializedProperty property, GUIContent label)
        {
            if (m_EffectHandlerNames == null || m_EffectHandlerNames.Length == 0)
                return;

            int selectedItem = m_EffectsManager.IndexOfEffectHandlerWithId(property.intValue);
            selectedItem = Mathf.Clamp(selectedItem, 0, m_EffectHandlerNames.Length - 1);
            selectedItem = EditorGUI.Popup(position, label.text, selectedItem, m_EffectHandlerNames);
            property.intValue = Animator.StringToHash(m_EffectHandlerNames[selectedItem]);
        }

        private bool TryGetEffectsManager(SerializedProperty property, out IWieldableEffectsManager manager)
        {
            var component = property.serializedObject.targetObject as Component;

            if (component == null)
                throw new InvalidCastException("Couldn't cast targetObject");

            manager = component.GetComponentInParent<IWieldableEffectsManager>();

            return manager != null;
        }

        private void ErrorMessage(Rect position)
        {
            EditorGUI.HelpBox(position, "The 'WieldableEffect' attribute runs just on integers.", MessageType.Error);
        }

        private void DefaultInspector(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.PropertyField(position, property, label);
        }

        private WieldableEffectAttribute WieldableEffectAttribute => (WieldableEffectAttribute)attribute;
    }
}