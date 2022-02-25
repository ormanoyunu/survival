using SurvivalTemplatePro.BuildingSystem;
using SurvivalTemplatePro.InventorySystem;
using UnityEngine;
using UnityEngine.UI;

namespace SurvivalTemplatePro.UISystem
{
    public class CampfireUI : PlayerUIBehaviour, IObjectInspector
    {
        public System.Type InspectableType => typeof(Campfire);

        [SerializeField, Range(0f, 10f)]
        private float m_StartFireDuration = 3f;

        [SerializeField, Range(0f, 10f)]
        private float m_StopFireDuration = 3f;

        [Space]

        [SerializeField]
        private PanelUI m_Panel;

        [SerializeField]
        private Button m_StartFireBtn;

        [SerializeField]
        private Button m_AddFuelBtn;

        [SerializeField]
        private Button m_ExtinguishBtn;

        [SerializeField]
        private Text m_DescriptionText;

        [Space]

        [SerializeField]
        private FuelSelectorUI m_FuelSelector;

        [SerializeField]
        private ItemContainerUI m_ItemContainer;

        [BHeader("Audio")]

        [SerializeField]
        private SoundPlayer m_FireStartAudio;

        private Campfire m_Campfire;

        private bool m_FireToggleInProgress;
        private ICustomActionManager m_CustomAction;


        public void Inspect(IInteractable inspectableObject)
        {
            m_Campfire = inspectableObject as Campfire;

            m_Panel.Show(true);

            if (!m_Campfire.FireActive)
                m_DescriptionText.text = string.Empty;

            m_StartFireBtn.gameObject.SetActive(!m_Campfire.FireActive);
            m_AddFuelBtn.gameObject.SetActive(m_Campfire.FireActive);
            m_ExtinguishBtn.interactable = m_Campfire.FireActive;

            m_ItemContainer.AttachToContainer(m_Campfire.ItemContainer);
            m_FuelSelector.AttachToInventory(PlayerInventory);

            m_Campfire.onDescriptionTextChanged += OnCampfireDescriptionUpdate;
            OnCampfireDescriptionUpdate();
        }

        public void EndInspection()
        {
            m_Panel.Show(false);

            if (m_FireToggleInProgress)
            {
                m_CustomAction.TryCancelAction();
                m_FireToggleInProgress = false;
            }

            m_ItemContainer.DetachFromContainer();
            m_FuelSelector.DetachFromInventory();

            m_Campfire.onDescriptionTextChanged -= OnCampfireDescriptionUpdate;
            m_Campfire = null;
        }

        public override void OnAttachment()
        {
            GetModule(out m_CustomAction);

            m_StartFireBtn.onClick.AddListener(QueueFireStart);
            m_ExtinguishBtn.onClick.AddListener(QueueFireStop);

            m_AddFuelBtn.onClick.AddListener(TryAddFuel);

            m_DescriptionText.text = string.Empty;
        }

        private void QueueFireStart()
        {
            if (m_Campfire != null && !m_Campfire.FireActive && m_FuelSelector.SelectedFuel != null)
            {
                bool removedItemFromStorage = PlayerInventory.RemoveItems(m_FuelSelector.SelectedFuel.Id, 1, ItemContainerFlags.Storage) > 0;

                if (removedItemFromStorage)
                {
                    CustomActionParams fireStartParams = new CustomActionParams("Fire Starting", "Starting Fire...", m_StartFireDuration, true, () => ToggleFire(true), CancelFireQueue);
                    m_CustomAction.StartAction(fireStartParams);

                    m_FireToggleInProgress = true;

                    m_FireStartAudio.Play2D();
                }
            }
        }

        private void QueueFireStop()
        {
            if (m_Campfire != null && m_Campfire.FireActive)
            {
                CustomActionParams fireStartParams = new CustomActionParams("Fire Extinguish", "Extinguishing Fire...", m_StopFireDuration, true, () => ToggleFire(false), CancelFireQueue);
                m_CustomAction.StartAction(fireStartParams);

                m_FireToggleInProgress = true;
            }
        }

        private void CancelFireQueue() => m_FireToggleInProgress = false;
        private void OnCampfireDescriptionUpdate() => m_DescriptionText.text = m_Campfire.FireActive ? m_Campfire.DescriptionText : string.Empty;

        private void TryAddFuel()
        {
            if (m_Campfire != null && m_Campfire.FireActive && m_FuelSelector.SelectedFuel != null)
            {
                bool removedItemFromStorage = PlayerInventory.RemoveItems(m_FuelSelector.SelectedFuel.Id, 1, ItemContainerFlags.Storage) > 0;

                if (removedItemFromStorage)
                    m_Campfire.AddFuel(m_FuelSelector.SelectedFuel.Duration);
            }
        }

        private void ToggleFire(bool enableFire)
        {
            m_StartFireBtn.gameObject.SetActive(!enableFire);
            m_AddFuelBtn.gameObject.SetActive(enableFire);
            m_ExtinguishBtn.interactable = enableFire;

            if (enableFire)
                m_Campfire.StartFire(m_FuelSelector.SelectedFuel.Duration);
            else
                m_Campfire.StopFire();

            m_DescriptionText.text = string.Empty;

            m_FireToggleInProgress = false;
        }
    }
}