using UnityEngine;
using UnityEngine.Events;

namespace SurvivalTemplatePro
{
    public interface IPauseHandler : ICharacterModule
    {
        bool PauseActive { get; }

        event UnityAction onPause;
        event UnityAction onUnpause;

        void RegisterLocker(Object locker, PlayerPauseParams pauseParams);
        void UnregisterLocker(Object locker);

        void RemoveAllLockers();
    }

    public struct PlayerPauseParams
    {
        public readonly bool UnlockCursor;
        public readonly bool BlurBackground;
        public readonly bool BlurForeground;
        public readonly bool EnableColorTweaks;

        public static PlayerPauseParams Default => m_Default;
        private static PlayerPauseParams m_Default = new PlayerPauseParams(false, false, false, false);


        public PlayerPauseParams(bool unlockCursor, bool blurBackground, bool blurForeground, bool enableColorTweaks)
        {
            UnlockCursor = unlockCursor;
            BlurBackground = blurBackground;
            BlurForeground = blurForeground;
            EnableColorTweaks = enableColorTweaks;
        }

        public static PlayerPauseParams operator +(PlayerPauseParams thisParams, PlayerPauseParams pauseParams)
        {
            return new PlayerPauseParams(
                thisParams.UnlockCursor | pauseParams.UnlockCursor,
                thisParams.BlurBackground | pauseParams.BlurBackground,
                thisParams.BlurForeground | pauseParams.BlurForeground,
                thisParams.EnableColorTweaks | pauseParams.EnableColorTweaks);
        }

        public static bool operator ==(PlayerPauseParams thisParams, PlayerPauseParams pauseParams)
        {
            bool areEqual = thisParams.UnlockCursor && pauseParams.UnlockCursor;
            areEqual &= thisParams.BlurBackground && pauseParams.BlurBackground;
            areEqual &= thisParams.BlurForeground && pauseParams.BlurForeground;
            areEqual &= thisParams.EnableColorTweaks && pauseParams.EnableColorTweaks;

            return areEqual;
        }

        public static bool operator !=(PlayerPauseParams thisParams, PlayerPauseParams pauseParams)
        {
            bool areNotEqual = thisParams.UnlockCursor != pauseParams.UnlockCursor;
            areNotEqual |= thisParams.BlurBackground != pauseParams.BlurBackground;
            areNotEqual |= thisParams.BlurForeground != pauseParams.BlurForeground;
            areNotEqual |= thisParams.EnableColorTweaks != pauseParams.EnableColorTweaks;

            return areNotEqual;
        }

        public override bool Equals(object obj) => base.Equals(obj);
        public override int GetHashCode() => base.GetHashCode();
    }
}