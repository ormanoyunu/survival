using UnityEngine;

namespace SurvivalTemplatePro.BuildingSystem
{
    public class Carriable : Interactable
    {
        [Space]

        [SerializeField]
        [Tooltip("The corresponding carriable definition.")]
        private CarriableDefinition m_Definition;


        public override void OnInteract(ICharacter character)
        {
            if (character.TryGetModule(out IObjectCarryController objectCarry))
            {
                if (objectCarry.TryCarryObject(m_Definition))
                    Destroy(gameObject);
            }
        }
    }
}