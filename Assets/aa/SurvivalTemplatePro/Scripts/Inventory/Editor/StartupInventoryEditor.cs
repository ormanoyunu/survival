using UnityEditor;
using UnityEngine;

namespace SurvivalTemplatePro.InventorySystem
{
    [CustomEditor(typeof(InventoryStartupItemsInfo))]
    public class StartupInventoryEditor : Editor
    {
        private Inventory m_Inventory;

        private string[] m_ContainerNames;
        private int m_SelectedContainer;

        private SerializedProperty m_Containers;

        private static Character m_Character;
        private static Character m_PrevCharacter; 


        public override void OnInspectorGUI()
        {
            if (m_Character == null)
                EditorGUILayout.HelpBox("Assign the Player prefab.", MessageType.Warning);

            m_Character = EditorGUILayout.ObjectField(m_Character, typeof(Character), true) as Character;

            if (m_Character == null)
                return;

            STPEditorGUI.Separator();

            if (m_Character != null && (m_Inventory == null || (m_Character != m_PrevCharacter)))
            {
                m_Inventory = m_Character.GetComponentInChildren<Inventory>();
                 
                if (m_Inventory != null) 
                {
                    m_Containers = serializedObject.FindProperty("m_ItemContainersStartupItems");
                    CheckContainers();
                }

                m_PrevCharacter = m_Character;
            }

            if (m_Inventory == null)
            {
                EditorGUILayout.HelpBox("No Inventory component found!", MessageType.Error); 

                return;
            }
            else if (m_ContainerNames == null || m_ContainerNames.Length == 0)
            {
                EditorGUILayout.HelpBox("No Inventory Containers found!", MessageType.Error);
                return;
            }

            EditorGUILayout.Space();

            m_SelectedContainer = EditorGUILayout.Popup("Container", m_SelectedContainer, m_ContainerNames);

            STPEditorGUI.Separator();
            EditorGUILayout.Space();

            serializedObject.Update();

            if (m_Inventory.StartupContainers.Length != m_ContainerNames.Length)
                CheckContainers();
     
            var container = m_Containers.GetArrayElementAtIndex(m_SelectedContainer);

            GUILayout.BeginHorizontal();
            GUILayout.Space(16f);
            GUILayout.BeginVertical();

            DoContainerGUI(container);

            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            serializedObject.ApplyModifiedProperties();
        }

        private void DoContainerGUI(SerializedProperty container)
        {
            GUILayout.Label(string.Format("Startup Items ({0})", container.FindPropertyRelative("Name").stringValue + " Container"), EditorStyles.boldLabel);

            var startupItems = container.FindPropertyRelative("StartupItems");

            int i = 0;
            int itemToRemove = -1;

            foreach(SerializedProperty item in startupItems)
            {
                SerializedProperty method = item.FindPropertyRelative("Method");
                SerializedProperty count = item.FindPropertyRelative("Count");

                EditorGUILayout.BeginVertical("Box");

                EditorGUILayout.PropertyField(method);

                EditorGUILayout.BeginHorizontal();

                if ((ItemGenerationMethod)method.enumValueIndex == ItemGenerationMethod.Specific)
                {
                    SerializedProperty specificItem = item.FindPropertyRelative("SpecificItem");

                    GUI.color = STPEditorGUI.HighlightColor1;
                    EditorGUILayout.PropertyField(specificItem, GUIContent.none);
                    GUI.color = Color.white;
                }
                else if((ItemGenerationMethod)method.enumValueIndex == ItemGenerationMethod.RandomFromCategory)
                {
                    SerializedProperty category = item.FindPropertyRelative("Category");

                    GUI.color = STPEditorGUI.HighlightColor1;
                    EditorGUILayout.PropertyField(category, GUIContent.none);
                    GUI.color = Color.white;
                }

                EditorGUILayout.PropertyField(count, GUIContent.none);

                EditorGUILayout.EndHorizontal();

                if (GUILayout.Button("Remove", EditorStyles.miniButton))
                {
                    itemToRemove = i;
                    break;
                }

                EditorGUILayout.EndVertical();

                EditorGUILayout.Space();

                i++;
            }

            EditorGUILayout.Space();

            GUILayout.BeginHorizontal();

            if (itemToRemove != -1)
                startupItems.DeleteArrayElementAtIndex(itemToRemove);

            if (GUILayout.Button("Add New"))
            {
                int addIndex = startupItems.arraySize == 0 ? 0 : startupItems.arraySize - 1;
                startupItems.InsertArrayElementAtIndex(addIndex);
            }

            if (GUILayout.Button("Remove All"))
            {
                if (EditorUtility.DisplayDialog("Remove All Items", "Are you sure you want to remove all items?", "Yes", "Cancel"))
                    startupItems.ClearArray();
            }

            GUILayout.EndHorizontal();
        }

        private int FindContainerIndex(string contName)
        {
            int i = 0;

            foreach(SerializedProperty cont in m_Containers)
            {
                if(cont.FindPropertyRelative("Name").stringValue == contName)
                    return i;

                i++;
            }

            return -1;
        }

        private void AddNewContainer(string contName)
        {
            int addIndex = m_Containers.arraySize == 0 ? 0 : m_Containers.arraySize - 1;

            m_Containers.InsertArrayElementAtIndex(addIndex);

            m_Containers.GetArrayElementAtIndex(addIndex).FindPropertyRelative("Name").stringValue = contName;
        }

        private void CheckContainers()
        {
            serializedObject.Update();

            PullContainerNames();
            CheckContainersExistence();
            CheckContainersOrder();
            CheckContainersName();

            serializedObject.ApplyModifiedProperties();
        }

        private void PullContainerNames()
        {
            m_ContainerNames = new string[m_Inventory.StartupContainers.Length];

            for(int i = 0;i < m_Inventory.StartupContainers.Length;i++)
                m_ContainerNames[i] = m_Inventory.StartupContainers[i].Name;
        }

        private void CheckContainersExistence()
        {
            if(m_ContainerNames.Length != m_Containers.arraySize)
            {
                foreach(var containerName in m_ContainerNames)
                {
                    int idxOfContainer = FindContainerIndex(containerName);

                    if(idxOfContainer == -1)
                        AddNewContainer(containerName);
                }
            }
        }

        private void CheckContainersOrder()
        {
            for(int i = 0;i < m_ContainerNames.Length;i++)
            {
                int containerIdx = FindContainerIndex(m_ContainerNames[i]);

                // If the order is not right
                if(containerIdx != -1 && containerIdx != i)
                    m_Containers.MoveArrayElement(containerIdx, i);
            }
        }

        private void CheckContainersName()
        {
            int i = 0;

            foreach (SerializedProperty container in m_Containers)
            {
                container.FindPropertyRelative("Name").stringValue = m_ContainerNames[Mathf.Clamp(i, 0, m_ContainerNames.Length - 1)];
                i++;
            }
        }
    }
}