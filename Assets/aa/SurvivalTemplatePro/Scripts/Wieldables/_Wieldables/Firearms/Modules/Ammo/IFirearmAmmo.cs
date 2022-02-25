using UnityEngine.Events;

namespace SurvivalTemplatePro.WieldableSystem
{
    public interface IFirearmAmmo : IFirearmAttachment
    {
        event UnityAction<int> onAmmoInStorageChanged;

        /// <summary>
        /// 
        /// </summary>
        int RemoveAmmo(int amount);

        /// <summary>
        /// 
        /// </summary>
        int AddAmmo(int amount);

        /// <summary>
        /// 
        /// </summary>
        int GetAmmoCount();
    }
}