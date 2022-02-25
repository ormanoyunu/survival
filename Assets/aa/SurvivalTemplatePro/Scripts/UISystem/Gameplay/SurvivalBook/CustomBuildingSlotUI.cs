using SurvivalTemplatePro.BuildingSystem;

namespace SurvivalTemplatePro.UISystem
{
    public class CustomBuildingSlotUI : BuildableSlotUI
    {
        public override void UpdateRequirementsUI() { }

        protected override void Awake()
        {
            base.Awake();

            var customBuildingCategory = PlaceableDatabase.GetCategory(PlaceableDatabase.k_CustomBuildingCategoryName);

            if (customBuildingCategory.Placeables != null && customBuildingCategory.Placeables.Length > 0)
                m_Buildable = customBuildingCategory.Placeables[0] as Buildable;

            UpdateRequirementsUI();
        }
    }
}