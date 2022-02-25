using System;
using UnityEngine;

namespace SurvivalTemplatePro
{
    /// <summary>
    /// Animator Paramater attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public class AnimatorParameterAttribute : PropertyAttribute
    {
        public AnimatorControllerParameterType ParameterType { get; set; }
        public int SelectedValue { get; set; }


        public AnimatorParameterAttribute(AnimatorControllerParameterType parameterType = AnimatorControllerParameterType.Trigger)
        {
            this.ParameterType = parameterType;
        }
    }
}
