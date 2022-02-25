using System;
using UnityEngine;
using UnityEngine.UI;

namespace SurvivalTemplatePro.UISystem
{
    public class ItemPickupInteractionUI : PlayerUIBehaviour, IInteractableInfoDisplayer
    {
        public Type InteractableType => typeof(ItemPickup);

        [SerializeField]
        [Tooltip("The main rect transform that will move the interactable's center (should be parent of everything else).")]
        private RectTransform m_RectTransform;

        [SerializeField]
        [Tooltip("The canvas group used to fade the item pickup displayer in & out.")]
        private CanvasGroup m_CanvasGroup;

        [SerializeField]
        [Tooltip("An image that used in showing the time the current interactable has been interacted with.")]
        private Image m_InteractProgressImg;

        [BHeader("Item Info")]

        [SerializeField]
        [Tooltip("A UI text that's used for displaying the observed item pickup's name.")]
        private Text m_NameText;

        [SerializeField]
        [Tooltip("A UI text that's used for displaying the observed item pickup's description.")]
        private Text m_DescriptionText;

        [SerializeField]
        [Tooltip("An UI Image that's used for displaying the observed item pickup's icon.")]
        private Image m_ItemIcon;

        [SerializeField]
        [Tooltip("An object that will be enabled based on if the observed item has a stack that's bigger than one and otherwise disabled.")]
        private GameObject m_StackObject;

        [SerializeField]
        [Tooltip("A UI text that's used for displaying the observed item pickup's current stack size.")]
        private Text m_StackText;

        [SerializeField]
        [Tooltip("A UI text that's used for displaying the observed item pickup's weight.")]
        private Text m_WeightText;

        [SerializeField]
        [Tooltip("A string that will be added at the of the weight amount (e.g. amount + 'KG' or 'LBS')")]
        private string m_WeightSuffix = "KG";

        [Space]

        [SerializeField]
        [Tooltip("An offset that will be applied to the position of the 'rect transform'")]
        private Vector3 m_CustomItemOffset;

        private Camera m_PlayerCamera;
        private ItemPickup m_ItemPickup;
        private bool m_IsVisible;

        private Bounds m_ItemPickupBounds;


        public void ShowInfo(IInteractable interactableObject) => m_IsVisible = true;
        public void HideInfo() => m_IsVisible = false;

        public void UpdateInfo(IInteractable interactableObject)
        {
            m_ItemPickup = interactableObject as ItemPickup;

            if (m_ItemPickup != null && m_ItemPickup.Item != null)
            {
                m_NameText.text = m_ItemPickup.Item.Name;
                m_ItemIcon.sprite = m_ItemPickup.Item.Info.Icon;

                m_StackObject.SetActive(m_ItemPickup.Item.CurrentStackSize > 1);
                m_StackText.text = m_ItemPickup.Item.CurrentStackSize > 1 ? ("x" + m_ItemPickup.Item.CurrentStackSize) : string.Empty;

                m_DescriptionText.text = m_ItemPickup.Item.Info.Description;
                m_WeightText.text = $"{Math.Round(m_ItemPickup.Item.Info.Weight * m_ItemPickup.Item.CurrentStackSize, 2)} {m_WeightSuffix}";

                m_ItemPickupBounds = m_ItemPickup.GetComponentInChildren<Renderer>().bounds;

                SetHeightOfText(m_DescriptionText, m_DescriptionText.text, true);
            }
        }

        public void SetInteractionProgress(float progress) => m_InteractProgressImg.fillAmount = progress;

        public override void OnAttachment()
        {
            m_PlayerCamera = Player.GetModule<ICameraFOVHandler>().UnityWorldCamera;
            m_InteractProgressImg.fillAmount = 0f;
        }

        public override void OnInterfaceUpdate()
        {
            if (!IsInitialized)
                return;

            bool isVisible = false;

            if (m_IsVisible && m_ItemPickup != null && m_ItemPickup.Item != null)
            {
                isVisible = true;

                float boundsMedian = GetMedian(m_ItemPickupBounds.extents.x, m_ItemPickupBounds.extents.y, m_ItemPickupBounds.extents.z);
                Vector3 screenPosition = m_PlayerCamera.WorldToScreenPoint(m_ItemPickupBounds.center + Player.transform.right * boundsMedian);

                PositionAtScreenPoint(m_RectTransform, screenPosition + m_CustomItemOffset);
            }

            m_CanvasGroup.alpha = Mathf.Lerp(m_CanvasGroup.alpha, (isVisible ? 1f : 0f), Time.deltaTime * 20f);
        }

        private float GetMedian(params float[] values)
        {
            float sum = 0f;

            for (int i = 0; i < values.Length; i++)
                sum += values[i];

            return sum / values.Length;
        }

        private void PositionAtScreenPoint(RectTransform rectTransform, Vector2 screenPosition)
        {
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform.parent as RectTransform, screenPosition, null, out Vector2 position))
                rectTransform.anchoredPosition = position;
        }

        private float SetHeightOfText(Text text, string myString, bool autoResizeRect)
        {
            // This is the height that the text would fit at the current font height setting (see the inspector)
            TextGenerator textGen = new TextGenerator();
            TextGenerationSettings generationSettings = text.GetGenerationSettings(text.rectTransform.rect.size);

            float height = textGen.GetPreferredHeight(myString, generationSettings);

            if (autoResizeRect)
            {
                var rt = text.rectTransform;

                // Resize the rect to the size of your text
                rt.sizeDelta = new Vector2(rt.rect.width, height);
            }

            return height;
        }
    }
}