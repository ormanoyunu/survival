using UnityEngine;

namespace SurvivalTemplatePro.Demo
{
    public class DamageCharacterBehaviour : MonoBehaviour
    {
        [SerializeField, MinMax(0f, 1000f)]
        private Vector2 m_Damage;

        [SerializeField, Range(0f, 100f)]
        private float m_HitImpulse = 5f;

        //karakter damage alma burasý büyük ihtimalle
        public void DamageCharacter(ICharacter character)
        {
            Debug.Log("burasý ne");
            character.HealthManager.ReceiveDamage(new DamageInfo(m_Damage.GetRandomFloat(), DamageType.Cut, transform.position, transform.position - character.transform.position, m_HitImpulse));
        }
    }
}
