namespace SurvivalTemplatePro
{
    public interface IAudioPlayer : ICharacterModule
    {
        void PlaySound(ISound sound);
        void PlaySounds(ISound[] sounds);

        void ClearAllQueuedSounds();

        /// <summary>
        /// A duration of 0 will play the sound indefinitely unless manually stopped
        /// </summary>
        void LoopSound(ISound sound, float duration);
        void StopLoopingSound(ISound sound);
    }
}