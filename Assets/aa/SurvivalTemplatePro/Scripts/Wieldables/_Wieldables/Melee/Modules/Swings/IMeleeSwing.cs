namespace SurvivalTemplatePro.WieldableSystem
{
    public interface IMeleeSwing
    {
        float SwingDuration { get; }
        float AttackEffort { get; }


        bool CanSwing();
        void DoSwing(ICharacter user);
    }
}