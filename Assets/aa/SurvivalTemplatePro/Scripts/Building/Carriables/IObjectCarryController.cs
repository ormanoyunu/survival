using UnityEngine.Events;
using SurvivalTemplatePro.BuildingSystem;

namespace SurvivalTemplatePro
{
    public interface IObjectCarryController : ICharacterModule
    {
        CarriableDefinition CarriedObject { get; }
        int CarriedObjectsCount { get; }

        event UnityAction onObjectCarryStart;
        event UnityAction onObjectCarryEnd;

        event UnityAction<int> onCarriedCountChanged;

        bool TryCarryObject(CarriableDefinition definition);
        void DropCarriedObjects(int amount);
        void UseCarriedObject();
    }
}