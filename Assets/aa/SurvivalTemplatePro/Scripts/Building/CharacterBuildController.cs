using UnityEngine;
using UnityEngine.Events;

namespace SurvivalTemplatePro.BuildingSystem
{
    /// <summary>
    /// Handles placing and building objects.
    /// </summary>
    public class CharacterBuildController : CharacterBehaviour, IBuildingController
    {
        public Placeable CurrentPlaceable => m_Placeable;
        public BuildableType BuildingMode => m_Buildable != null ? m_Buildable.BuildableType : BuildableType.Free;

        public event UnityAction onBuildingStart;
        public event UnityAction onBuildingEnd;
        public event UnityAction onObjectPlaced;
        public event UnityAction<Placeable> onObjectChanged;

        [BHeader("Build Settings")]

        [SerializeField]
        [Tooltip("Should the placeables follow the rotation of the character?")]
        private bool m_FollowCharacterRotation;

        [SerializeField, Range(0f, 70f)]
        [Tooltip("Max angle for detecting nearby sockets.")]
        private float m_ViewAngleThreshold = 35f;

        [SerializeField, Range(0f, 360)]
        [Tooltip("How fast can the placeables be rotated.")]
        private float m_RotationSpeed = 45f;

        [SerializeField, Range(0f, 10f)]
        [Tooltip("Max building range.")]
        private float m_BuildRange = 4f;

        [BHeader("Build Filters")]

        [SerializeField]
        [Tooltip("Tells the controller on what layers the 'buildables' are.")]
        private LayerMask m_BuildableMask;

        [SerializeField]
        [Tooltip("Tells the controller on what layers can 'placeables' be placed")]
        private LayerMask m_FreePlacementMask;

        [SerializeField]
        [Tooltip("Tells the controller what layers to to check when checking for collisions.")]
        private LayerMask m_OverlapCheckMask;

        [BHeader("Build Audio")]

        [SerializeField]
        [Tooltip("Sound to play when placing an object")]
        private StandardSound m_PlaceSound;

        [SerializeField]
        [Tooltip("Sound to play when the controller tries to place an object but detects a collision.")]
        private StandardSound m_InvalidPlaceSound;

        [Space]

        [SerializeField]
        private UnityEvent m_OnPlaceObject;

        [SerializeField]
        private UnityEvent m_OnBuildingStart;

        [SerializeField]
        private UnityEvent m_OnBuildingEnd;

        private Placeable m_Placeable;
        private Placeable m_PlaceablePrefab;
        private Buildable m_Buildable;

        private float m_RotationOffset;

        private bool m_PlacementAllowed;
        private float m_NextTimeCanPlace;
        private Socket m_Socket;
        private Collider m_Surface;

        private ILookHandler m_LookHandler;


        public static void OccupySurroundingSockets(Buildable buildable)
        {
            Collider[] overlappingStuff = Physics.OverlapBox(buildable.Bounds.center, buildable.Bounds.extents, buildable.transform.rotation, PlaceableDatabase.GetFreePlacementMask(), QueryTriggerInteraction.Collide);

            for (int i = 0; i < overlappingStuff.Length; i++)
            {
                Socket socket = overlappingStuff[i].GetComponent<Socket>();

                if (socket != null && socket.SupportsBuildable(buildable) && !socket.HasSpaceForBuildable(PlaceableDatabase.GetFreePlacementMask(), buildable))
                    socket.OccupySpaces(buildable);
            }
        }

        public override void OnInitialized()
        {
            GetModule(out m_LookHandler);
            m_LookHandler.onPostViewUpdate += UpdateObjectPlacement;
        }

        public void StartBuilding(Placeable placeable)
        {
            SetPlaceable(placeable);

            onBuildingStart?.Invoke();
            m_OnBuildingStart?.Invoke();
        }

        public void EndBuilding()
        {
            SetPlaceable(null);

            onBuildingEnd?.Invoke();
            m_OnBuildingEnd?.Invoke();
        }

        public void SetPlaceable(Placeable objectToPlace)
        {
            if (objectToPlace != m_PlaceablePrefab)
            {
                ResetInfo();

                if (objectToPlace != null)
                    CreatePlaceable(objectToPlace);

                onObjectChanged?.Invoke(m_Placeable);
            }
        }

        public void SelectNextPlaceable(bool next)
        {
            if (BuildingMode == BuildableType.SocketBased)
            {
                Placeable[] customBuildingParts = PlaceableDatabase.GetCategory(PlaceableDatabase.k_CustomBuildingCategoryName).Placeables;

                int customBuildableIdx = PlaceableDatabase.GetIndexInCategory(m_Placeable.PlaceableID, PlaceableDatabase.k_CustomBuildingCategoryName);

                if (customBuildableIdx == -1)
                    customBuildableIdx = 0;

                customBuildableIdx = (int)Mathf.Repeat(customBuildableIdx + (next ? 1 : -1), customBuildingParts.Length);
                SetPlaceable(customBuildingParts[customBuildableIdx]);
            }
        }

