namespace SurvivalTemplatePro.WorldManagement
{
    public abstract class WorldManagerBase : Singleton<WorldManagerBase>
    {
        public virtual bool TimeProgressionEnabled { get; set; }

        public virtual float GetNormalizedTime() => 0f; 
        public virtual TimeOfDay GetTimeOfDay() => TimeOfDay.Day;
        public virtual GameTime GetGameTime() => new GameTime(0f, 0f, 0f);
        public virtual float GetTemperature() => 0f;
        public virtual float GetDayDurationInMinutes() => 0f;
        public virtual float GetTimeIncrementPerSecond() => 0f;

        /// <summary>
        /// A time to pass of 1 is equal to a full day, duration is seconds.
        /// </summary>
        /// <param name="timeToPass"> normalized time</param>
        /// <param name="duration"> duration in seconds</param>
        public virtual void PassTime(float timeToPass, float duration) { }
    }

    public enum TimeOfDay
    {
        Day, Night
    }
}