using SurvivalTemplatePro.BuildingSystem;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SurvivalTemplatePro.UISystem
{
    public class BuildableSlotUI : Selectable
    {
        /// <summary>
        /// Int: Placeable Id
        /// </summary>
        public event UnityAction<Buildable> onClick;

        [SerializeField]
        private SoundPlayer m_PointerEnterAudio;

        [Space]

        [SerializeField]
        private Text m_BuildableName;

        [SerializeField]
        private Image m_BuildableIcon;

        [SerializeField]
        private Text m_BuildableDescription;
        
        [Space]

        [SerializeField]
        private RequirementUI[] m_Requirements;

        [SerializeField]
        private Color m_RequirementsColor;

        protected Buildable m_Buildable;


        public virtual void DisplayBuildable(Buildable buildable)
        {
            m_BuildableName.text = buildable.PlaceableName;
            m_BuildableIcon.sprite = buildable.PlaceableIcon;
            m_BuildableDescription.text = buildable.PlaceableDescription;

            m_Buildable = buildable;
        }

        public virtual void UpdateRequirementsUI()
        {
            if (m_Buildable == null)
                return;

            var requirements = m_Buildable.BuildRequirements;

            for (int i = 0; i < m_Requirements.Length; i++)
            {
                if (i > requirements.Length - 1)
                {
                    m_Requirements[i].gameObject.SetActive(false);
                    continue;
                }

                m_Requirements[i].gameObject.SetActive(true);

                BuildRequirementInfo requirement = requirements[i];
                BuildingMaterialInfo requiredMaterial = BuildMaterialsDatabase.GetBuildingMaterialById(requirement.BuildingMaterialId);

                if (requiredMaterial != null)
                    m_Requirements[i].Display(requiredMaterial.Icon, "x" + requirement.RequiredAmount, m_RequirementsColor);
            }
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);

            m_PointerEnterAudio.Play2D();
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            onClick?.Invoke(m_Buildable);
        }
    }
}
