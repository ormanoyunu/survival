using UnityEngine;

namespace SurvivalTemplatePro.Demo
{
    public class MessagePopup : MonoBehaviour
    {
        [SerializeField]
        private Animator m_InfoAnimator;

        [Space]

        [SerializeField]
        private SpriteRenderer m_Icon;

        [SerializeField]
        private Color m_MessageSeenIconColor;

        [Space]

        [SerializeField]
        private SoundPlayer m_ProximityEnterAudio;

        [SerializeField]
        private SoundPlayer m_ProxmityExitAudio;

        private bool m_PreviouslyInProximity;



        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                m_PreviouslyInProximity = true;
                m_InfoAnimator.SetBool("IsInPlayerProximity", true);

                m_ProximityEnterAudio.Play2D();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                m_InfoAnimator.SetBool("IsInPlayerProximity", false);

                if (m_PreviouslyInProximity)
                    m_Icon.color = m_MessageSeenIconColor;

                m_ProxmityExitAudio.Play2D();
            }
        }
    }
}
