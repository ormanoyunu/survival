using UnityEngine;

namespace SurvivalTemplatePro.WieldableSystem
{
    [System.Serializable]
    public class WieldableEffect
    {
        public string Name => m_EffectName;

        [SerializeField]
        private string m_EffectName;


        public virtual void TriggerEffect(float value) { }
    }
}