using SurvivalTemplatePro.BuildingSystem;
using UnityEngine;
using UnityEngine.UI;

namespace SurvivalTemplatePro.UISystem
{
    public class CustomBuildablesSelectionUI : PlayerUIBehaviour
    {
        [SerializeField]
        private Image m_PreviousImg;

        [SerializeField]
        private Image m_CurrentImg;

        [SerializeField]
        private Image m_NextImg;

        private IBuildingController m_BuildingController;


        public override void OnAttachment() => GetModule(out m_BuildingController);

        public void CurrentPlaceableChanged(Placeable placeable) 
        {
            if (m_BuildingController.BuildingMode == BuildableType.SocketBased)
            {
                var customBuildingParts = PlaceableDatabase.GetCategory(PlaceableDatabase.k_CustomBuildingCategoryName).Placeables;

                int currentIndex = GetIndexOfCustomBuildingPlaceable(placeable.PlaceableID, customBuildingParts);
                int previousIdx = (int)Mathf.Repeat(currentIndex - 1, customBuildingParts.Length);
                int nextIdx = (int)Mathf.Repeat(currentIndex + 1, customBuildingParts.Length);

                if (currentIndex != -1)
                {
                    m_CurrentImg.sprite = customBuildingParts[currentIndex].PlaceableIcon;
                    m_PreviousImg.sprite = customBuildingParts[previousIdx].PlaceableIcon;
                    m_NextImg.sprite = customBuildingParts[nextIdx].PlaceableIcon;
                }
            }
        }

        // Called through unity events
        public void CustomBuildingStarted() => CurrentPlaceableChanged(m_BuildingController.CurrentPlaceable);

        private int GetIndexOfCustomBuildingPlaceable(int placeableId, Placeable[] placeables) 
        {
            for (int i = 0; i < placeables.Length; i++)
            {
                if (placeables[i].PlaceableID == placeableId)
                    return i;
            }

            return -1;
        }
    }
}