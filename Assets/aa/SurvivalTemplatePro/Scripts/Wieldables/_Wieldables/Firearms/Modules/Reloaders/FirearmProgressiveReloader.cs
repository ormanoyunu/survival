using UnityEngine;

namespace SurvivalTemplatePro.WieldableSystem
{
    public class FirearmProgressiveReloader : FirearmReloaderBehaviour
	{
		public override int MagazineSize => m_MagazineSize;
		public override int AmmoToLoad => m_AmmoToLoad;

		[Space]

		[SerializeField, Range(0, 500)]
		private int m_MagazineSize;

		[Space]

		[SerializeField, Range(0f, 15f)]
		private float m_ReloadStartDuration;

		[SerializeField, Range(0f, 15f)]
		private float m_ReloadLoopDuration;

		[Space]

		[SerializeField]
		private ReloadType m_EmptyReloadType = ReloadType.Standard;

		[SerializeField, Range(0f, 15f)]
		private float m_EmptyReloadDuration = 3f;

		[SerializeField]
		private WieldableObjectSpawnEffect m_EmptyReloadEffect;

		[BHeader("Audio")]

		[SerializeField]
		private DelayedSoundRandom[] m_ReloadStartSounds;

		[SerializeField]
		private DelayedSoundRandom[] m_ReloadLoopSounds;

		[SerializeField]
		private DelayedSoundRandom[] m_ReloadEndSounds;

		[SerializeField]
		private DelayedSoundRandom[] m_EmptyReloadSounds;

		[BHeader("Local Effects")]

		[SerializeField, WieldableEffect]
		private int[] m_ReloadStartEffects;

		[SerializeField, WieldableEffect]
		private int[] m_ReloadLoopEffects;

		[SerializeField, WieldableEffect]
		private int[] m_ReloadEndEffects;

		[SerializeField, WieldableEffect]
		private int[] m_EmptyReloadEffects;

		private IFirearmAmmo m_AmmoModule;
		private int m_AmmoToLoad = 0;

		private bool m_ReloadLoopActive;
		private float m_ReloadLoopEndTime;
		private float m_ReloadLoopStartTime;


		public override bool TryUseAmmo(int amount)
		{
			if (IsMagazineEmpty || AmmoInMagazine < amount)
				return false;

			AmmoInMagazine -= amount;

			return true;
		}

		public override void CancelReload(IFirearmAmmo ammoModule)
		{
			if (!IsReloading)
				return;

			IsReloading = false;

			Firearm.AudioPlayer.ClearAllQueuedSounds();

			EndReload();
		}

		public override bool TryStartReload(IFirearmAmmo ammoModule)
		{
			if (IsReloading || IsMagazineFull)
				return false;

			m_AmmoToLoad = MagazineSize - AmmoInMagazine;
			int currentInStorage = ammoModule.GetAmmoCount();

			if (currentInStorage < m_AmmoToLoad)
				m_AmmoToLoad = currentInStorage;

			if (!IsMagazineFull && m_AmmoToLoad > 0)
			{
				// Start Empty Reload
				if (IsMagazineEmpty)
				{
					m_ReloadLoopStartTime = Time.time + m_EmptyReloadDuration;

					// Audio
					Firearm.AudioPlayer.PlaySounds(m_EmptyReloadSounds);

					// Do Reload Effect
					if (m_EmptyReloadEffect != null)
						m_EmptyReloadEffect.DoEffect(Firearm.Character);

					// Local Effects
					EventManager.PlayEffects(m_EmptyReloadEffects, 1f);
				}
				// Start Tactical Reload
				else
				{
					m_ReloadLoopStartTime = Time.time + m_ReloadStartDuration;

					// Audio
					Firearm.AudioPlayer.PlaySounds(m_ReloadStartSounds);

					// Local Effects
					EventManager.PlayEffects(m_ReloadStartEffects, 1f);
				}

				IsReloading = true;
				m_ReloadLoopActive = false;

				m_AmmoModule = ammoModule;

				return true;
			}

			return false;
		}

		private void Update()
		{
			if (!IsReloading)
				return;

			// Start reload loop
			if (!m_ReloadLoopActive && Time.time > m_ReloadLoopStartTime)
				StartReloadLoop();

			// Update Reload loop
			if (m_ReloadLoopActive && Time.time > m_ReloadLoopEndTime)
			{
				if (UpdateReloadLoop())
					IsReloading = false;
			}
		}

		private void StartReloadLoop() 
		{
			// Empty Reload
			if (IsMagazineEmpty)
			{
				if (m_EmptyReloadType == ReloadType.Progressive)
				{
					m_AmmoModule.RemoveAmmo(1);
					AmmoInMagazine++;
					m_AmmoToLoad--;

					m_ReloadLoopEndTime = Time.time + m_ReloadStartDuration;

					// Audio
					Firearm.AudioPlayer.PlaySounds(m_ReloadStartSounds);

					// Local Effects
					EventManager.PlayEffects(m_ReloadStartEffects, 1f);
				}
				else
				{
					m_AmmoModule.RemoveAmmo(m_AmmoToLoad);
					AmmoInMagazine += m_AmmoToLoad;
					m_AmmoToLoad = 0;

					IsReloading = false;

					// No need to start the loop since the firearm is fully reloaded
					return;
				}
			}

			m_ReloadLoopActive = true;
		}

		private bool UpdateReloadLoop() 
		{
			if (m_AmmoToLoad > 0)
			{
				m_AmmoModule.RemoveAmmo(1);
				AmmoInMagazine++;
				m_AmmoToLoad--;

				m_ReloadLoopEndTime = Time.time + m_ReloadLoopDuration;

				// Audio
				Firearm.AudioPlayer.PlaySounds(m_ReloadLoopSounds);

				// Local Effects
				EventManager.PlayEffects(m_ReloadLoopEffects, 1f);
			}
			else
			{
				EndReload();

				return true;
			}

			return false;
		}

		private void EndReload() 
		{
			// Audio
			Firearm.AudioPlayer.PlaySounds(m_ReloadEndSounds);

			// Local Effects
			EventManager.PlayEffects(m_ReloadEndEffects, 1f);
		}
	}
}