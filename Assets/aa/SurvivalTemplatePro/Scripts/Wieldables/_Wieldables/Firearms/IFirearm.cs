using UnityEngine;
using UnityEngine.Events;

namespace SurvivalTemplatePro.WieldableSystem
{
    public interface IFirearm : IWieldable
    {
        IFirearmAimer Aimer { get; }
        IFirearmTrigger Trigger { get; }
        IFirearmShooter Shooter { get; }
        IFirearmAmmo Ammo { get; }
        IFirearmReloader Reloader { get; }
        IFirearmRecoil Recoil { get; }

        event UnityAction<IFirearmAimer> onAimerChanged;
        event UnityAction<IFirearmTrigger> onTriggerChanged;
        event UnityAction<IFirearmShooter> onShooterChanged;
        event UnityAction<IFirearmAmmo> onAmmoChanged;
        event UnityAction<IFirearmReloader> onReloaderChanged;
        event UnityAction<IFirearmRecoil> onRecoilChanged;

        Ray GetShootRay(float spreadMod = 1f);

        void SetAimer(IFirearmAimer aimer);
        void SetTrigger(IFirearmTrigger trigger);
        void SetShooter(IFirearmShooter shooter);
        void SetAmmo(IFirearmAmmo ammo);
        void SetReloader(IFirearmReloader reloader);
        void SetRecoil(IFirearmRecoil recoilHandler);
    }
}