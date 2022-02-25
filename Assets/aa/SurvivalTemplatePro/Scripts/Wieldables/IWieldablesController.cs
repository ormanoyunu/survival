using SurvivalTemplatePro.InventorySystem;
using SurvivalTemplatePro.WieldableSystem;
using UnityEngine.Events;

namespace SurvivalTemplatePro
{
    public interface IWieldablesController : ICharacterModule
    {
        IWieldable ActiveWieldable { get; }

        float LastEquipTime { get; }
        bool IsEquipping { get; }
        bool IsHolstering { get; }

        event UnityAction<IWieldable> onWieldableEquipped;

        bool GetWieldableOfType<T>(out T wieldable) where T : IWieldable;
        bool HasWieldable(IWieldable wieldable);

        bool TryEquipWieldable(IWieldable wieldable, Item itemToAttach = null, float holsterSpeed = 1f);

        IWieldable SpawnWieldable(IWieldable wieldable);
        bool DestroyWieldable(IWieldable wieldable);
    }
}