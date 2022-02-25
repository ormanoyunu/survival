using UnityEditor;

namespace SurvivalTemplatePro
{
    [CustomEditor(typeof(CharacterBehaviour), true)]
    public class CharacterBehaviourEditor : InterfaceModuleRequirementEditor<ICharacterModule>
    {
        protected override string GetRequiredString() => "Required Module";      
    }
}