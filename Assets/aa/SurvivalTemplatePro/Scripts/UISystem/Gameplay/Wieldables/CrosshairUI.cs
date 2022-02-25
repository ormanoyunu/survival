using UnityEngine;
using UnityEngine.UI;

namespace SurvivalTemplatePro.UISystem
{
    [RequireComponent(typeof(RectTransform))]
    public class CrosshairUI : MonoBehaviour
    {
        [Range(1f, 50f)]
        public float m_BaseSize = 5f;

        [Range(1f, 50f)]
        public float m_SizeFadeSpeed = 5f;

        [SerializeField, HideInInspector]
        private RectTransform m_RectTransform;

        [SerializeField, HideInInspector]
        private Image[] m_Images;

        private float m_CurrentSize;


        public void Show()
        {
            SetSize(0f);
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            SetSize(0f);
            gameObject.SetActive(false);
        }

        public void SetSize(float size) => m_RectTransform.sizeDelta = new Vector2(size, size);

        public void UpdateSize(float sizeMod) 
        {
            m_CurrentSize = Mathf.Lerp(m_CurrentSize, m_BaseSize * sizeMod * 10f, Time.fixedDeltaTime * m_SizeFadeSpeed) ;
            m_RectTransform.sizeDelta = new Vector2(m_CurrentSize, m_CurrentSize);
        }

        public void SetColor(Color color)
        {
            foreach (Image image in m_Images)
                image.color = color;
        }

        private void Awake() => SetSize(0f);

#if UNITY_EDITOR
        private void OnValidate()
        {
            m_Images = GetComponentsInChildren<Image>();
            m_RectTransform = GetComponent<RectTransform>();

            SetSize(m_BaseSize);
        }
#endif
    }
}