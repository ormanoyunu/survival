namespace SurvivalTemplatePro
{
    public interface ISleepModule : ICharacterModule
    {
        public void DoSleepEffects(ISleepingPlace sleepingPlace, float duration);
        public void DoWakeUpEffects(float duration);
    }
}