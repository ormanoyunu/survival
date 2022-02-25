using SurvivalTemplatePro.BuildingSystem;
using SurvivalTemplatePro.InventorySystem;
using UnityEngine;
using UnityEngine.UI;

namespace SurvivalTemplatePro.UISystem
{
    public class WorkbenchUI : PlayerUIBehaviour, IObjectInspector
    {
        public System.Type InspectableType => typeof(Workbench);

        [SerializeField]
        private PanelUI m_Panel;

        [SerializeField]
        private ItemContainerUI m_ContainerUI;

        [Space]

        [SerializeField]
        private Button m_RepairButton;

        [BHeader("Message")]

        [SerializeField]
        private Text m_PlaceItemMsg;

        [SerializeField]
        private string m_CanNotRepairItemText;

        [SerializeField]
        private string m_PlaceItemText;

        [BHeader("Required Items")]

        [SerializeField]
        private GameObject m_RequiredItemsHeader;

        [SerializeField]
        private GameObject m_RequiredItemsRoot;

        [SerializeField]
        private RequirementUI m_RequiredItemTemplate;

        [SerializeField]
        private Color m_EnoughItemsColor = Color.gray;

        [SerializeField]
        private Color m_NotEnoughItemsColor = new Color(0.7f, 0f, 0f, 0.7f);

        private Workbench m_Workbench;
        private RequirementUI[] m_RequiredItemUIs;


        public void Inspect(IInteractable inspectableObject)
        {
            m_Panel.Show(true);
            m_Workbench = inspectableObject as Workbench;

            m_ContainerUI.AttachToContainer(m_Workbench.ItemContainer);

            m_Workbench.onUpdated += UpdateUI;
            PlayerInventory.onContainerChanged += OnInventoryChanged;

            UpdateUI();
        }

        public void EndInspection()
        {
            m_ContainerUI.DetachFromContainer();

            m_Workbench.onUpdated -= UpdateUI;
            PlayerInventory.onContainerChanged -= OnInventoryChanged;

            m_Workbench = null;

            m_Panel.Show(false);
        }

        private void OnInventoryChanged(ItemSlot itemSlot) => UpdateUI();

        private void Awake()
        {
            m_RequiredItemUIs = new RequirementUI[4];

            for (int i = 0; i < m_RequiredItemUIs.Length; i++)
            {
                m_RequiredItemUIs[i] = Instantiate(m_RequiredItemTemplate, m_RequiredItemsRoot.transform);
                m_RequiredItemUIs[i].gameObject.SetActive(false);
            }

            m_RepairButton.onClick.AddListener(OnRepairBtnClicked);
        }

        private void OnRepairBtnClicked()
        {
            if (m_Workbench.RepairDuration > 0.01f && Player.TryGetModule(out ICustomActionManager customActionManager))
            {
                CustomActionParams repairStartParams = new CustomActionParams("Item Repair", "Repairing Item...", m_Workbench.RepairDuration, true, OnRepairDone, null);
                customActionManager.StartAction(repairStartParams);
            }
            else
                OnRepairDone();
        }

        private void OnRepairDone()
        {
            m_Workbench.RepairItem(Player);
            UpdateUI();
        }

        private void UpdateUI()
        {
            if (m_Workbench == null)
                return;

            bool hasItemToRepair = m_Workbench.ItemToRepair != null;
            bool canRepairItem = m_Workbench.CanRepairItem();

            m_PlaceItemMsg.gameObject.SetActive(!canRepairItem);
            m_PlaceItemMsg.text = hasItemToRepair && !canRepairItem ? m_CanNotRepairItemText : m_PlaceItemText;

            m_RequiredItemsHeader.SetActive(canRepairItem);
            m_RequiredItemsRoot.SetActive(canRepairItem);

            bool hasEnoughItems = true;

            if (canRepairItem)
            {
                for (int i = 0; i < m_RequiredItemUIs.Length; i++)
                {
                    if (m_Workbench.RepairRequirementsForCurrentItem.Count > i)
                    {
                        m_RequiredItemUIs[i].gameObject.SetActive(true);

                        CraftRequirement requirement = m_Workbench.RepairRequirementsForCurrentItem[i];
                        ItemInfo itemInfo = requirement.Item.GetItem();

                        if (itemInfo != null)
                        {
                            bool weHaveEnough = PlayerInventory.GetItemCount(requirement.Item) >= requirement.Amount;

                            if (!weHaveEnough)
                                hasEnoughItems = false;

                            m_RequiredItemUIs[i].Display(itemInfo.Icon, itemInfo.Name + " x" + requirement.Amount, weHaveEnough ? m_EnoughItemsColor : m_NotEnoughItemsColor);
                        }
                    }
                    else
                    {
                        m_RequiredItemUIs[i].gameObject.SetActive(false);
                    }
                }
            }

            m_RepairButton.gameObject.SetActive(canRepairItem && hasEnoughItems);
        }
    }
}