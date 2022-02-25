using UnityEditor;
using UnityEngine;

namespace SurvivalTemplatePro.UISystem
{
    [CustomEditor(typeof(InventoryPanelsManagerUI))]
    public class InventoryPanelsManagerUIEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space();

            STPEditorGUI.Separator();

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Show All the Inventory Panels"))
            {
                ShowAllChildPanels(true);
                return;
            }

            if (GUILayout.Button("Hide All the Inventory Panels"))
                ShowAllChildPanels(false);

            EditorGUILayout.EndHorizontal();
        }

        private void ShowAllChildPanels(bool show) 
        {
            var obj = (target as Component).gameObject;

            if (obj == null)
                return;

            var panels = obj.GetComponentsInChildren<PanelUI>(true);

            foreach (var panel in panels)
            {
                panel.CanvasGroup.alpha = show ? 1f : 0f;
                panel.CanvasGroup.blocksRaycasts = show;
            }
        }
    }
}