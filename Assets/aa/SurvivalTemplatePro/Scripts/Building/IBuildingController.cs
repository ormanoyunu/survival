using SurvivalTemplatePro.BuildingSystem;
using UnityEngine.Events;

namespace SurvivalTemplatePro
{
    public interface IBuildingController : ICharacterModule
    {
        Placeable CurrentPlaceable { get; }
        BuildableType BuildingMode { get; }

        event UnityAction onBuildingStart;
        event UnityAction onBuildingEnd;
        event UnityAction<Placeable> onObjectChanged;
        event UnityAction onObjectPlaced;

        void StartBuilding(Placeable placeable);
        void EndBuilding();
        void SetPlaceable(Placeable placeable);
        void SelectNextPlaceable(bool next);
        void RotateObject(float rotationDelta);
        void PlaceObject();
    }
}