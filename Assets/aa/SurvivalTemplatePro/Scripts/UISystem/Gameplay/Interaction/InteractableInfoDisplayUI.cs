using System;
using System.Collections.Generic;

namespace SurvivalTemplatePro.UISystem
{
    #region Internal
    public interface IInteractableInfoDisplayer
    {
        Type InteractableType { get; }

        void ShowInfo(IInteractable interactableObject);
        void UpdateInfo(IInteractable interactableObject);
        void SetInteractionProgress(float interactProgress);
        void HideInfo();
    }
    #endregion

    public class InteractableInfoDisplayUI : PlayerUIBehaviour
    {
        private readonly Dictionary<Type, IInteractableInfoDisplayer> m_ObjectInfoDisplayers = new Dictionary<Type, IInteractableInfoDisplayer>();

        private IWieldableSurvivalBookHandler m_SurvivalBookHandler;
        private IHealthManager m_HealthManager;
        private IInventoryInspectManager m_InventoryInspector;
        private IInteractableInfoDisplayer m_ActiveInfoDisplayer;
        private IInteractionHandler m_InteractionHandler;


        public override void OnAttachment()
        {
            GetModule(out m_SurvivalBookHandler);
            GetModule(out m_InteractionHandler);
            GetModule(out m_InventoryInspector);
            GetModule(out m_HealthManager);

            IInteractableInfoDisplayer[] objInfoDisplayers = GetComponentsInChildren<IInteractableInfoDisplayer>(true);

            foreach (IInteractableInfoDisplayer infoDisplayer in objInfoDisplayers)
            {
                if (!m_ObjectInfoDisplayers.ContainsKey(infoDisplayer.InteractableType))
                    m_ObjectInfoDisplayers.Add(infoDisplayer.InteractableType, infoDisplayer);
            }

            m_InteractionHandler.onHoverInfoChanged += OnHoverInfoChanged;
            m_InteractionHandler.onInteractProgressChanged += SetInteractionProgress;

            m_HealthManager.onDeath += HideDisplayer;
            m_InventoryInspector.onInspectStarted += (InventoryInspectState state) => HideDisplayer();
            m_InventoryInspector.onInspectEnded += ShowDisplayer;
            m_SurvivalBookHandler.onInspectionStarted += HideDisplayer;
            m_SurvivalBookHandler.onInspectionEnded += ShowDisplayer;
        }

        public override void OnDetachment()
        {
            if (m_InteractionHandler != null)
                m_InteractionHandler.onHoverInfoChanged -= OnHoverInfoChanged;

            if (m_SurvivalBookHandler != null)
            {
                m_SurvivalBookHandler.onInspectionStarted -= HideDisplayer;
                m_SurvivalBookHandler.onInspectionEnded -= ShowDisplayer;
            }
        }

        private void OnHoverInfoChanged(HoverInfo hoverInfo)
        {
            if (m_InventoryInspector.InspectState != InventoryInspectState.None)
                return;

            if (hoverInfo != null && hoverInfo.IsInteractable)
            {
                IInteractableInfoDisplayer correspondingDisplayer;

                if (m_ObjectInfoDisplayers.TryGetValue(hoverInfo.Interactable.GetType(), out correspondingDisplayer) || m_ObjectInfoDisplayers.TryGetValue(typeof(IInteractable), out correspondingDisplayer))
                {
                    if (correspondingDisplayer != m_ActiveInfoDisplayer)
                    {
                        if (m_ActiveInfoDisplayer != null)
                            m_ActiveInfoDisplayer.HideInfo();

                        m_ActiveInfoDisplayer = correspondingDisplayer;

                        if (m_ActiveInfoDisplayer != null)
                        {
                            m_ActiveInfoDisplayer.UpdateInfo(hoverInfo.Interactable);
                            m_ActiveInfoDisplayer.ShowInfo(hoverInfo.Interactable);
                        }
                    }
                    else
                    {
                        if (m_ActiveInfoDisplayer != null)
                            m_ActiveInfoDisplayer.UpdateInfo(hoverInfo.Interactable);
                    }
                }
            }
            else
            {
                if (m_ActiveInfoDisplayer != null)
                    m_ActiveInfoDisplayer.HideInfo();

                m_ActiveInfoDisplayer = null;
            }
        }

        private void SetInteractionProgress(float progress)
        {
            if (m_ActiveInfoDisplayer != null)
                m_ActiveInfoDisplayer.SetInteractionProgress(progress);
        }

        private void HideDisplayer()
        {
            if (m_ActiveInfoDisplayer != null)
                m_ActiveInfoDisplayer.HideInfo();
        }

        private void ShowDisplayer()
        {
            if (m_ActiveInfoDisplayer != null && m_HealthManager.IsAlive && m_InteractionHandler.HoverInfo != null)
            {
                var interactable = m_InteractionHandler.HoverInfo.Interactable;

                m_ActiveInfoDisplayer.ShowInfo(interactable);
                m_ActiveInfoDisplayer.UpdateInfo(interactable);
            }
        }
    }
}