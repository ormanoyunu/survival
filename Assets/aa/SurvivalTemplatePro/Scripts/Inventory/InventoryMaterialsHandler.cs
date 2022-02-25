using SurvivalTemplatePro.InventorySystem;

namespace SurvivalTemplatePro.BuildingSystem
{
    /// <summary>
    /// TODO: Remove/Refactor
    /// Acts as a wrapper for adding inventory based materials (e.g. sticks, rope etc.) to a structure.
    /// </summary>
    public class InventoryMaterialsHandler : CharacterBehaviour, IInventoryMaterialsHandler
    {
        private IObjectCarryController m_ObjectCarryController;
        private IInventory m_Inventory;


        public override void OnInitialized()
        {
            GetModule(out m_Inventory);
            GetModule(out m_ObjectCarryController);
        }

        public void AddMaterial(BuildablePreview structure)
        {
            if (m_ObjectCarryController.CarriedObjectsCount > 0)
                return;

            if (structure != null)
            {
                foreach (var buildRequirement in structure.BuildRequirements)
                {
                    if (buildRequirement.RequiredAmount == buildRequirement.CurrentAmount)
                        continue;

                    var materialInfo = BuildMaterialsDatabase.GetBuildingMaterialById(buildRequirement.BuildingMaterialId);

                    if (materialInfo != null)
                    {
                        if (m_Inventory.RemoveItems(materialInfo.Item, 1, ItemContainerFlags.Storage) > 0)
                            structure.TryAddBuildMaterial(materialInfo);
                    }
                }
            }
        }
    }
}