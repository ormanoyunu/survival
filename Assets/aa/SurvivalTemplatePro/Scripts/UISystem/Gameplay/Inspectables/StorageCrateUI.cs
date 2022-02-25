using SurvivalTemplatePro.BuildingSystem;
using UnityEngine;

namespace SurvivalTemplatePro.UISystem
{
    public class StorageCrateUI : PlayerUIBehaviour, IObjectInspector
    {
        public System.Type InspectableType => typeof(StorageCrate);

        [SerializeField]
        private PanelUI m_Panel;

        [SerializeField]
        private ItemContainerUI m_ItemContainer;

        private StorageCrate m_Crate;


        public void Inspect(IInteractable inspectableObject)
        {
            m_Panel.Show(true);
            m_Crate = inspectableObject as StorageCrate;

            m_ItemContainer.AttachToContainer(m_Crate.ItemContainer);

            m_Crate.OpenCrate();
        }

        public void EndInspection()
        {
            m_Crate.CloseCrate();

            m_ItemContainer.DetachFromContainer();
            m_Crate = null;

            m_Panel.Show(false);
        }
    }
}