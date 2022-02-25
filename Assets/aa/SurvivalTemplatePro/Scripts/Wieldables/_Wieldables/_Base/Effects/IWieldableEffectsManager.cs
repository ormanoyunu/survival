namespace SurvivalTemplatePro.WieldableSystem
{
    public interface IWieldableEffectsManager : IMonoBehaviour
    {
        void PlayEffect(int effectId, float value);
        void PlayEffects(int[] effectIds, float value);
        void StopEffects(int[] handlersId);

#if UNITY_EDITOR
        string[] GetAllEffectNames();
        string[] GetAllEffectHandlerNames();
        int IndexOfEffectWithId(int id);
        int IndexOfEffectHandlerWithId(int id);
#endif
    }
}