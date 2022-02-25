using UnityEngine;
using UnityEngine.Events;

namespace SurvivalTemplatePro.WieldableSystem
{
    public abstract class FirearmReloaderBehaviour : FirearmAttachmentBehaviour, IFirearmReloader
    {
        #region Internal
        protected enum ReloadType
        {
            Standard,
            Progressive
        }
        #endregion

        public bool IsReloading
        {
            get => m_IsReloading;
            protected set
            {
                if (value != m_IsReloading)
                {
                    m_IsReloading = value;

                    if (m_IsReloading)
                        onReloadStart?.Invoke();
                    else
                        onReloadFinish?.Invoke();
                }
            }
        }

        public int AmmoInMagazine
        {
            get => m_AmmoInMagazine;
            set
            {
                int clampedValue = Mathf.Clamp(value, 0, MagazineSize);

                if (clampedValue != m_AmmoInMagazine)
                {
                    int prevInMagazine = m_AmmoInMagazine;
                    m_AmmoInMagazine = clampedValue;
                    onAmmoInMagazineChanged?.Invoke(prevInMagazine, m_AmmoInMagazine);
                }
            }
        }

        public virtual int MagazineSize => 0;
        public virtual int AmmoToLoad => 0;

        public bool IsMagazineEmpty => AmmoInMagazine <= 0;
        public bool IsMagazineFull => AmmoInMagazine >= MagazineSize;

        public event UnityAction<int, int> onAmmoInMagazineChanged;
        public event UnityAction onReloadFinish;
        public event UnityAction onReloadStart;

        private bool m_IsReloading;
        private int m_AmmoInMagazine;


        protected virtual void OnEnable() => Firearm.SetReloader(this);

        protected virtual void OnDisable() 
        {
            CancelReload(Firearm.Ammo);
            IsReloading = false;
        }

        public abstract bool TryStartReload(IFirearmAmmo ammoModule);
        public abstract void CancelReload(IFirearmAmmo ammoModule);
        public abstract bool TryUseAmmo(int amount);
    }
}