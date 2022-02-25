using UnityEngine;
using UnityEngine.Events;

namespace SurvivalTemplatePro
{
    public interface ICharacterMotor
    {
        Transform transform { get; }

        /// <summary>
        /// 
        /// </summary>
        bool IsGrounded { get; }

        /// <summary>
        /// 
        /// </summary>
        Vector3 Velocity { get; }

        /// <summary>
        /// 
        /// </summary>
        Vector3 GroundNormal { get; }

        /// <summary>
        /// 
        /// </summary>
        float Height { get; }

        /// <summary>
        /// 
        /// </summary>
        float Radius { get; }

        /// <summary>
        /// 
        /// </summary>
        float SlopeLimit { get; }

        /// <summary>
        /// 
        /// </summary>
        event UnityAction<bool> onGroundedStateChanged;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="height"></param>
        void SetHeight(float height);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="translation"></param>
        void Move(Vector3 translation);
    }
}