using SurvivalTemplatePro.BuildingSystem;
using UnityEngine.Events;

namespace SurvivalTemplatePro
{
    public interface IStructureDetector : ICharacterModule
    {
        BuildablePreview StructureInView { get; }

        event UnityAction<BuildablePreview> onStructureChanged;

        void CancelStructureInView();
    }
}