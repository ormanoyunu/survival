using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace SurvivalTemplatePro.UISystem
{
    public class DeathUI : MonoBehaviour
    {
        [SerializeField]
        private FadePanelUI m_FadeScreen;

        [SerializeField, Range(0f, 10f)]
        private float m_FadeDeathDelay = 2f;

        [SerializeField, Range(0f, 10f)]
        private float m_FadeSpawnDelay = 3f;

        [Space]

        [SerializeField]
        private Text m_RespawnTimeText;

        [SerializeField]
        private Button m_RespawnButton;

        [SerializeField, Range(0f, 50f)]
        private float m_ShowRespawnButtonDelay = 5f;

        [Space]

        [SerializeField]
        private AudioMixerSnapshot m_NotAliveSnapshot;

        [SerializeField]
        private AudioMixerSnapshot m_DefaultSnapshot;

        private Player m_Player;


        /// <summary>
        /// Respawn the player by restoring the health to the max amount
        /// </summary>
        public void RespawnPlayer() 
        {
            m_Player.HealthManager.RestoreHealth(m_Player.HealthManager.MaxHealth);
            m_FadeScreen.Fade(false, m_FadeSpawnDelay);

            // Audio
            m_DefaultSnapshot.TransitionTo(m_FadeSpawnDelay * 2f);

            m_RespawnButton.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            m_FadeScreen.Fade(true);
            m_FadeScreen.Fade(false, m_FadeSpawnDelay);

            m_RespawnButton.onClick.AddListener(RespawnPlayer);

            m_RespawnButton.interactable = false;
            m_RespawnButton.gameObject.SetActive(false);

            Player.onLocalPlayerChanged += OnLocalPlayerChanged;

            if (Player.LocalPlayer != null)
                OnLocalPlayerChanged(Player.LocalPlayer);
        }

        private void OnDisable()
        {
            m_RespawnButton.onClick.RemoveListener(RespawnPlayer);

            Player.onLocalPlayerChanged -= OnLocalPlayerChanged;
        }

        private void OnLocalPlayerChanged(Player player)
        {
            if (m_Player != null)
                m_Player.HealthManager.onDeath -= OnPlayerDeath;

            m_Player = player;
            m_Player.HealthManager.onDeath += OnPlayerDeath;
        }

        private void OnPlayerDeath()
        {
            StartCoroutine(C_ShowRespawnPanel());
            m_FadeScreen.Fade(true, m_FadeDeathDelay);

            // Audio
            m_NotAliveSnapshot.TransitionTo(m_FadeDeathDelay * 2f);
        }

        private IEnumerator Start()
        {
            m_NotAliveSnapshot.TransitionTo(0f);

            yield return new WaitForSeconds(0.5f);

            m_DefaultSnapshot.TransitionTo(m_FadeSpawnDelay);
        }

        private IEnumerator C_ShowRespawnPanel() 
        {
            m_RespawnButton.gameObject.SetActive(true);
            m_RespawnButton.interactable = false;

            float currentTimeLeft = m_ShowRespawnButtonDelay;

            while (currentTimeLeft > 0.01f)
            {
                m_RespawnTimeText.text = currentTimeLeft.ToString("0.0");

                currentTimeLeft -= Time.deltaTime;

                yield return null;
            }

            m_RespawnTimeText.text = "Respawn";
            m_RespawnButton.interactable = true;
        }
    }
}