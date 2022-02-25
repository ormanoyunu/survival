using System.Collections;
using UnityEngine;

namespace SurvivalTemplatePro.WieldableSystem
{
    public class FPFirearmProjectileCorrector : MonoBehaviour
    {
        [SerializeField]
        private FPFirearmCartridge m_FPCartridge = null;

        [SerializeField, Range(0f, 10f)]
        private float m_ReloadUpdateDelay = 0.5f;

        private IFirearmReloader m_Reloader;
        private IFirearm m_Firearm;

        private WaitForSeconds m_ReloadWait;


        private void Awake()
        {
            m_Firearm = GetComponentInParent<IFirearm>();
            m_ReloadWait = new WaitForSeconds(m_ReloadUpdateDelay);
        }

        private void Start()
        {
            m_Firearm.onReloaderChanged += OnReloaderChanged;
            OnReloaderChanged(m_Firearm.Reloader);
        }

        private void OnDestroy()
        {
            if (m_Firearm != null)
                m_Firearm.onReloaderChanged -= OnReloaderChanged;
        }

        private void OnReloaderChanged(IFirearmReloader reloader)
        {
            // Unsubscribe from previous reloader
            if (m_Reloader != null)
            {
                m_Reloader.onAmmoInMagazineChanged -= OnAmmoChanged;
                m_Reloader.onReloadStart -= OnReloadStart;
            }

            // Subscribe to current reloader
            m_Reloader = reloader;

            if (m_Reloader != null)
            {
                m_Reloader.onAmmoInMagazineChanged += OnAmmoChanged;
                m_Reloader.onReloadStart += OnReloadStart;
                OnAmmoChanged(m_Reloader.AmmoInMagazine, m_Reloader.AmmoInMagazine);
            }
        }

        protected virtual void OnAmmoChanged(int prevInMag, int currentInMag)
        {
            if (m_FPCartridge == null || m_Reloader.IsReloading)
                return;

            if (currentInMag == 0)
                m_FPCartridge.ChangeState(false);
        }

        private void OnReloadStart() => StartCoroutine(C_ReloadUpdateCartridges(m_Reloader.AmmoToLoad));

        private IEnumerator C_ReloadUpdateCartridges(int reloadingAmount)
        {
            yield return m_ReloadWait;

            if (reloadingAmount > 0)
                m_FPCartridge.ChangeState(true);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (Application.isPlaying)
                m_ReloadWait = new WaitForSeconds(m_ReloadUpdateDelay);
        }
#endif
    }
}