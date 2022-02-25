using SurvivalTemplatePro.InventorySystem;
using UnityEngine.Events;

namespace SurvivalTemplatePro.WieldableSystem
{
    public interface IWieldable : IMonoBehaviour
    {
        ICharacter Character { get; }
        IAudioPlayer AudioPlayer { get; set; }
        IRayGenerator RayGenerator { get; set; }
        IWieldableEffectsManager EventManager { get; }

        Item AttachedItem { get; }
        ItemProperty ItemDurability { get; }

        bool IsVisible { get; } 

        float EquipDuration { get; }
        float HolsterDuration { get; }

        event UnityAction onEquippingStarted;
        event UnityAction<float> onHolsteringStarted;


        void SetVisibility(bool visible);
        void SetWielder(ICharacter wielder);
        void OnEquip(Item itemToAttach);
        void OnHolster(float holsterSpeed);
    }
}