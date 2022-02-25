using System;
using UnityEditor;
using UnityEngine;

namespace SurvivalTemplatePro.WieldableSystem
{
    [CustomPropertyDrawer(typeof(FPWieldableMotionState))]
	public class FPWieldableMotionStateDrawer : PropertyDrawer
	{
		public static event Action<FPWieldableMotionStateDrawer> EnabledVisualization;

		private bool m_Enabled;
		private bool m_Visualize;
		private float m_VisualizationSpeed = 3f;

		private FPWieldableMotion m_MotionComponent;
		private FPWieldableMotionState m_MotionState;


		public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
		{
			if (property.isExpanded)
				GUI.Box(new Rect(position.x, position.y + 16f, position.width, position.height - 16f), "");

			if (!m_Enabled)
				OnEnable(property);

			EditorGUI.PropertyField(position, property, label, true);

			if (property.isExpanded)
			{
				if (Application.isPlaying)
				{
					// Visualize button
					position.x += 8f;
					position.y = position.yMax - 38f;
					position.height = 16f;
					position.width -= 16f;

					STPEditorGUI.Separator(new Rect(position.x, position.y - 4f, position.width, 1f));

					GUI.color = m_Visualize ? Color.grey : Color.white;

					if (GUI.Button(position, "Visualize"))
					{
						if (m_MotionComponent != null && m_MotionComponent.enabled && m_MotionComponent.gameObject.activeInHierarchy)
						{
							m_Visualize = !m_Visualize;

							if (m_Visualize)
								EnabledVisualization(this);

							m_MotionComponent.VisualizeState(m_Visualize ? m_MotionState : null, m_VisualizationSpeed);
						}
					}

					GUI.color = Color.white;

					// Visualize speed
					position.y = position.yMax + 2f;
					m_VisualizationSpeed = EditorGUI.Slider(position, "Speed", m_VisualizationSpeed, 0f, 20f);
				}
					
				if (GUI.changed && m_Visualize && m_MotionComponent != null)
					m_MotionComponent.VisualizeState(m_MotionState, m_VisualizationSpeed);
			}
		}
		
		public override float GetPropertyHeight (SerializedProperty property, GUIContent label)
		{
			return EditorGUI.GetPropertyHeight(property) + ((Application.isPlaying && property.isExpanded) ? 44f : 0f);
		}

		private void OnEnable(SerializedProperty property)
		{
			EnabledVisualization += OnEnabledVisualization;

			Selection.selectionChanged += OnSelectionChanged;

			m_MotionState = property.GetValue<FPWieldableMotionState>();

			//if (m_MotionState == null)
		//		m_MotionState = ((FPWieldableMotion.StateSettings)fieldInfo.GetValue(property.serializedObject.targetObject)).Motion;

			var comp = property.serializedObject.targetObject as Component;
			m_MotionComponent = comp.GetComponent<FPWieldableMotion>();

			m_Enabled = true;
		}

		private void OnEnabledVisualization(FPWieldableMotionStateDrawer drawer)
		{
			m_Visualize = (drawer == this);
		}

		private void OnSelectionChanged()
		{
			if (m_MotionComponent == null || Selection.activeGameObject == null || Selection.activeGameObject != m_MotionComponent.gameObject)
				OnDestroy();
		}

		private void OnDestroy()
		{
			EnabledVisualization -= OnEnabledVisualization;
			Selection.selectionChanged -= OnSelectionChanged;

			if (m_MotionComponent != null)
				m_MotionComponent.VisualizeState(null);
		}
    }
}