using SurvivalTemplatePro.WieldableSystem;
using System.Collections.Generic;
using UnityEngine;

namespace SurvivalTemplatePro.UISystem
{
    public class CrosshairManagerUI : PlayerUIBehaviour
    {
        [SerializeField]
        [Tooltip("Default Crosshair: The default (starting) crosshair.")]
        private CrosshairUI m_DefaultCrosshair;

        [Space]

        [SerializeField]
        [Tooltip("The canvas group used to fade a crosshair in & out.")]
        private CanvasGroup m_CanvasGroup;

        [SerializeField, Range(0.1f, 1f)]
        [Tooltip("The max crosshairs alpha.")]
        private float m_CrosshairAlpha = 0.7f;

        [SerializeField, Range(0.5f, 35f)]
        [Tooltip("The speed at which the crosshairs will change their alpha.")]
        private float m_AlphaLerpSpeed = 5f;

        private List<CrosshairUI> m_Crosshairs = new List<CrosshairUI>();
        private CrosshairUI m_CurrentCrosshair;

        private IUseHandler m_UseHandler;
        private ICrosshairHandler m_CrosshairHandler;
        private IInteractionHandler m_InteractionHandler;
        private IWieldablesController m_WieldableController;


        public override void OnAttachment()
        {
            GetModule(out m_InteractionHandler);
            GetModule(out m_WieldableController);

            SetupCrosshairs();

            m_WieldableController.onWieldableEquipped += OnWieldableChanged;
            OnWieldableChanged(m_WieldableController.ActiveWieldable);
        }

        private void SetupCrosshairs()
        {
            m_Crosshairs.Add(m_DefaultCrosshair);

            foreach (var crosshair in GetComponentsInChildren<CrosshairUI>(true))
            {
                if (!m_Crosshairs.Contains(crosshair))
                    m_Crosshairs.Add(crosshair);

                crosshair.Hide();
            }

            m_CurrentCrosshair = m_DefaultCrosshair;
        }

        private void OnWieldableChanged(IWieldable wieldable)
        {
            if (m_UseHandler != null)
                m_UseHandler.onUse -= OnWieldableUse;

            if (m_CrosshairHandler != null)
                m_CrosshairHandler.onCrosshairIndexChanged -= ChangeCrosshair;

            if (wieldable != null)
            {
                m_UseHandler = wieldable as IUseHandler;
                m_CrosshairHandler = wieldable as ICrosshairHandler;

                if (m_CrosshairHandler != null)
                {
                    m_CrosshairHandler.onCrosshairIndexChanged += ChangeCrosshair;
                    ChangeCrosshair(m_CrosshairHandler.CrosshairIndex);

                    if (m_UseHandler != null)
                        m_UseHandler.onUse += OnWieldableUse;

                    return;
                }
            }
            else
            {
                m_UseHandler = null;
                m_CrosshairHandler = null;
            }

            ChangeCrosshair(0);
        }

        // Temporary hack to increase the crosshair size when the player uses the weapon
        private void OnWieldableUse()
        {
            if (m_CurrentCrosshair != null)
                m_CurrentCrosshair.UpdateSize(15f);
        }

        private void ChangeCrosshair(int crosshairIndex)
        {
            var crosshairToEnable = crosshairIndex < 0 ? null : m_Crosshairs[Mathf.Clamp(crosshairIndex, 0, m_Crosshairs.Count - 1)];

            if (m_CurrentCrosshair != null)
                m_CurrentCrosshair.Hide();

            m_CurrentCrosshair = crosshairToEnable;

            if (m_CurrentCrosshair != null)
                m_CurrentCrosshair.Show();
        }

        private void FixedUpdate()
        {
            if (!IsInitialized || m_CurrentCrosshair == null)
                return;

            bool canEnableCrosshair = CanEnableCrosshair();
            m_CanvasGroup.alpha = Mathf.Lerp(m_CanvasGroup.alpha, (canEnableCrosshair ? m_CrosshairAlpha : 0f), Time.deltaTime * m_AlphaLerpSpeed);

            float crosshairSize = m_CrosshairHandler != null ? m_CrosshairHandler.GetCrosshairAccuracy() : 1f;
            crosshairSize *= canEnableCrosshair ? crosshairSize : 0f;
            m_CurrentCrosshair.UpdateSize(crosshairSize);
        }

        private bool CanEnableCrosshair()
        {
            HoverInfo hoverInfo = m_InteractionHandler.HoverInfo;

            return hoverInfo == null || !hoverInfo.IsInteractable;
        }
    }
}