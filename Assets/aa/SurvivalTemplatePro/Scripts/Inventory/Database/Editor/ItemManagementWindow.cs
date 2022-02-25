using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace SurvivalTemplatePro
{
	public class ItemManagementWindow : EditorWindow
	{
		/// <summary>
		/// This is a hack for avoiding an issue with the ReorderableList's DrawHeader method. 
		/// </summary>
		public static bool DrawingItemWindow { get; private set; }

		private int m_SelectedTab;
		private SerializedObject m_ItemDatabase;

		private ReorderableList m_CategoryList;
		private ReorderableList m_ItemList;

		private Vector2 m_CategoriesScrollPos;
		private Vector2 m_ItemsScrollPos;
		private Vector2 m_ItemInspectorScrollPos;


		[MenuItem("Tools/STP/Item Management", false, priority = 8)]
		public static void Init()
		{
			GetWindow<ItemManagementWindow>(true, "Item Management");
		}

		public void OnGUI()
		{
			DrawingItemWindow = true;

			if (m_ItemDatabase == null)
			{
				EditorGUILayout.HelpBox("No ItemDatabase was found in the Resources folder!", MessageType.Error);

				if (GUILayout.Button("Refresh"))
					InitializeWindow();

				if (m_ItemDatabase == null)
					return;
			}

			// Display the database path
			EditorGUILayout.LabelField(string.Format("Database path: '{0}'", AssetDatabase.GetAssetPath(m_ItemDatabase.targetObject)), STPEditorGUI.BoldMiniGreyLabel);

			// Display the shortcuts
			var shortcutStyle = new GUIStyle(STPEditorGUI.BoldMiniGreyLabel) { alignment = TextAnchor.MiddleRight };
			EditorGUI.LabelField(new Rect(position.width - 262f, 2f, 256f, 16f), "'Ctrl + D' to duplicate", shortcutStyle);
			EditorGUI.LabelField(new Rect(position.width - 262f, 17f, 256f, 16f), "'Delete' to delete", shortcutStyle);

			Vector2 buttonSize = new Vector2(192f, 32f);
			float topPadding = 32f;

			m_SelectedTab = GUI.Toolbar(new Rect(position.width * 0.5f - buttonSize.x, topPadding, buttonSize.x * 2, buttonSize.y), m_SelectedTab, new string[] { "Items", "Properties" });

			// Horizontal line.
			GUI.Box(new Rect(0f, topPadding + buttonSize.y * 1.25f, position.width, 1f), "");

			// Draw the item editors.
			m_ItemDatabase.Update();

			float innerWindowPadding = 8f;
			Rect innerWindowRect = new Rect(innerWindowPadding, topPadding + buttonSize.y * 1.25f + innerWindowPadding, position.width - innerWindowPadding * 2f, position.height - (topPadding + buttonSize.y * 1.25f + innerWindowPadding * 3f));

			// Inner window box.
			GUI.backgroundColor = Color.grey;
			GUI.Box(innerWindowRect, "");
			GUI.backgroundColor = Color.white;

			// GUI Style used for the list titles (e.g. Item list, Categories, etc.) 
			var listTitleBoxes = new GUIStyle(EditorStyles.toolbar);
			listTitleBoxes.fontSize = 12;
			listTitleBoxes.alignment = TextAnchor.MiddleCenter;

			if (m_SelectedTab == 0)
				DrawItemEditor(innerWindowRect, listTitleBoxes);
			else if (m_SelectedTab == 1)
				DrawPropertiesAndTagsEditor(innerWindowRect, listTitleBoxes);

			m_ItemDatabase.ApplyModifiedProperties();

			DrawingItemWindow = false;
		}

		private void OnEnable()
		{
			InitializeWindow();

			Undo.undoRedoPerformed += Repaint;
		}

        private void OnDestroy()
        {
			Undo.undoRedoPerformed -= Repaint;
		}

        private void InitializeWindow()
		{
			var database = Resources.LoadAll<ItemDatabase>("")[0];

			if (database)
			{
				m_ItemDatabase = new SerializedObject(database);

				m_CategoryList = new ReorderableList(m_ItemDatabase, m_ItemDatabase.FindProperty("m_Categories"), true, true, true, true);
				m_CategoryList.drawElementCallback += DrawCategory;
				m_CategoryList.drawHeaderCallback = (Rect rect) => { EditorGUI.LabelField(rect, ""); };
				m_CategoryList.onSelectCallback += On_SelectedCategory;
				m_CategoryList.onRemoveCallback = (ReorderableList list) => { m_CategoryList.serializedProperty.DeleteArrayElementAtIndex(m_CategoryList.index); };
			}
		}

		private void On_SelectedCategory(ReorderableList list)
		{
			m_ItemList = new ReorderableList(m_ItemDatabase, m_CategoryList.serializedProperty.GetArrayElementAtIndex(Mathf.Clamp(m_CategoryList.index, 0, m_CategoryList.count - 1)).FindPropertyRelative("m_Items"), true, true, true, true);
			m_ItemList.drawElementCallback += DrawItem;
			m_ItemList.drawHeaderCallback = (Rect rect) => { EditorGUI.LabelField(rect, ""); };
			m_ItemList.onRemoveCallback = OnItemRemoved;
		}

		private void OnItemRemoved(ReorderableList list)
		{
			m_ItemList.serializedProperty.DeleteArrayElementAtIndex(m_ItemList.index);
		}

		private void DrawItemEditor(Rect totalRect, GUIStyle listTitleBoxes)
		{
			// Inner window cross (partitioning in 4 smaller boxes)
			UnityEngine.GUI.Box(new Rect(totalRect.x, totalRect.y + totalRect.height * 0.5f, totalRect.width / 2f, 1f), "");
			UnityEngine.GUI.Box(new Rect(totalRect.x + totalRect.width * 0.5f, totalRect.y, 1f, totalRect.height), "");

			Vector2 labelSize = new Vector2(192f, 20f);

			// Draw the item list
			string selCategoryName = (m_CategoryList.count == 0 || m_CategoryList.index == -1) ? "None" : m_CategoryList.serializedProperty.GetArrayElementAtIndex(Mathf.Clamp(m_CategoryList.index, 0, m_CategoryList.count - 1)).FindPropertyRelative("m_Name").stringValue;

			string itemListName = string.Format("Item List ({0})", selCategoryName);

			UnityEngine.GUI.Box(new Rect(totalRect.x + totalRect.width * 0.25f - labelSize.x * 0.5f, totalRect.y, labelSize.x, labelSize.y), itemListName, listTitleBoxes);
			Rect itemListRect = new Rect(totalRect.x + 4f, totalRect.y + labelSize.y + 4f, totalRect.width * 0.5f - 10f, totalRect.height * 0.5f - labelSize.y - 5f);

			if (m_CategoryList.count != 0 && m_CategoryList.index != -1 && m_CategoryList.index < m_CategoryList.count)
				DrawList(m_ItemList, itemListRect, ref m_ItemsScrollPos);
			else
			{
				itemListRect.x += 8f;
				UnityEngine.GUI.Label(itemListRect, "Select a category...", new GUIStyle(STPEditorGUI.LargeTitleLabel) { alignment = TextAnchor.UpperLeft });
			}

			// Draw the categories
			UnityEngine.GUI.Box(new Rect(totalRect.x + totalRect.width * 0.25f - labelSize.x * 0.5f, totalRect.y + totalRect.height * 0.5f + 2f, labelSize.x, labelSize.y), "Category List", listTitleBoxes);

			int categoryCountBefore = m_CategoryList.count;
			DrawList(m_CategoryList, new Rect(totalRect.x + 4f, totalRect.y + totalRect.height * 0.5f + labelSize.y + 6f, totalRect.width * 0.5f - 10f, totalRect.height * 0.5f - labelSize.y - 7f), ref m_CategoriesScrollPos);

			if (categoryCountBefore != m_CategoryList.count && m_CategoryList.count > 0)
			{
				On_SelectedCategory(m_CategoryList);
				return;
			}

			// Inspector label
			UnityEngine.GUI.Box(new Rect(totalRect.x + totalRect.width * 0.75f - labelSize.x * 0.5f, totalRect.y, labelSize.x, labelSize.y), "Item Inspector", listTitleBoxes);

			// Draw the inspector
			bool itemIsSelected = m_CategoryList.count != 0 && m_ItemList != null && m_ItemList.count != 0 && m_ItemList.index != -1 && m_ItemList.index < m_ItemList.count;
			Rect inspectorRect = new Rect(totalRect.x + totalRect.width * 0.5f + 8f, totalRect.y + labelSize.y + 4f, totalRect.width * 0.5f - 13f, totalRect.height - labelSize.y - 9f);

			if (itemIsSelected)
				DrawItemInspector(inspectorRect);
			else
			{
				inspectorRect.x += 4f;
				inspectorRect.y += 4f;
				UnityEngine.GUI.Label(inspectorRect, "Select an item to inspect...", new GUIStyle(STPEditorGUI.LargeTitleLabel) { alignment = TextAnchor.UpperLeft });
			}
		}

		private void DrawList(ReorderableList list, Rect totalRect, ref Vector2 scrollPosition)
		{
			float scrollbarWidth = 16f;

			Rect onlySeenRect = new Rect(totalRect.x, totalRect.y, totalRect.width, totalRect.height);
			Rect allContentRect = new Rect(totalRect.x, totalRect.y, totalRect.width - scrollbarWidth, (list.headerHeight * 2f) + (list.count * (list.elementHeight + 1f)));

			scrollPosition = GUI.BeginScrollView(onlySeenRect, scrollPosition, allContentRect, false, true);

			// Draw the clear button.
			Vector2 buttonSize = new Vector2(56f, 17f);

			if (list.count > 0 && GUI.Button(new Rect(allContentRect.x + 2f, allContentRect.yMax, buttonSize.x, buttonSize.y), "Clear"))
			{
				if (EditorUtility.DisplayDialog("Warning!", "Are you sure you want the list to be cleared? (All elements will be deleted)", "Yes", "Cancel"))
					list.serializedProperty.ClearArray();
			}

			list.DoList(allContentRect);
			GUI.EndScrollView();
		}

		private void DrawCategory(Rect rect, int index, bool isActive, bool isFocused)
		{
			ItemManagementUtility.DrawListElementByName(m_CategoryList, index, rect, "m_Name", isFocused, this);
		}

		private void DrawItem(Rect rect, int index, bool isActive, bool isFocused)
		{
			if (m_ItemList.serializedProperty.arraySize > index)
				ItemManagementUtility.DrawListElementByName(m_ItemList, index, rect, "m_Name", isFocused, this);
		}

		private void DrawItemInspector(Rect viewportRect)
		{
			var item = m_ItemList.serializedProperty.GetArrayElementAtIndex(m_ItemList.index);
			item.isExpanded = true;

			GUI.Box(viewportRect, "");

			float indentation = 8f;
			Rect rect = new Rect(viewportRect.x + indentation, viewportRect.y + indentation, viewportRect.width - indentation * 2, viewportRect.height - indentation * 2);

			m_ItemInspectorScrollPos = GUI.BeginScrollView(viewportRect, m_ItemInspectorScrollPos, new Rect(rect.x, rect.y, rect.width - 16f, 28f + EditorGUI.GetPropertyHeight(item, true)));

			// Draw item name
			rect.xMin += indentation;
			rect.xMax -= 16f;
			rect.yMin += indentation;

			var itemNameStyle = new GUIStyle() { fontStyle = FontStyle.Bold, fontSize = 16 };
			itemNameStyle.normal.textColor = STPEditorGUI.LargeTitleLabel.normal.textColor;
			GUI.Label(rect, item.FindPropertyRelative("m_Name").stringValue, itemNameStyle);

			// Draw all item fields
			rect.yMax -= 16f;
			rect.y += 24f;

			var properties = item.Copy().GetChildren();

			rect.y += EditorGUIUtility.standardVerticalSpacing;

			foreach (var prop in properties)
			{
				rect.height = EditorGUI.GetPropertyHeight(prop, true);
				EditorGUI.PropertyField(rect, prop, true);
				rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
				prop.isExpanded = true;
			}

			GUI.EndScrollView();
		}

		private void DrawPropertiesAndTagsEditor(Rect totalRect, GUIStyle listTitleBoxes)
		{
			SerializedProperty propertiesProp = m_ItemDatabase.FindProperty("m_ItemProperties");
			SerializedProperty tagsProp = m_ItemDatabase.FindProperty("m_ItemTags");

			Vector2 labelSize = new Vector2(128f, 20f);

			// Inner window cross (partitioning in 4 smaller boxes)
			UnityEngine.GUI.Box(new Rect(totalRect.x + totalRect.width * 0.5f, totalRect.y, 1f, totalRect.height), "");

			// Properties label
			UnityEngine.GUI.Box(new Rect(totalRect.x + totalRect.width * 0.25f - labelSize.x * 0.5f, totalRect.y, labelSize.x, labelSize.y), "Properties", listTitleBoxes);

			// Properties list
			Rect propertiesRect = new Rect(totalRect.x + 4f, totalRect.y + labelSize.y + 4f, totalRect.width * 0.5f - 10f, EditorGUI.GetPropertyHeight(propertiesProp));

			EditorGUI.PropertyField(propertiesRect, propertiesProp);

			// Tags label
			UnityEngine.GUI.Box(new Rect(totalRect.x + totalRect.width * 0.75f - labelSize.x * 0.5f, totalRect.y, labelSize.x, labelSize.y), "Tags", listTitleBoxes);

			// Tags list
			Rect tagsRect = new Rect(totalRect.x + totalRect.width * 0.5f + 8f, totalRect.y + labelSize.y + 4f, totalRect.width * 0.5f - 13f, EditorGUI.GetPropertyHeight(tagsProp));

			EditorGUI.PropertyField(tagsRect, tagsProp);
		}
	}

	public static class ItemManagementUtility
	{
		public static void DoListElementBehaviours(ReorderableList list, int index, bool isFocused, EditorWindow window = null)
		{
			var current = Event.current;

			if (current.type == EventType.KeyDown)
			{
				if (list.index == index && isFocused)
				{
					if (current.keyCode == KeyCode.Delete)
					{
						int newIndex = 0;
						if (list.count == 1)
							newIndex = -1;
						else if (index == list.count - 1)
							newIndex = index - 1;
						else if (index > 0)
							newIndex = index - 1;

						list.serializedProperty.DeleteArrayElementAtIndex(index);

						if (newIndex != -1)
						{
							list.index = newIndex;
							if (list.onSelectCallback != null)
								list.onSelectCallback(list);
						}

						Event.current.Use();
						if (window)
							window.Repaint();
					}
					else if (current.control && current.keyCode == KeyCode.D)
					{
						list.serializedProperty.InsertArrayElementAtIndex(list.index);
						list.index++;
						if (list.onSelectCallback != null)
							list.onSelectCallback(list);

						Event.current.Use();
						if (window)
							window.Repaint();
					}
				}
			}
		}

		public static void DrawListElementByName(ReorderableList list, int index, Rect rect, string nameProperty, bool isFocused, EditorWindow window)
		{
			if (list.serializedProperty.arraySize == index)
				return;

			rect.y += 2;
			var element = list.serializedProperty.GetArrayElementAtIndex(index);
			var name = element.FindPropertyRelative(nameProperty);

			name.stringValue = EditorGUI.TextField(new Rect(rect.x, rect.y, 256f, 16f), name.stringValue);

			DoListElementBehaviours(list, index, isFocused, window);
		}
	}
}