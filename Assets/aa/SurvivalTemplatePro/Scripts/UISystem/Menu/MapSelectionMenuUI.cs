using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace SurvivalTemplatePro.UISystem
{
    public class MapSelectionMenuUI : MonoBehaviour
    {
        #region Internal
        [System.Serializable]
        private class MapUI
        {
            public SceneField Scene;
            public string Name;
            public Sprite Sprite;
        }
        #endregion

        [SerializeField]
        private FadePanelUI m_FadePanel;

        [SerializeField, Range(0f, 10f)]
        private float m_LoadDelay;

        [Space]

        [SerializeField]
        private GameObject m_MapSelectionTemplate;

        [SerializeField]
        private RectTransform m_SpawnRoot;

        [Space]

        [SerializeField]
        private MapUI[] m_Maps;

        private bool m_IsLoading;


        private void Awake()
        {
            SetupTemplates();

            m_FadePanel.Fade(false);
        }

        private void SetupTemplates() 
        {
            foreach (var map in m_Maps)
            {
                var template = Instantiate(m_MapSelectionTemplate, m_SpawnRoot);

                // Set the template text to the map name
                var templateName = template.GetComponentInChildren<Text>();
                if (templateName != null)
                    templateName.text = map.Name;

                // Set the template sprite to the map sprite
                var templateSprite = template.transform.FindDeepChild("Sprite").GetComponent<Image>();
                if (templateSprite != null)
                    templateSprite.sprite = map.Sprite;

                // Set the template button to load the map
                var templateButton = template.GetComponentInChildren<Button>();
                if (templateButton != null)
                    templateButton.onClick.AddListener(() => LoadLevel(map.Scene));
            }
        }

        private void LoadLevel(string sceneName) 
        {
            if (!m_IsLoading)
                StartCoroutine(C_LoadLevel(sceneName));
        }

        private IEnumerator C_LoadLevel(string sceneName)
        {
            m_IsLoading = true;
            m_FadePanel.Fade(true, 0.2f);
            yield return new WaitForSeconds(m_LoadDelay);
            LevelManager.LoadScene(sceneName);
        }
    }
}