using UnityEngine;

namespace SurvivalTemplatePro.UISystem
{
    public interface IItemWheelUI
    {
        ItemWheelState ItemWheelState { get; }
        bool IsVisible { get; }

        void SetItemWheelState(ItemWheelState wheelState);
        void EndInspection();
        void StartInspection();
        void UpdateSelection(Vector2 input);
    }

    public enum ItemWheelState 
    {
        SelectItems,
        InsertItems
    }
}