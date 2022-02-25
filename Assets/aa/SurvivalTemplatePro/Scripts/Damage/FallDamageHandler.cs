using UnityEngine;

namespace SurvivalTemplatePro
{
    /// <summary>
    /// Handles dealing fall damage to the character based on the impact velocity.
    /// </summary>
    public class FallDamageHandler : CharacterBehaviour
    {
        [SerializeField]
        private bool m_EnableDamage = true;

        [Space]

        [InfoBox("At which landing speed, the character will start taking damage.")]
        [SerializeField, Range(1f, 30f)] 
        private float m_MinFallSpeed = 12f;

        [Space]

        [InfoBox("At which landing speed, the character will die, if it has no defense.")]
        [SerializeField, Range(1f, 50f)]
        private float m_FatalFallSpeed = 30f;


        public override void OnInitialized()
        {
            Character.Mover.onFallImpact += OnFallImpact;
        }

        private void OnFallImpact(float impactSpeed)
        {
            if (!m_EnableDamage)
                return;

            if (impactSpeed >= m_MinFallSpeed)
                Character.HealthManager.ReceiveDamage(new DamageInfo(-100f * (impactSpeed / m_FatalFallSpeed)));
        }
    }
}