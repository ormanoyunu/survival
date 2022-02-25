using SurvivalTemplatePro.UISystem;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SurvivalTemplatePro.Gameplay
{
    public class StandardPlayerSpawner : MonoBehaviour
    {
        [SerializeField]
        private Player m_PlayerPrefab;

        [SerializeField]
        private PlayerUIBehavioursManager m_PlayerUIPrefab;

        [Space]

        [SerializeField]
        private GameObject m_SceneCamera;

        private Player m_Player;
        private PlayerUIBehavioursManager m_PlayerUI;


        public SpawnPointInfo GetPlayerSpawnPoint()
        {
            var player = Player.LocalPlayer;

            SpawnPointInfo spawnPoint = SpawnPointInfo.Default;

            if (Application.isPlaying)
            {
                // Set the spawn position to the sleeping place.
                if (player.TryGetModule(out ISleepHandler sleepHandler))
                    spawnPoint = new SpawnPointInfo(sleepHandler.LastSleepPosition, sleepHandler.LastSleepRotation);
            }

            if (spawnPoint == SpawnPointInfo.Default)
            {
                // Search for random spawn point.
                var foundSpawnPoints = FindObjectsOfType<SpawnPoint>();

                if (foundSpawnPoints != null && foundSpawnPoints.Length > 0)
                    spawnPoint = foundSpawnPoints.SelectRandom().GetSpawnPoint();
            }

            if (spawnPoint == SpawnPointInfo.Default)
            {
                // Snaps the spawn point position to the ground.
                if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out RaycastHit hitInfo, 10f))
                    spawnPoint = new SpawnPointInfo(hitInfo.point + Vector3.up * 0.1f, transform.rotation);
                else
                    spawnPoint = new SpawnPointInfo(transform.position, transform.rotation);
            }

            return spawnPoint;
        }

        private void Start()
        {
            SetupPlayer();
            SetupUI();
        }

        private void SetupPlayer() 
        {
            if (TryGetPlayer(out m_Player))
            {
                m_Player.HealthManager.onRespawn += SetPlayerPosition;
                SetPlayerPosition();
            }

            m_SceneCamera.GetComponent<Camera>().enabled = false;
            Destroy(m_SceneCamera);
        }

        private void SetupUI()
        {
            if (TryGetPlayerUI(out m_PlayerUI))
                m_PlayerUI.AttachToPlayer(m_Player);
        }

        private bool TryGetPlayer(out Player player)
        {
            player = Object.FindObjectOfType<Player>();

            if (player == null)
            {
                if (m_PlayerPrefab != null)
                    player = Instantiate(m_PlayerPrefab);
                else
                    Debug.LogError("Player prefab null, assign it in the inspector.");
            }

            return player != null;
        }

        private bool TryGetPlayerUI(out PlayerUIBehavioursManager playerUI) 
        {
            playerUI = Object.FindObjectOfType<PlayerUIBehavioursManager>();

            if (playerUI == null)
            {
                if (m_PlayerUIPrefab != null)
                    playerUI = Instantiate(m_PlayerUIPrefab);
                else
                    Debug.LogError("Player UI prefab null, assign it in the inspector.");
            }

            return playerUI != null;
        }

        private void SetPlayerPosition() 
        {
            // Get a spawn point
            SpawnPointInfo spawnPoint = GetPlayerSpawnPoint();

            // Set the player's position and rotation.
            m_Player.transform.SetPositionAndRotation(spawnPoint.Position, spawnPoint.Rotation);
        }
    }
}