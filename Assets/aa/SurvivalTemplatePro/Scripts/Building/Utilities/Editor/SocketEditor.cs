using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace SurvivalTemplatePro.BuildingSystem
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Socket))]
    public class SocketEditor : Editor
    {
        public enum Axis { X, Z }

        private static bool m_EditOffset;

        private static Axis m_MirrorAxis;
        private static bool m_InvertRotationToggle;
        private static bool m_AlignRotationToggle;

        private static GUIStyle m_HandlesLabelStyle;

		private static Material m_PreviewMat;

		private Socket m_Socket;
        private Buildable m_Buildable;

		private SerializedProperty m_Radius;
		private int m_SelectedPieceIdx;

		private ReorderableList m_PieceOffsets;

		private Socket.PieceOffset m_SelectedPieceOffset;

        private GameObject m_CurrentPreview;


        public override void OnInspectorGUI()
		{
            if(m_HandlesLabelStyle == null)
                m_HandlesLabelStyle = new GUIStyle(EditorStyles.whiteBoldLabel) { alignment = TextAnchor.MiddleCenter };

            serializedObject.Update();

			EditorGUILayout.Space();
			EditorGUILayout.PropertyField(m_Radius);
			STPEditorGUI.Separator();
			EditorGUILayout.Space();

			GUI.color = m_EditOffset ? Color.grey : Color.white;

			if (GUILayout.Button("Enable Preview"))
			{
				m_EditOffset = !m_EditOffset;

                if (m_EditOffset)
                {
                    m_PieceOffsets.index = 0;
                    m_PieceOffsets.onSelectCallback.Invoke(m_PieceOffsets);
                }
                else
                {
                    Tools.current = Tool.Move;

                    if(m_CurrentPreview != null)
                        DestroyImmediate(m_CurrentPreview);
                }
            }

			GUI.color = Color.white;

			if(!serializedObject.isEditingMultipleObjects)
				m_PieceOffsets.DoLayoutList();

            STPEditorGUI.Separator();

			EditorGUILayout.Space();

            EditorGUILayout.LabelField("Tools", EditorStyles.boldLabel);

            if (GUILayout.Button("Create Mirror Socket"))
            {
                Vector3 mirrorAxisVector = Vector3.right;

                if(m_MirrorAxis == Axis.Z)
                    mirrorAxisVector = Vector3.forward;

                MirrorSocket(mirrorAxisVector);
            }

            Color prevGUIColor = GUI.color;
            GUI.color = new Color(prevGUIColor.r, prevGUIColor.g, prevGUIColor.b, 0.6f);

            Rect rect = EditorGUILayout.GetControlRect();

            Rect popupRect = new Rect(rect.x + rect.width * 0.75f, rect.y, rect.width * 0.25f, rect.height);
            Rect labelRect = new Rect(rect.xMax - popupRect.width - 72, rect.y, 72, rect.height);

            m_MirrorAxis = (Axis)EditorGUI.EnumPopup(popupRect, m_MirrorAxis);

            EditorGUI.LabelField(labelRect, "Mirror Axis: ");

            GUI.color = prevGUIColor;

            EditorGUILayout.Space();

            if(GUILayout.Button("Create Perpendicular Socket (90 Degrees)"))
                CreatePerpendicularSocket();

            GUI.color = new Color(prevGUIColor.r, prevGUIColor.g, prevGUIColor.b, 0.6f);

            rect = EditorGUILayout.GetControlRect();

            popupRect = new Rect(rect.x + rect.width * 0.75f, rect.y, rect.width * 0.25f, rect.height);
            labelRect = new Rect(rect.xMax - popupRect.width - 44, rect.y, 44, rect.height);

            m_InvertRotationToggle = EditorGUI.Toggle(popupRect, m_InvertRotationToggle);

            EditorGUI.LabelField(labelRect, "Invert: ");

            rect = EditorGUILayout.GetControlRect();

            popupRect = new Rect(rect.x + rect.width * 0.75f, rect.y, rect.width * 0.25f, rect.height);
            labelRect = new Rect(rect.xMax - popupRect.width - 44, rect.y, 44, rect.height);

            m_AlignRotationToggle = EditorGUI.Toggle(popupRect, m_AlignRotationToggle);

            EditorGUI.LabelField(labelRect, "Align: ");

            GUI.color = prevGUIColor;

            serializedObject.ApplyModifiedProperties();
		}

        private void MirrorSocket(Vector3 mirrorAxis)
        {
            Vector3 mirrorScaler = new Vector3(-Mathf.Clamp01(Mathf.Abs(mirrorAxis.x)), -Mathf.Clamp01(Mathf.Abs(mirrorAxis.y)), -Mathf.Clamp01(Mathf.Abs(mirrorAxis.z)));

            if(mirrorScaler.x == 0f)
                mirrorScaler.x = 1f;

            if(mirrorScaler.y == 0f)
                mirrorScaler.y = 1f;

            if(mirrorScaler.z == 0f)
                mirrorScaler.z = 1f;

            Vector3 originalPosition = m_Socket.transform.localPosition;
            Vector3 mirrorPosition = Vector3.Scale(originalPosition, mirrorScaler);

            GameObject mirrorSocket = Instantiate(m_Socket.gameObject, m_Socket.transform.parent);
            mirrorSocket.transform.localPosition = mirrorPosition;

            mirrorSocket.name = "Socket";

            var offsets = mirrorSocket.GetComponent<Socket>().PieceOffsets;

            foreach(var offset in offsets)
                offset.PositionOffset = Vector3.Scale(offset.PositionOffset, mirrorScaler);

            Undo.RegisterCreatedObjectUndo(mirrorSocket, "Create Mirror Socket");

            Selection.activeGameObject = mirrorSocket;
        }

        private void CreatePerpendicularSocket()
        {
            Quaternion rotator = Quaternion.Euler(0f, 90f * (m_InvertRotationToggle ? -1 : 1), 0f);

            Vector3 rotatedSocketPosition = RoundVector3(rotator * m_Socket.transform.localPosition);

            GameObject mirrorSocket = Instantiate(m_Socket.gameObject, m_Socket.transform.parent);
            Undo.RegisterCreatedObjectUndo(mirrorSocket, "Crate Perpendicular Socket");

            mirrorSocket.transform.localPosition = rotatedSocketPosition;

            mirrorSocket.name = "Socket";

            var offsets = mirrorSocket.GetComponent<Socket>().PieceOffsets;

            foreach(var offset in offsets)
            {
                offset.PositionOffset = RoundVector3(rotator * offset.PositionOffset);

                if(m_AlignRotationToggle)
                    offset.RotationOffsetEuler = RoundVector3((rotator * offset.RotationOffset).eulerAngles);
            }

            Selection.activeGameObject = mirrorSocket;
        }

		private void OnEnable()
		{
			// Create the material for the preview if it's null.
			if(m_PreviewMat == null)
			{
				m_PreviewMat = new Material(Shader.Find("Diffuse"));
				m_PreviewMat.color = new Color(0.5f, 0.6f, 0.5f, 1f);
			}

			m_Socket = target as Socket;

			m_Radius = serializedObject.FindProperty("m_Radius");

			// Initialize the piece list.
			m_PieceOffsets = new ReorderableList(serializedObject, serializedObject.FindProperty("m_PieceOffsets"));

			m_PieceOffsets.drawHeaderCallback = (Rect rect)=> GUI.Label(rect, "Attached Buildables");
			m_PieceOffsets.drawElementCallback = DrawPieceElement;
			m_PieceOffsets.onSelectCallback += OnPieceSelect;
            
            // Buildable
            m_Buildable = m_Socket.GetComponentInParent<Buildable>();

            if (m_EditOffset)
            {
                m_PieceOffsets.index = serializedObject.FindProperty("m_SelectedBuildableOffset").intValue;
                m_PieceOffsets.onSelectCallback.Invoke(m_PieceOffsets);
            }

            // GUI styles
            if (m_HandlesLabelStyle == null)
                m_HandlesLabelStyle = new GUIStyle(EditorStyles.whiteBoldLabel) { alignment = TextAnchor.MiddleCenter };
        }

        private void OnDestroy()
        {
            Tools.hidden = false;

            if(m_CurrentPreview != null)
                DestroyImmediate(m_CurrentPreview);
        }

		private void DrawPieceElement(Rect rect, int index, bool isActive, bool isFocused)
		{
			EditorGUI.BeginChangeCheck();

			EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, 16f), m_PieceOffsets.serializedProperty.GetArrayElementAtIndex(index).FindPropertyRelative("m_Buildable"));

			if(EditorGUI.EndChangeCheck())
				TrySelectPiece(index);
		}

        private void OnPieceSelect(ReorderableList list) => TrySelectPiece(list.index);

		private void TrySelectPiece(int index)
		{
			if(m_PieceOffsets.count > 0)
			{
                if(m_CurrentPreview != null)
                    DestroyImmediate(m_CurrentPreview);

                index = Mathf.Clamp(index, 0, m_PieceOffsets.count - 1);

                if(m_Socket.PieceOffsets[index].Buildable != null)
                {
                    m_SelectedPieceOffset = (target as Socket).PieceOffsets[index];

                    m_PieceOffsets.index = index;
                    m_SelectedPieceIdx = index;

                    serializedObject.FindProperty("m_SelectedBuildableOffset").intValue = index;

                    CreatePreview(m_SelectedPieceOffset.Buildable);
                }
            }
		}

		private void OnSceneGUI()
		{
            if (CannotDisplayMesh() || m_Buildable == null)
            {
                SceneView.RepaintAll();
                return;
            }

            // Get the scene camera and it's pixel rect.
            var sceneCamera = SceneView.GetAllSceneCameras()[0];
			Rect pixelRect = sceneCamera.pixelRect;

			if (HasValidPiece())
			{
				// Draw the piece handle.
				Vector3 pieceWorldPos = m_Socket.transform.position + m_Buildable.transform.TransformVector(m_SelectedPieceOffset.PositionOffset);


				// HACK
				try
				{
                    RefreshPreviewPosition();

                    Color prevHandlesColor = Handles.color;
                    Handles.color = Color.white;

                    Handles.Label(m_CurrentPreview.transform.position, m_CurrentPreview.name, m_HandlesLabelStyle);

                    Handles.color = prevHandlesColor;

                } catch {}
            
                SceneView.RepaintAll();
			}

			// Draw the inspector for the piece offset for the selected socket, so you can modify the position and rotation precisely.
			DoPieceOffsetInspectorWindow(pixelRect);
		}

        private void RefreshPreviewPosition()
        {
            Vector3 position = m_Socket.transform.position + m_Buildable.transform.TransformVector(m_Socket.PieceOffsets[m_SelectedPieceIdx].PositionOffset);
            Quaternion rotation = m_Buildable.transform.rotation * m_Socket.PieceOffsets[m_SelectedPieceIdx].RotationOffset;

            m_CurrentPreview.transform.position = position;
            m_CurrentPreview.transform.rotation = rotation;
        }

        private void CreatePreview(Buildable targetBuildable)
        {
            GameObject preview = Instantiate(targetBuildable.gameObject, m_Buildable.transform);
            preview.name = targetBuildable.name + " (Preview)";
            var components = preview.GetComponentsInChildren<Component>();

            foreach (var rootComponent in components)
            {
                if (rootComponent.GetType() != typeof(Transform) && rootComponent.GetType() != typeof(MeshFilter) && rootComponent.GetType() != typeof(MeshRenderer))
                    DestroyImmediate(rootComponent);
            }

            m_CurrentPreview = preview;
        }

		private void DoPieceOffsetTools()
		{
			Vector3 pieceWorldPos = m_Socket.transform.position + m_Socket.transform.TransformVector(m_SelectedPieceOffset.PositionOffset);

			EditorGUI.BeginChangeCheck();
			var handlePos = Handles.PositionHandle(pieceWorldPos, m_Socket.transform.rotation * m_SelectedPieceOffset.RotationOffset);

			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(target, "Socket");

				handlePos = RoundVector3(m_Socket.transform.InverseTransformPoint(handlePos));
				m_SelectedPieceOffset.PositionOffset = handlePos;
			}
		}

		private void DoPieceOffsetInspectorWindow(Rect pixelRect)
		{
			Color color = Color.white;
			GUI.backgroundColor = color;

			var windowRect = new Rect(16f, 32f, 256f, 112f);
			Rect totalRect = new Rect(windowRect.x, windowRect.y - 16f, windowRect.width, windowRect.height);

			GUI.backgroundColor = Color.white;
			GUI.Window(1, windowRect, DrawPieceOffsetInspector, "Position & Rotation");

			Event e = Event.current;

			if(totalRect.Contains(e.mousePosition))
			{
				HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

				if(e.type != EventType.Layout && e.type != EventType.Repaint)
					e.Use();
			}
		}

		private void DrawPieceOffsetInspector(int windowID)
		{
			if(!HasValidPiece())
			{
				EditorGUI.HelpBox(new Rect(0f, 32f, 512f, 32f), "No valid piece selected!", MessageType.Warning);
				return;
			}
				
			var pieceOffset = m_SelectedPieceOffset;

			EditorGUI.BeginChangeCheck();

			// Position field.
			var positionOffset = EditorGUI.Vector3Field(new Rect(6f, 32f, 240f, 16f), "Position", pieceOffset.PositionOffset);

			// Rotation field.
			var rotationOffset = EditorGUI.Vector3Field(new Rect(6f, 64f, 240f, 16f), "Rotation", pieceOffset.RotationOffsetEuler);

			if(EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(target, "Socket");

				positionOffset = RoundVector3(positionOffset);
				rotationOffset = RoundVector3(rotationOffset);

				pieceOffset.PositionOffset = positionOffset;
				pieceOffset.RotationOffsetEuler = rotationOffset;
			}
		}

        private bool HasValidPiece() => m_PieceOffsets.count != 0 && m_SelectedPieceIdx >= 0 && m_SelectedPieceOffset != null && m_SelectedPieceOffset.Buildable != null;
        private bool CannotDisplayMesh() => (!m_EditOffset || Selection.activeGameObject == null || Selection.activeGameObject != m_Socket.gameObject);

        private Vector3 RoundVector3(Vector3 source, int digits = 3)
		{
			source.x = (float)System.Math.Round(source.x, digits);
			if(Mathf.Approximately(source.x, 0f))
				source.x = 0f;

			source.y = (float)System.Math.Round(source.y, digits);
			if(Mathf.Approximately(source.y, 0f))
				source.y = 0f;

			source.z = (float)System.Math.Round(source.z, digits);
			if(Mathf.Approximately(source.y, 0f))
				source.y = 0f;

			return source;
		}
	}
}
