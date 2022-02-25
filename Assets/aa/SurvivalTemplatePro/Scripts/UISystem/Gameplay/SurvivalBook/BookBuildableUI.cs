using SurvivalTemplatePro.BuildingSystem;
using System.Collections.Generic;
using UnityEngine;

namespace SurvivalTemplatePro.UISystem
{
    public class BookBuildableUI : BookCategoryUI
    {
        [SerializeField]
        private BuildableSlotUI m_Template;

        [SerializeField]
        private RectTransform m_TemplateSpawnRect;

        [SerializeField]
        private string m_BuildableCategory;

        [Space]

        [SerializeField]
        private PlaceableReference[] m_CustomBuildables;

        [SerializeField]
        private BuildableSlotUI[] m_CustomSlots;

        private BuildableSlotUI[] m_BuildableSlots;


        protected override void Awake()
        {
            base.Awake();

            var correspondingCategory = PlaceableDatabase.GetCategory(m_BuildableCategory);

            if (correspondingCategory == null)
            {
                Debug.LogError($"The category '{m_BuildableCategory}' doesn't exist.");
                return;
            }

            List<Buildable> correspondingBuildables = new List<Buildable>();

            for (int i = 0; i < correspondingCategory.Placeables.Length; i++)
            {
                var buildable = correspondingCategory.Placeables[i] as Buildable;

                if (buildable != null)
                    correspondingBuildables.Add(buildable);
            }

            for (int i = 0; i < m_CustomBuildables.Length; i++)
            {
                var buildable = m_CustomBuildables[i].GetPlaceable() as Buildable;

                if (buildable != null && !correspondingBuildables.Contains(buildable))
                    correspondingBuildables.Add(buildable);
            }

            m_BuildableSlots = new BuildableSlotUI[correspondingBuildables.Count];

            for (int i = 0; i < m_BuildableSlots.Length; i++)
            {
                m_BuildableSlots[i] = Instantiate(m_Template.gameObject, m_TemplateSpawnRect).GetComponent<BuildableSlotUI>();
                m_BuildableSlots[i].DisplayBuildable(correspondingBuildables[i]);
                m_BuildableSlots[i].UpdateRequirementsUI();

                m_BuildableSlots[i].onClick += StartBuilding;
            }

            for (int i = 0; i < m_CustomSlots.Length; i++)
                m_CustomSlots[i].onClick += StartBuilding;
        }

        private void StartBuilding(Buildable buildable)
        {
            Player.GetModule<IWieldableSurvivalBookHandler>().ToggleInspection();
            Player.GetModule<IBuildingController>().StartBuilding(buildable);
        }
    }
}