        public void PlaceObject()
        {
            if (m_Placeable == null || Time.time < m_NextTimeCanPlace)
                return;

            if (!m_PlacementAllowed)
            {
                // Play invalid place sound
                Character.AudioPlayer.PlaySound(m_InvalidPlaceSound);

                m_NextTimeCanPlace = Time.time + 0.5f;

                return;
            }

            PlaceObject(m_Placeable);

            m_OnPlaceObject?.Invoke();
            onObjectPlaced?.Invoke();

            if (m_Buildable != null && m_Buildable.BuildableType == BuildableType.SocketBased)
                CreatePlaceable(m_PlaceablePrefab);
            else
                EndBuilding();

            // Play place sound
            Character.AudioPlayer.PlaySound(m_PlaceSound);
        }

        public void RotateObject(float rotationDelta)
        {
            if (m_Placeable != null)
                m_RotationOffset += m_RotationSpeed * rotationDelta;
        }

        private void UpdateObjectPlacement()
        {
            if (m_Placeable == null)
                return;

            bool isObstructed = CheckForCollisions(m_Placeable);
            m_PlacementAllowed = !isObstructed && m_Placeable.CanPlace();

            if (m_Buildable != null)
            {
                m_Socket = FindSocket(m_Buildable);
                m_PlacementAllowed &= (!m_Buildable.RequiresSockets || m_Socket != null);

                if (m_Buildable.MaterialChanger != null)
                    m_Buildable.MaterialChanger.SetOverrideMaterial(m_PlacementAllowed ? PlaceableDatabase.GetPlaceAllowedMaterial() : PlaceableDatabase.GetPlaceDeniedMaterial());
            }

            if (m_Buildable != null && m_Socket != null)
                SnapToSocket(m_Buildable, m_Socket);
            else
                DoFreePlacement(m_Placeable, m_PlaceablePrefab.transform.rotation, out m_Surface);
        }

        private void DoFreePlacement(Placeable placeable, Quaternion baseRotation, out Collider surface)
        {
            surface = null;

            Vector3 position;
            Quaternion rotation;

            Camera cam = Camera.main;
            Ray ray = cam.ViewportPointToRay(Vector3.one * 0.5f);

            if (Physics.Raycast(ray, out RaycastHit hitInfo, m_BuildRange, m_FreePlacementMask, QueryTriggerInteraction.Ignore))
            {
                position = hitInfo.point;
                surface = hitInfo.collider;
            }
            else
            {
                Vector3 currentPos = transform.position + transform.forward * m_BuildRange;
                Vector3 startPos = placeable.transform.position + new Vector3(0, 0.25f, 0);

                if (Physics.Raycast(startPos, Vector3.down, out RaycastHit hit, 1f, m_FreePlacementMask, QueryTriggerInteraction.Ignore))
                {
                    currentPos.y = hit.point.y;
                    surface = hit.collider;
                }

                position = currentPos;
            }

            if (m_FollowCharacterRotation)
                rotation = transform.rotation * Quaternion.Euler(0f, m_RotationOffset, 0f) * baseRotation;
            else
                rotation = Quaternion.Euler(0f, m_RotationOffset, 0f) * baseRotation;

            placeable.transform.position = position;
            placeable.transform.rotation = rotation;
        }

        private bool CheckForCollisions(Placeable placeable)
        {
            bool canPlace = placeable.PlaceOnBuildables || (m_Surface == null || m_Surface.GetComponent<Buildable>() == null);

            if (!canPlace)
                return false;

            Bounds bounds = placeable.Bounds;

            Collider[] overlappingColliders = Physics.OverlapBox(bounds.center, bounds.extents, placeable.transform.rotation, m_OverlapCheckMask, QueryTriggerInteraction.Ignore);

            for (int i = 0; i < overlappingColliders.Length; i++)
            {
                if (!placeable.HasCollider(overlappingColliders[i]))
                {
                    TerrainCollider terrainCollider = overlappingColliders[i] as TerrainCollider;

                    if (terrainCollider == null)
                    {
                        Buildable buildable = overlappingColliders[i].GetComponent<Buildable>();
                        bool isSameStructure = buildable && m_Socket != null && buildable.ParentStructure == m_Socket.Buildable.ParentStructure;

                        if (!isSameStructure)
                            return true;
                    }
                }
            }

            return false;
        }

        private Socket FindSocket(Buildable buildable)
        {
            Collider[] proximityColliders = Physics.OverlapSphere(transform.position, m_BuildRange, m_BuildableMask, QueryTriggerInteraction.Ignore);

            float smallestAngleToSocket = Mathf.Infinity;
            Socket socket = null;

            // Loop through all buildables in proximity and calculate which socket is the closest in terms of distance & angle
            for (int i = 0; i < proximityColliders.Length; i++)
            {
                Buildable proximityBuildable = proximityColliders[i].GetComponent<Buildable>();

                // Skip the checks on this object if there's no buildable component or if the buildable doesn't have sockets
                if (proximityBuildable == null || proximityBuildable.Sockets.Length == 0)
                    continue;

                // Loop through all sockets, compare it to the last one that was checked, see if we find a better one to snap to
                for (int j = 0; j < proximityBuildable.Sockets.Length; j++)
                {
                    Ray viewRay = new Ray(Character.View.position, Character.View.forward);
                    CheckSocket(viewRay, buildable, proximityBuildable.Sockets[j], ref socket, ref smallestAngleToSocket);
                }
            }

            return socket;
        }

