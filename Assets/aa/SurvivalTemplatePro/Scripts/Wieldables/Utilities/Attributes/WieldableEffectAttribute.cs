using System;
using UnityEngine;

namespace SurvivalTemplatePro.WieldableSystem
{
    #region Internal
    public enum WieldableEffectPlayType
    {
        PlayEffect,
        StopEffect
    }
    #endregion

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class WieldableEffectAttribute : PropertyAttribute
    {
        public WieldableEffectPlayType PlayType { get; }


        public WieldableEffectAttribute(WieldableEffectPlayType type = WieldableEffectPlayType.PlayEffect)
        {
            this.PlayType = type;
        }
    }
}