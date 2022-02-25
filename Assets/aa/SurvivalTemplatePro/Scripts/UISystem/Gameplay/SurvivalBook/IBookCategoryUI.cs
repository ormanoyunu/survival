using UnityEngine.Events;

namespace SurvivalTemplatePro.UISystem
{
    public interface IBookCategoryUI
    {
        void AttachToPlayer(ICharacter player);
        void Select();
        void Deselect();
    }
}