using UnityEngine;
using UnityEngine.UI;

namespace SurvivalTemplatePro.UISystem
{
    public class BookCategoryUI : SelectableUI, IBookCategoryUI
    {
        protected ICharacter Player { get; private set; }

        [Space]

        [SerializeField]
        private RectTransform m_CorrepondingContent;

        [SerializeField, Range(0, 100)]
        private int m_SelectedFontSize = 15;

        [SerializeField]
        private Color m_SelectedTextColor = Color.white;

        private Text m_CategoryName;
        private int m_OriginalFontSize;
        private Color m_OriginalTextColor;
        private FontStyle m_OriginalFontStyle;


        public virtual void AttachToPlayer(ICharacter player) => Player = player;

        public override void Select()
        {
            base.Select();

            m_CategoryName.fontStyle = FontStyle.Bold;
            m_CategoryName.fontSize = m_SelectedFontSize;
            m_CategoryName.color = m_SelectedTextColor;

            m_CorrepondingContent.gameObject.SetActive(true);
        }

        public override void Deselect()
        {
            base.Deselect();

            m_CategoryName.fontStyle = m_OriginalFontStyle;
            m_CategoryName.fontSize = m_OriginalFontSize;
            m_CategoryName.color = m_OriginalTextColor;

            m_CorrepondingContent.gameObject.SetActive(false);
        }       

        protected virtual void Awake()
        {
            m_CategoryName = GetComponentInChildren<Text>();
            m_OriginalFontSize = m_CategoryName.fontSize;
            m_OriginalTextColor = m_CategoryName.color;
            m_OriginalFontStyle = m_CategoryName.fontStyle;
        }
    }
}
