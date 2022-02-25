using SurvivalTemplatePro.WieldableSystem;
using UnityEngine;
using UnityEngine.UI;

namespace SurvivalTemplatePro.UISystem
{
    public class AmmoUI : PlayerUIBehaviour
    {
        [SerializeField]
        [Tooltip("Reference to the animator used for the Ammo UI.")]
        private Animator m_Animator;

        [Space]

        [SerializeField]
        [Tooltip("A UI text component that's used for displaying the current ammo in the magazine.")]
        private Text m_MagazineText;

        [SerializeField]
        [Tooltip("A UI text component that's used for displaying the current ammo in the storage.")]
        private Text m_InventoryText;

        private int m_HashedDecreaseTrigger;
        private int m_HashedShowTrigger;
        private int m_HashedHideTrigger;

        private IWieldablesController m_WieldableController;
        private IFirearmReloader m_Reloader;
        private IFirearmAmmo m_Ammo;
        private IFirearm m_Firearm;


        public override void OnAttachment()
        {
            m_HashedDecreaseTrigger = Animator.StringToHash("Decrease");
            m_HashedShowTrigger = Animator.StringToHash("Show");
            m_HashedHideTrigger = Animator.StringToHash("Hide");

            m_Animator.Play(m_HashedHideTrigger, 0, 1f);

            GetModule(out m_WieldableController);
            m_WieldableController.onWieldableEquipped += OnWieldableEquipped;
        }

        private void OnWieldableEquipped(IWieldable wieldable)
        {
            IFirearm firearm = wieldable as IFirearm;

            // Unsubscribe from previous firearm
            if (m_Firearm != null)
            {
                m_Firearm.onReloaderChanged -= OnReloaderChanged;
                m_Firearm.onAmmoChanged -= OnAmmoChanged;

                m_Firearm = null;

                m_Animator.SetTrigger(m_HashedHideTrigger);
            }

            if (firearm != null)
            {
                // Subscribe to current firearm
                m_Firearm = firearm;
                m_Firearm.onReloaderChanged += OnReloaderChanged;
                m_Firearm.onAmmoChanged += OnAmmoChanged;

                OnReloaderChanged(m_Firearm.Reloader);
                OnAmmoChanged(m_Firearm.Ammo);

                m_Animator.SetTrigger(m_HashedShowTrigger);
            }
        }

        private void OnReloaderChanged(IFirearmReloader currentReloader)
        {
            // Prev reloader
            if (m_Reloader != null)
                m_Reloader.onAmmoInMagazineChanged -= UpdateMagazineText;

            // Current reloader
            if (currentReloader != null)
            {
                m_Reloader = currentReloader;

                m_Reloader.onAmmoInMagazineChanged += UpdateMagazineText;
                UpdateMagazineText(m_Reloader.AmmoInMagazine, m_Reloader.AmmoInMagazine);
            }
        }

        private void OnAmmoChanged(IFirearmAmmo currentAmmo)
        {
            // Prev ammo
            if (m_Ammo != null)
                m_Ammo.onAmmoInStorageChanged -= UpdateStorageText;

            // Current ammo
            if (currentAmmo != null)
            {
                m_Ammo = currentAmmo;

                m_Ammo.onAmmoInStorageChanged += UpdateStorageText;
                UpdateStorageText(m_Ammo.GetAmmoCount());
            }
        }

        private void UpdateMagazineText(int prevAmmo, int currentAmmo) 
        {
            m_MagazineText.text = currentAmmo.ToString();

            if (prevAmmo > currentAmmo)
                m_Animator.SetTrigger(m_HashedDecreaseTrigger);
        }

        private void UpdateStorageText(int currentAmmo)
        {
            m_InventoryText.text = currentAmmo.ToString();
        }
    }
}