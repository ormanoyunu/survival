using UnityEngine;
using UnityEngine.Events;

namespace SurvivalTemplatePro.WieldableSystem
{
    [RequireComponent(typeof(Wieldable))]
    public class WieldableDurabilityDepleter : MonoBehaviour
    {
        [SerializeField, ArrayElementAttribute("Depletion:", "Durability/s", 100, 70)]
        private float m_DurabilityPerSecond = 1f;

        [Space]

        [SerializeField]
        private UnityEvent m_OnDurabilityDepleted;

        [SerializeField]
        private UnityEvent m_OnDurabilityRestored;

        [Space]

        [SerializeField]
        private StandardSound m_DurabilityDepletedSound;

        private IWieldable m_Wieldable;
        private float m_DurabilityToDeplete;
        private bool m_DepleteDurability;
        private bool m_DurabilityDepleted;
        private float m_LastDurability;


        public void StartDepletion() => m_DepleteDurability = true;
        public void EndDepletion(float holsterSpeed) => m_DepleteDurability = false;

        private void Awake()
        {
            m_Wieldable = GetComponent<IWieldable>();

            m_Wieldable.onEquippingStarted += StartDepletion;
            m_Wieldable.onHolsteringStarted += EndDepletion;
        }

        private void Update()
        {
            if (!m_Wieldable.IsVisible || !m_DepleteDurability || m_Wieldable.ItemDurability == null)
                return;

            m_DurabilityToDeplete += m_DurabilityPerSecond * Time.deltaTime;

            if (m_DurabilityToDeplete >= m_DurabilityPerSecond)
            {
                // On Durability Increased && Previoulsy fully depleted
                if (m_DurabilityDepleted && m_Wieldable.ItemDurability.Float > m_LastDurability)
                {
                    m_DurabilityDepleted = false;
                    m_OnDurabilityRestored?.Invoke();
                }

                m_Wieldable.ItemDurability.Float = Mathf.Max(m_Wieldable.ItemDurability.Float - m_DurabilityToDeplete, 0f);

                m_LastDurability = m_Wieldable.ItemDurability.Float;
                m_DurabilityToDeplete = 0f;

                // On Depleted Durability
                if (!m_DurabilityDepleted && m_LastDurability <= 0.001f)
                {
                    m_DurabilityDepleted = true;
                    m_OnDurabilityDepleted?.Invoke();

                    m_Wieldable.AudioPlayer.PlaySound(m_DurabilityDepletedSound);
                }
            }
        }
    }
}