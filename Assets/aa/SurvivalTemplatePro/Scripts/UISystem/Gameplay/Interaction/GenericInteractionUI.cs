using System;
using UnityEngine;
using UnityEngine.UI;

namespace SurvivalTemplatePro.UISystem
{
    public class GenericInteractionUI : MonoBehaviour, IInteractableInfoDisplayer
    {
        public Type InteractableType => typeof(IInteractable);

        [SerializeField]
        [Tooltip("The UI panel used in showing / hiding the underlying images.")]
        private PanelUI m_Panel;

        [Space]

		[SerializeField]
        [Tooltip("A UI text component that's used for displaying the current interactable's name.")]
        private Text m_InteractableNameText;

        [SerializeField]
        [Tooltip("An image that separate the name text from the description text (optional). " +
                "It gets disabled when the current interactable doesn't have a description.")]
        private Image m_Separator;

        [SerializeField]
        [Tooltip("A UI text component that's used for displaying the current interactable's description.")]
        private Text m_DescriptionText;

        [SerializeField]
        [Tooltip("An image that used in showing the time the current interactable has been interacted with.")]
        private Image m_InteractProgressImg;

        private IInteractable m_AttachedInteractable;


        public void ShowInfo(IInteractable interactableObject)
        {
            if (!string.IsNullOrEmpty(interactableObject.InteractionText))
                m_Panel.Show(true);
        }

        public void HideInfo()
        {
            m_Panel.Show(false);
        }

        public void UpdateInfo(IInteractable interactableObject)
        {
            if (m_AttachedInteractable != null)
                m_AttachedInteractable.onDescriptionTextChanged -= OnDescriptionChanged;

            m_AttachedInteractable = interactableObject;

            if (m_AttachedInteractable != null)
            {
                m_InteractableNameText.text = m_AttachedInteractable.InteractionText;
                m_AttachedInteractable.onDescriptionTextChanged += OnDescriptionChanged;
                OnDescriptionChanged();
            }
        }

        public void SetInteractionProgress(float progress) => m_InteractProgressImg.fillAmount = progress;
        private void Start() => m_InteractProgressImg.fillAmount = 0f;

        private void OnDescriptionChanged()
        {
            m_DescriptionText.text = m_AttachedInteractable.DescriptionText;

            if (m_Separator != null)
                m_Separator.enabled = !string.IsNullOrEmpty(m_DescriptionText.text);
        }
    }
}