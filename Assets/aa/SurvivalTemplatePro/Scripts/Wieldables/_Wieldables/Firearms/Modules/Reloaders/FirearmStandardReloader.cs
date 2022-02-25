using UnityEngine;

namespace SurvivalTemplatePro.WieldableSystem
{
    public class FirearmStandardReloader : FirearmBasicReloader
    {
        [BHeader("Empty Reload")]

        [SerializeField, Range(0.01f, 15f)]
        private float m_EmptyReloadDuration = 3f;

        [SerializeField]
        private WieldableObjectSpawnEffect m_EmptyReloadEffect;

        [BHeader("Audio")]

        [SerializeField]
        private DelayedSoundRandom[] m_EmptyReloadSounds;

        [BHeader("Local Effects")]

        [SerializeField, WieldableEffect]
        private int[] m_EmptyReloadEffects;


        public override bool TryStartReload(IFirearmAmmo ammoModule)
        {
            if (!IsMagazineEmpty)
            {
                base.TryStartReload(ammoModule);
                return false;
            }

            if (IsReloading || IsMagazineFull)
                return false;

            m_AmmoToLoad = ammoModule.RemoveAmmo(MagazineSize - AmmoInMagazine);

            // Empty Reload
            m_ReloadEndTime = Time.time + m_EmptyReloadDuration;

            // Play Empty reload audio
            Firearm.AudioPlayer.PlaySounds(m_EmptyReloadSounds);

            // Do Reload Effect
            if (m_EmptyReloadEffect != null)
                m_EmptyReloadEffect.DoEffect(Firearm.Character);

            // Local Effects
            EventManager.PlayEffects(m_EmptyReloadEffects, 1f);

            IsReloading = true;

            return true;
        }
    }
}