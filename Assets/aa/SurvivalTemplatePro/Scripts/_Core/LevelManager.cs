using SurvivalTemplatePro.InputSystem;
using UnityEngine.SceneManagement;

namespace SurvivalTemplatePro
{
    public class LevelManager : Singleton<LevelManager>
    {
        public static void LoadScene(string sceneName)
        {
            SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);
            SceneManager.activeSceneChanged += Instance.OnActiveSceneChanged;
        }

        private void OnActiveSceneChanged(Scene arg0, Scene arg1) => InputBlockManager.ClearAllBlockers();
    }
}