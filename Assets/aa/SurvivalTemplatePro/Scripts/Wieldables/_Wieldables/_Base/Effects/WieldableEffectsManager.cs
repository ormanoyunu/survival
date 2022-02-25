using System.Collections.Generic;
using UnityEngine;

namespace SurvivalTemplatePro.WieldableSystem
{
    public class WieldableEffectsManager : MonoBehaviour, IWieldableEffectsManager
    {
        private Dictionary<int, WieldableEffect> m_Effects = new Dictionary<int, WieldableEffect>();
        private Dictionary<int, WieldableEffectsHandler> m_EffectHandlers = new Dictionary<int, WieldableEffectsHandler>();


        public void PlayEffect(int effectId, float value)
        {
            if (m_Effects.TryGetValue(effectId, out WieldableEffect effect))
                effect.TriggerEffect(value);
        }

        public void PlayEffects(int[] effectIds, float value)
        {
            for (int i = 0; i < effectIds.Length; i++)
            {
                if (m_Effects.TryGetValue(effectIds[i], out WieldableEffect effect))
                    effect.TriggerEffect(value);
            }
        }

        public void StopEffects(int[] handlersId)
        {
            for (int i = 0; i < handlersId.Length; i++)
            {
                if (m_EffectHandlers.TryGetValue(handlersId[i], out var handler))
                    handler.StopEffects();
            }
        }

        private void Awake() => GenerateDictionaries();

        private void GenerateDictionaries() 
        {
            var foundHandlers = GetComponentsInChildren<WieldableEffectsHandler>();

            if (foundHandlers != null && foundHandlers.Length > 0)
            {
                foreach (var handler in foundHandlers)
                {
                    int handlerHash = Animator.StringToHash(handler.GetType().Name);

                    if (!m_EffectHandlers.ContainsKey(handlerHash))
                    {
                        m_EffectHandlers.Add(handlerHash, handler);

                        var foundEffects = handler.GetAllEffects();

                        if (foundEffects != null && foundEffects.Length > 0)
                        {
                            foreach (var effect in foundEffects)
                            {
                                int effectHash = Animator.StringToHash(handler.GetType().Name + "/" + effect.Name);

                                if (!m_Effects.ContainsKey(effectHash))
                                    m_Effects.Add(effectHash, effect);
                            }
                        }
                    }
                }
            }
        }

#if UNITY_EDITOR
        public int IndexOfEffectWithId(int id)
        {
            int i = 0;

            var foundHandlers = GetComponentsInChildren<WieldableEffectsHandler>();

            if (foundHandlers != null && foundHandlers.Length > 0)
            {
                foreach (var handler in GetComponentsInChildren<WieldableEffectsHandler>())
                {
                    var foundEffects = handler.GetAllEffects();

                    if (foundEffects != null && foundEffects.Length > 0)
                    {
                        foreach (var effect in foundEffects)
                        {
                            int effectHash = Animator.StringToHash(handler.GetType().Name + "/" + effect.Name);

                            if (effectHash == id)
                                return i;

                            i++;
                        }
                    }
                }
            }

            return i;
        }

        public string[] GetAllEffectNames()
        {
            List<string> effectNames = new List<string>();

            var foundHandlers = GetComponentsInChildren<WieldableEffectsHandler>();

            if (foundHandlers != null && foundHandlers.Length > 0)
            {
                foreach (var handler in foundHandlers)
                {
                    var foundEffects = handler.GetAllEffects();

                    if (foundEffects != null && foundEffects.Length > 0)
                    {
                        foreach (var effect in handler.GetAllEffects())
                        {
                            if (!string.IsNullOrEmpty(effect.Name))
                                effectNames.Add(handler.GetType().Name + "/" + effect.Name);
                        }
                    }
                }
            }

            return effectNames.ToArray();
        }

        public string[] GetAllEffectHandlerNames()
        {
            List<string> handlerNames = new List<string>();

            var foundHandlers = GetComponentsInChildren<WieldableEffectsHandler>();

            if (foundHandlers != null && foundHandlers.Length > 0)
            {
                foreach (var handler in foundHandlers)
                    handlerNames.Add(handler.GetType().Name);
            }

            return handlerNames.ToArray();
        }

        public int IndexOfEffectHandlerWithId(int id)
        {
            int i = 0;

            var foundHandlers = GetComponentsInChildren<WieldableEffectsHandler>();

            if (foundHandlers != null && foundHandlers.Length > 0)
            {
                foreach (var handler in GetComponentsInChildren<WieldableEffectsHandler>())
                {
                    int handlerHash = Animator.StringToHash(handler.GetType().Name);

                    if (handlerHash == id)
                        return i;

                    i++;
                }
            }

            return i;
        }
#endif
    }
}