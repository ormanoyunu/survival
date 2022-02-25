using SurvivalTemplatePro.BuildingSystem;
using UnityEngine;

namespace SurvivalTemplatePro.UISystem
{
    public class BuildMaterialsUI : PlayerUIBehaviour
    {
        [SerializeField]
        private BuildMaterialUI m_BuildMaterialTemplate;

        [SerializeField, Range(3, 30)]
        private int m_CachedBuildMaterialCount = 10;

        [Space]

        [SerializeField]
        private CanvasGroup m_CanvasGroup;

        [SerializeField, Range(1f, 20f)]
        private float m_AlphaLerpSpeed = 5f;

        private BuildMaterialUI[] m_MaterialDisplayers;
        private float m_TargetAlpha;

        private IWieldableSurvivalBookHandler m_SurvivalBook;
        private IStructureDetector m_StructureManager;

        private BuildablePreview m_StructureInView;
        private RectTransform m_RectTransform;
        private Camera m_Camera;


        private void Awake()
        {
            m_MaterialDisplayers = new BuildMaterialUI[m_CachedBuildMaterialCount];

            for (int i = 0; i < m_CachedBuildMaterialCount; i++)
            {
                m_MaterialDisplayers[i] = Instantiate(m_BuildMaterialTemplate, m_BuildMaterialTemplate.transform.parent);
                m_MaterialDisplayers[i].gameObject.SetActive(false);
            }

            m_BuildMaterialTemplate.gameObject.SetActive(false);
        }

        public override void OnAttachment()
        {
            GetModule(out m_SurvivalBook);
            GetModule(out m_StructureManager);

            m_Camera = GetModule<ICameraFOVHandler>().UnityWorldCamera;
            m_RectTransform = GetComponent<RectTransform>();

            m_StructureManager.onStructureChanged += OnStructureInViewChanged;
        }

        public override void OnInterfaceUpdate()
        {
            if (m_StructureInView != null)
            {
                SetBuildRequirementsPosition(m_StructureInView);
                SetBuildRequirementsInfo(m_StructureInView);
            }

            if (m_SurvivalBook.InspectionActive)
                m_TargetAlpha = 0f;

            m_CanvasGroup.alpha = Mathf.MoveTowards(m_CanvasGroup.alpha, m_TargetAlpha, Time.deltaTime * m_AlphaLerpSpeed);
        }

        private void SetBuildRequirementsPosition(BuildablePreview preview)
        {
            Vector2 screenPositionOfPreview = m_Camera.WorldToScreenPoint(preview.Center);
            Vector2 positionOfUI;

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)transform.parent, screenPositionOfPreview, null, out positionOfUI))
                m_RectTransform.localPosition = positionOfUI;
        }

        private void SetBuildRequirementsInfo(BuildablePreview preview) 
        {
            if (preview != null)
            {
                for (int i = 0; i < m_MaterialDisplayers.Length; i++)
                {
                    BuildMaterialUI displayer = m_MaterialDisplayers[i];

                    if (i < preview.BuildRequirements.Count)
                    {
                        displayer.gameObject.SetActive(true);

                        BuildRequirement requirement = preview.BuildRequirements[i];

                        BuildingMaterialInfo buildingMaterial = BuildMaterialsDatabase.GetBuildingMaterialById(requirement.BuildingMaterialId);

                        if (buildingMaterial != null)
                            displayer.Display(buildingMaterial.Icon, requirement.CurrentAmount + "/" + requirement.RequiredAmount);
                    }
                    else
                        displayer.gameObject.SetActive(false);
                }
            }
        }

        private void OnStructureInViewChanged(BuildablePreview preview)
        {
            m_StructureInView = preview;
            m_TargetAlpha = preview != null ? 1f : 0f;
        }
    }
}