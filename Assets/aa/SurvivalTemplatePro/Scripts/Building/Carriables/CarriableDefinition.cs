using SurvivalTemplatePro.WieldableSystem;
using UnityEngine;

namespace SurvivalTemplatePro.BuildingSystem
{
    [CreateAssetMenu(menuName = "Survival Template Pro/Building/Carriable Definition")]
    public class CarriableDefinition : ScriptableObject
    {
        public int BuildMaterial => m_BuildMaterial;
        public int MaxCarryCount => m_MaxCarryCount;
        public IWieldable TargetWieldable => m_Wieldable;
        public GameObject TargetCarriable => m_Carriable;
        public ObjectDropSettings DropSettings => m_DropSettings;
        public SoundPlayer CarrySound => m_CarrySound;

        [SerializeField]
        private BuildMaterialReference m_BuildMaterial;

        [SerializeField]
        private int m_MaxCarryCount;

        [Space]

        [SerializeField]
        [InfoBox("Corresponding Wieldable")]
        private Wieldable m_Wieldable;

        [SerializeField]
        [InfoBox("Corresponding Carriable")]
        private GameObject m_Carriable;

        [Space]

        [SerializeField]
        private ObjectDropSettings m_DropSettings;

        [SerializeField]
        private SoundPlayer m_CarrySound;
    }
}