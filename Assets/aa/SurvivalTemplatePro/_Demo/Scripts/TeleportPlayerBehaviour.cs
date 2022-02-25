using UnityEngine;
using System.Collections;

namespace SurvivalTemplatePro.Demo
{
    public class TeleportPlayerBehaviour : MonoBehaviour
    {
        [SerializeField]
        private bool m_TeleportOnTrigger;

        [SerializeField]
        private float m_TeleportDelay = 0.3f;

        [Space]

        [SerializeField]
        private Transform[] m_TeleportPoints;


        public void TeleportPlayer(ICharacter character)
        {
            var controller = character.GetComponent<CharacterController>();
            StartCoroutine(C_TeleportController(controller));
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!m_TeleportOnTrigger)
                return;

            if (other.CompareTag("Player"))
            {
                if (other.TryGetComponent(out CharacterController controller))
                    StartCoroutine(C_TeleportController(controller));
            }
        }

        private IEnumerator C_TeleportController(CharacterController controller) 
        {
            yield return new WaitForSeconds(m_TeleportDelay);

            controller.enabled = false;
            controller.transform.position = m_TeleportPoints.SelectRandom().position;
            controller.enabled = true;
        }
    }
}