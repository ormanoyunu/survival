using UnityEngine;

namespace SurvivalTemplatePro.WorldManagement
{
    public struct GameTime
    {
        public readonly int Hours;
        public readonly int Minutes;
        public readonly int Seconds;


        public GameTime(float realtimeDuration, float inGameDayScale)
        {
            float inGameDuration = realtimeDuration / inGameDayScale;

            this.Hours = (int)(inGameDuration / 3600);
            this.Minutes = (int)((inGameDuration - this.Hours * 3600) / 60);
            this.Seconds = Mathf.CeilToInt(inGameDuration - this.Hours * 3600 - this.Minutes * 60);
        }

        public GameTime(float hours, float minutes, float seconds)
        {
            this.Hours = (int)hours;
            this.Minutes = (int)minutes;
            this.Seconds = (int)seconds;
        }

        public GameTime(int hours, int minutes, int seconds)
        {
            this.Hours = hours;
            this.Minutes = minutes;
            this.Seconds = seconds;
        }

        public float GetNormalized()
        {
            float normalizedTime = Mathf.Max(this.Hours - 1, 0) / 24f;
            normalizedTime += Mathf.Max(this.Minutes, 0) / 60f;
            normalizedTime += Mathf.Max(this.Seconds, 0) / 3600f;

            return normalizedTime;
        }

        public string GetTimeToString(bool hours, bool minutes, bool seconds)
        {
            string timeStr = string.Empty;

            if (hours)
                timeStr += this.Hours.ToString("00");

            if (minutes)
                timeStr += (hours ? ":" : "") + this.Minutes.ToString("00");

            if (seconds)
                timeStr += (minutes ? ":" : "") + this.Seconds.ToString("00");

            return timeStr;
        }

        public string GetTimeToStringWithSuffixes(bool hours, bool minutes, bool seconds)
        {
            string timeStr = string.Empty;

            if (hours && this.Hours > 0.01f)
                timeStr += (this.Hours + "h ");

            if (minutes && this.Minutes > 0.01f)
                timeStr += (this.Minutes + "m ");

            if (seconds && this.Seconds > 0.01f)
                timeStr += (this.Seconds + "s ");

            return timeStr;
        }
    }
}