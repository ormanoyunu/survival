using System;
using UnityEngine;
using UnityEngine.Events;

namespace SurvivalTemplatePro
{
    public interface ICharacterMover : ICharacterModule
    {
        /// <summary>
        /// 
        /// </summary>
        CharMotionMask ActiveMotions { get; }

        /// <summary>
        /// 
        /// </summary>
        CharMotionMask BlockedMotions { get; set; }

        /// <summary>
        /// 
        /// </summary>
        ICharacterMotor Motor { get; }

        /// <summary>
        /// 
        /// </summary>
        float VelocityMod { get; set; }

        /// <summary>
        /// 
        /// </summary>
        float StepCycle { get; }

        /// <summary>
        /// 
        /// </summary>
        event UnityAction<CharMotionMask, bool> onMotionChanged;

        /// <summary>
        /// 
        /// </summary>
        event UnityAction<float> onFallImpact;

        /// <summary>
        /// 
        /// </summary>
        event UnityAction onStepCycleEnded;

        /// <summary>
        /// 
        /// </summary>
        void Move(Vector3 moveDirection, CharMotionMask motionInputs);

        /// <summary>
        /// 
        /// </summary>
        void ResetStateAndVelocity();
    }

    /// <summary>
    /// Character motion flags.
    /// </summary>
    [Flags]
    public enum CharMotionMask
    {
        Run = 1,
        Crouch = 8,
        Jump = 256,
    }

    public static class CharacterMotionsExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        public static CharMotionMask SetFlag(this CharMotionMask motionMask, CharMotionMask motion)
        {
            motionMask |= motion;
            return motionMask;
        }

        /// <summary>
        /// 
        /// </summary>
        public static CharMotionMask UnsetFlag(this CharMotionMask motionMask, CharMotionMask motion)
        {
            motionMask &= (~motion);
            return motionMask;
        }

        /// <summary>
        /// 
        /// </summary>
        public static bool Has(this CharMotionMask motionFlags, CharMotionMask motion)
        {
            return (motionFlags & motion) == motion;
        }
    }
}