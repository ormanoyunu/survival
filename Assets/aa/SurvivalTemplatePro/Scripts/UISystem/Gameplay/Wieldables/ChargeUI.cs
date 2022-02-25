using SurvivalTemplatePro.WieldableSystem;
using UnityEngine;
using UnityEngine.UI;

namespace SurvivalTemplatePro.UISystem
{
    public class ChargeUI : PlayerUIBehaviour
    {
        [SerializeField]
        [Tooltip("The canvas group used to fade the stamina bar in & out.")]
        private CanvasGroup m_CanvasGroup;

        [SerializeField]
        [Tooltip("UI images that will have their fill amount value set to the current charge value.")]
        private Image[] m_ChargeFillImages;

        [SerializeField]
        [Tooltip("A gradient used in determining the color of the charge image relative to the current charge value.")]
        private Gradient m_FillGradient;

        private IChargeHandler m_ChargeHandler;
        private IWieldablesController m_WieldableController;
        private IFirearm m_Firearm;


        public override void OnAttachment()
        {
            GetModule(out m_WieldableController);
            m_WieldableController.onWieldableEquipped += OnWieldableEquipped;

            m_CanvasGroup.alpha = 0f;
        }

        public override void OnDetachment()
        {
            m_WieldableController.onWieldableEquipped -= OnWieldableEquipped;
        }

        private void FixedUpdate()
        {
            bool showCanvas = false;

            if (m_ChargeHandler != null)
            {
                float normalizedCharge = m_ChargeHandler.GetNormalizedCharge();
                UpdateChargeImages(normalizedCharge);

                showCanvas = normalizedCharge > 0.01f;
            }

            m_CanvasGroup.alpha = Mathf.Lerp(m_CanvasGroup.alpha, showCanvas ? 1f : 0f, Time.deltaTime * 3f);
        }

        private void OnWieldableEquipped(IWieldable wieldable)
        {
            IFirearm firearm = wieldable as IFirearm;

            // Unsubscribe from previous firearm
            if (m_Firearm != null)
            {
                m_Firearm.onTriggerChanged -= OnTriggerChanged;
                m_Firearm = null;

                UpdateChargeImages(0f);
            }

            if (firearm != null)
            {
                // Subscribe to current firearm
                m_Firearm = firearm;
                m_Firearm.onTriggerChanged += OnTriggerChanged;

                OnTriggerChanged(m_Firearm.Trigger);
            }
        }

        private void UpdateChargeImages(float amount) 
        {
            Color chargeColor = m_FillGradient.Evaluate(amount);

            for (int i = 0; i < m_ChargeFillImages.Length; i++)
            {
                m_ChargeFillImages[i].fillAmount = amount;
                m_ChargeFillImages[i].color = chargeColor;
            }
        }

        private void OnTriggerChanged(IFirearmTrigger currentTrigger) => m_ChargeHandler = currentTrigger as IChargeHandler;
    }
}