namespace SurvivalTemplatePro.UISystem
{
    public interface IPlayerUIBehavioursManager
    {
        void AttachToPlayer(ICharacter player);
        void DetachFromPlayer(ICharacter player);

        void AddBehaviour(IPlayerUIBehaviour playerUIBehaviour);
        void RemoveBehaviour(IPlayerUIBehaviour playerUIBehaviour);
    }
}