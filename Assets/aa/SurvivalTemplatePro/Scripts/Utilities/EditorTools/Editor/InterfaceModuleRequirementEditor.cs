using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
namespace SurvivalTemplatePro
{
    public class InterfaceModuleRequirementEditor<T> : Editor
    {
        private bool m_FoldoutActive = false;
        private Component m_Component;
        private Type m_InterfaceType;
        private Dictionary<Type, bool> m_ModuleFieldsDictionary;

        private static Color m_ErrorColor = new Color(0.9f, 0.65f, 0.65f, 1f);
        private static Color m_StandardColor = new Color(0.8f, 0.8f, 0.8f, 0.9f);
        private static Dictionary<Type, Type[]> m_AllModuleTypes;


        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (m_ModuleFieldsDictionary != null && m_ModuleFieldsDictionary.Count > 0 && !Application.isPlaying)
            {
                EditorGUILayout.Space();

                GUI.color = m_StandardColor;
                GUILayout.BeginVertical(EditorStyles.helpBox);

                m_FoldoutActive = EditorGUILayout.Foldout(m_FoldoutActive, m_FoldoutActive ? GetRequiredString() : GetRequiredString() + "...", true, STPEditorGUI.FoldOutStyle);

                if (m_FoldoutActive)
                {
                    STPEditorGUI.Separator();

                    int index = 1;

                    foreach (var moduleType in m_ModuleFieldsDictionary)
                    {
                        if (!moduleType.Value)
                        {
                            GUI.color = m_ErrorColor;
                            DoErrorMessage(moduleType.Key, index);
                        }
                        else
                        {
                            GUI.color = m_StandardColor;
                            GUILayout.BeginHorizontal();
                            DoOkMessage(moduleType.Key, index);
                            GUILayout.EndHorizontal();
                        }

                        index++;
                    }
                }

                GUILayout.EndVertical();
            }
        }

        protected virtual string GetRequiredString() => "Required";

        private void OnEnable()
        {
            m_Component = target as Component;
            m_InterfaceType = typeof(T);

            if (m_AllModuleTypes == null)
                m_AllModuleTypes = new Dictionary<Type, Type[]>();

            if (!m_AllModuleTypes.ContainsKey(m_InterfaceType))
                m_AllModuleTypes.Add(m_InterfaceType, GetAllInterfaceModuleTypes());

            m_ModuleFieldsDictionary = new Dictionary<Type, bool>();
            CreateInterfaceModuleFieldsDictionary();
            m_FoldoutActive = m_ModuleFieldsDictionary.ContainsValue(false);
        }

        private Type[] GetAllInterfaceModuleTypes()
        {
            var allModuleTypes = new List<Type>();

            foreach (var type in m_InterfaceType.Assembly.GetTypes())
            {
                if (type.IsInterface && type.GetInterface(m_InterfaceType.Name) != null)
                    allModuleTypes.Add(type);
            }

            return allModuleTypes.ToArray();
        }

        private void CreateInterfaceModuleFieldsDictionary()
        {
            Type charType = m_Component.GetType();
            FieldInfo[] privateFields = charType.GetFields(BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.NonPublic);

            foreach (var privateField in privateFields)
            {
                var fieldType = privateField.FieldType;

                if (fieldType.IsInterface && fieldType.GetInterface(m_InterfaceType.Name) != null)
                    m_ModuleFieldsDictionary.Add(fieldType, CharacterHasModule(fieldType));
            }
        }

        private bool CharacterHasModule(Type interfaceModule)
        {
            return m_Component.transform.root == m_Component.transform || m_Component.transform.root.GetComponentInChildren(interfaceModule) != null;
        }

        private void DoOkMessage(Type interfaceModule, int index)
        {
            GUILayout.Toggle(true, $" {index}: {interfaceModule.Name}".DoUnityLikeNameFormat(), EditorStyles.radioButton);
        }

        private void DoErrorMessage(Type interfaceModule, int index)
        {
            EditorGUILayout.HelpBox($"{index}: This behaviour requires a ''{interfaceModule.Name}'' {m_InterfaceType.Name.DoUnityLikeNameFormat()}.", MessageType.Error);
        }
    }
}
