using UnityEngine.Events;

namespace SurvivalTemplatePro.WieldableSystem
{
    public abstract class FirearmAmmoBehaviour : FirearmAttachmentBehaviour, IFirearmAmmo
    {
        public event UnityAction<int> onAmmoInStorageChanged;


        public virtual int RemoveAmmo(int amount) => 0;
        public virtual int AddAmmo(int amount) => 0;
        public virtual int GetAmmoCount() => 0;

        protected virtual void OnEnable() => Firearm.SetAmmo(this);
        protected virtual void OnDisable() { }

        protected void RaiseAmmoChangedEvent(int ammo) => onAmmoInStorageChanged?.Invoke(ammo);
    }
}