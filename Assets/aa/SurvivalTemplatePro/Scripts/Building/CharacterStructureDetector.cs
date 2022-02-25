using UnityEngine;
using UnityEngine.Events;

namespace SurvivalTemplatePro.BuildingSystem
{
    [RequireComponent(typeof(IBuildingController))]
    public class CharacterStructureDetector : CharacterBehaviour, IStructureDetector
    {
        public BuildablePreview StructureInView
        {
            get => m_StructureInView;
            set 
            {
                m_StructureInView = value;
                onStructureChanged?.Invoke(m_StructureInView);
            }
        }

        public event UnityAction<BuildablePreview> onStructureChanged;

        [SerializeField, Range(0f, 10f)]
        [Tooltip("The max detection building preview distance.")]
        private float m_MaxDetectionDistance = 5f;

        [SerializeField, Range(0f, 120f)]
        [Tooltip("The max angle detection building preview.")]
        private float m_MaxDetectionAngle = 60f;

        private BuildablePreview m_StructureInView;
        private Vector3 m_LastPlayerPosition = Vector2.zero;
        private Vector2 m_LastPlayerLookAngle = Vector2.zero;

        private ILookHandler m_LookHandler;
        private IBuildingController m_BuildingController;


        public override void OnInitialized()
        {
            GetModule(out m_LookHandler);
            TryGetComponent(out m_BuildingController);

            m_BuildingController.onObjectPlaced += UpdateDetection;
        }

        private void LateUpdate()
        {
            if (!IsInitialized)
                return;

            // Check if the Player position or view rotation changed
            if ((Character.transform.position != m_LastPlayerPosition) || (m_LastPlayerLookAngle != m_LookHandler.LookAngle))
            {
                UpdateDetection();

                m_LastPlayerPosition = Character.transform.position;
                m_LastPlayerLookAngle = m_LookHandler.LookAngle;
            }
        }

        private void UpdateDetection()
        {
            var allPreviews = BuildablePreview.AllPreviewsInScene;

            if (allPreviews.Count > 0)
            {
                for (int i = 0; i < allPreviews.Count; i++)
                {
                    if (allPreviews[i].BuildRequirements.Count == 0)
                        continue;

                    Vector3 playerPosition = Character.transform.position;
                    Vector3 previewPosition = allPreviews[i].Center;
                    Vector3 dirFromPlayerToPreview = previewPosition - playerPosition;

                    float distanceToPreviewSqr = (playerPosition - previewPosition).sqrMagnitude;

                    if (distanceToPreviewSqr < m_MaxDetectionDistance * m_MaxDetectionDistance && Vector3.Angle(dirFromPlayerToPreview, Character.transform.forward) < m_MaxDetectionAngle)
                    {
                        StructureInView = allPreviews[i];

                        return;
                    }
                }
            }

            StructureInView = null;
        }

        public void CancelStructureInView()
        {
            if (m_StructureInView != null)
            {
                m_StructureInView.Cancel();
                StructureInView = null;
            }
        }
    }
}