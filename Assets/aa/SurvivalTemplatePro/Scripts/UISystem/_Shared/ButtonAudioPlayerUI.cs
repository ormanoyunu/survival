using UnityEngine;
using UnityEngine.UI;

namespace SurvivalTemplatePro.UISystem
{
    public class ButtonAudioPlayerUI : MonoBehaviour
    {
        [SerializeField]
        private SoundPlayer m_OnClickAudio;


        private void Awake() => GetComponent<Button>().onClick.AddListener(OnButtonClick);
        private void OnButtonClick() => m_OnClickAudio.Play2D();
    }
}