        private void SnapToSocket(Buildable buildable, Socket socket)
        {
            Socket.PieceOffset buildableOffset = socket.GetBuildableOffset(buildable.PlaceableName);

            buildable.transform.position = socket.transform.position + socket.Buildable.transform.TransformVector(buildableOffset.PositionOffset);
            buildable.transform.rotation = socket.Buildable.transform.rotation * buildableOffset.RotationOffset;
        }

        private void CheckSocket(Ray viewRay, Buildable buildable, Socket socket, ref Socket bestMatchSocket, ref float bestMatchAngle)
        {
            bool supportsBuildable = socket.SupportsBuildable(buildable);
            bool isInRange = (socket.transform.position - transform.position).sqrMagnitude < m_BuildRange * m_BuildRange;

            if (supportsBuildable && isInRange)
            {
                float angleToSocket = Vector3.Angle(viewRay.direction, socket.transform.position - viewRay.origin);

                if (angleToSocket < bestMatchAngle && angleToSocket < m_ViewAngleThreshold)
                {
                    bestMatchAngle = angleToSocket;
                    bestMatchSocket = socket;
                }
            }
        }

        private void PlaceObject(Placeable placeable)
        {
            Buildable buildable = placeable as Buildable;

            if (buildable == null)
            {
                GameObject placeableInstance = Instantiate(PlaceableDatabase.GetPlaceableById(placeable.PlaceableID).gameObject, placeable.transform.position, placeable.transform.rotation);

                BuildablePreview preview = placeableInstance.AddComponent<BuildablePreview>();

                preview.EnablePreview();
                preview.UpdatePreview();
            }
            else
            {
                if (m_Socket != null)
                {
                    StructureManager structure = m_Socket.Buildable.ParentStructure;

                    Buildable partPrefab = PlaceableDatabase.GetPlaceableById(buildable.PlaceableID) as Buildable;
                    Buildable part = Instantiate(partPrefab, buildable.transform.position, buildable.transform.rotation);

                    part.transform.SetParent(structure.transform);
                    part.ParentStructure = structure;

                    structure.AddPart(part, true, true);

                    if (structure.Buildables.Count > 1)
                        OccupySurroundingSockets(buildable);

                    BuildablePreview preview = structure.gameObject.GetComponent<BuildablePreview>();
                    preview.UpdatePreview();
                }
                else
                {
                    bool isChildOfStructure = buildable.BuildableType == BuildableType.SocketBased;

                    if (isChildOfStructure)
                    {
                        StructureManager structure = Instantiate(PlaceableDatabase.GetStructurePrefab().gameObject, placeable.transform.position, placeable.transform.rotation).GetComponent<StructureManager>();

                        Buildable partPrefab = PlaceableDatabase.GetPlaceableById(buildable.PlaceableID) as Buildable;
                        Buildable part = Instantiate(partPrefab, buildable.transform.position, buildable.transform.rotation);

                        part.transform.SetParent(structure.transform);
                        part.ParentStructure = structure;

                        structure.AddPart(part, true, true);

                        if (structure.Buildables.Count > 1)
                            OccupySurroundingSockets(buildable);

                        BuildablePreview preview = structure.gameObject.AddComponent<BuildablePreview>();

                        preview.EnablePreview();
                        preview.UpdatePreview();
                    }
                    else
                    {
                        GameObject simpleBuildable = Instantiate(PlaceableDatabase.GetPlaceableById(placeable.PlaceableID).gameObject, placeable.transform.position, placeable.transform.rotation);

                        BuildablePreview preview = simpleBuildable.AddComponent<BuildablePreview>();

                        preview.EnablePreview();
                        preview.UpdatePreview();
                    }
                }
            }

            Destroy(placeable.gameObject);
        }

        private void CreatePlaceable(Placeable placeablePrefab)
        {
            m_Placeable = Instantiate(placeablePrefab);
            m_Placeable.transform.position = Vector3.one * 10000f;

            m_PlaceablePrefab = placeablePrefab;
            m_Buildable = m_Placeable as Buildable;

            if (m_Buildable != null)
                m_Buildable.SetActivationState(BuildableActivationState.Disabled);
        }

        private void ResetInfo() 
        {
            if (m_Placeable == null)
                return;

            Destroy(m_Placeable.gameObject);

            m_Placeable = null;
            m_PlaceablePrefab = null;
            m_Buildable = null;
        }
    }
}