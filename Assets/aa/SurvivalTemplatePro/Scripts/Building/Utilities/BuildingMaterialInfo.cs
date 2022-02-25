using UnityEngine;

namespace SurvivalTemplatePro.BuildingSystem
{
    [System.Serializable]
    public class BuildingMaterialInfo
    {
        public int Id => m_Id;
        public string Name => m_Name;
        public Sprite Icon => m_Icon;
        public string Description => m_Description;
        public int Item => m_Item;
        public SoundPlayer UseSound => m_UseAudio;

        [SerializeField]
        private string m_Name;

        [SerializeField, HideInInspector]
        private int m_Id;

        [SerializeField]
        private Sprite m_Icon;

        [SerializeField, Multiline]
        private string m_Description;

        [SerializeField]
        private ItemReference m_Item;

        [Space]

        [SerializeField]
        private SoundPlayer m_UseAudio;
    }
}