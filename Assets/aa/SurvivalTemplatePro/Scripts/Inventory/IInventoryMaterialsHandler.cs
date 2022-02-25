using SurvivalTemplatePro.BuildingSystem;

namespace SurvivalTemplatePro
{
    public interface IInventoryMaterialsHandler : ICharacterModule
    {
        void AddMaterial(BuildablePreview structure);
    }
}