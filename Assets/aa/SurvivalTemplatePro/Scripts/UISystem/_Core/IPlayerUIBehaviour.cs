namespace SurvivalTemplatePro.UISystem
{
    public interface IPlayerUIBehaviour
    {
        void InitBehaviour(ICharacter player);
        void OnAttachment();
        void OnDetachment();

        void OnInterfaceUpdate();
    }
}