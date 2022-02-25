using UnityEngine.Events;

namespace SurvivalTemplatePro.WieldableSystem
{
    public interface IFirearmReloader : IFirearmAttachment
    {
        bool IsReloading { get; }

        int AmmoInMagazine { get; set; }
        int AmmoToLoad { get; }
        int MagazineSize { get; }
        bool IsMagazineFull { get; }
        bool IsMagazineEmpty { get; }

        /// <summary> Prev ammo, Current ammo </summary>
        event UnityAction<int, int> onAmmoInMagazineChanged;
        event UnityAction onReloadStart;
        event UnityAction onReloadFinish;


        bool TryStartReload(IFirearmAmmo ammoModule);
        void CancelReload(IFirearmAmmo ammoModule);
        bool TryUseAmmo(int amount);
    }
}