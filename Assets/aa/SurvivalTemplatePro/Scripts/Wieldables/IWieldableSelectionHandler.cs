using UnityEngine.Events;

namespace SurvivalTemplatePro
{
    public interface IWieldableSelectionHandler : ICharacterModule
    {
        int SelectedIndex { get; }
        int PreviousIndex { get; }

        /// <summary>
        /// 
        /// </summary>
        event UnityAction<int> onSelectedChanged;

        void Refresh();
        void SelectAtIndex(int index, float holsterPrevWieldableSpeed = 1f);
    }
}