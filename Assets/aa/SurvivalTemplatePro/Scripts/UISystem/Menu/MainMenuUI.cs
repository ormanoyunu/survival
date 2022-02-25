using UnityEngine;
using UnityEngine.UI;

namespace SurvivalTemplatePro.UISystem
{
    public class MainMenuUI : Singleton<MainMenuUI>
    {
        [SerializeField]
        private PanelUI m_PlayerNamePanel;

        [SerializeField]
        private Text m_PlayerNameBtn;

        [SerializeField]
        private InputField m_PlayerNameField;


        public void Quit()
        {
            Application.Quit();
        }

        public void SavePlayerNameFromField()
        {
            if (m_PlayerNameField.text == string.Empty)
                return;

            SavePlayerName(m_PlayerNameField.text);
        }

        public void SavePlayerName(string name)
        {
            PlayerPrefs.SetString("PLAYER_NAME", name);
            m_PlayerNameBtn.text = PlayerPrefs.GetString("PLAYER_NAME");
        }

        public void ResetPlayerNameField()
        {
            m_PlayerNameField.text = PlayerPrefs.GetString("PLAYER_NAME");
        }

        private void Start()
        {
            if (!PlayerPrefs.HasKey("PLAYER_NAME") || PlayerPrefs.GetString("PLAYER_NAME") == string.Empty)
            {
                SavePlayerName("Unnamed");
                m_PlayerNamePanel.Show(true);
            }
            else
                m_PlayerNameField.text = m_PlayerNameBtn.text = PlayerPrefs.GetString("PLAYER_NAME");
        }
    }
}