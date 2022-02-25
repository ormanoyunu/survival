using System;
using System.Collections.Generic;

namespace SurvivalTemplatePro.UISystem
{
    #region Internal
    public interface IObjectInspector
    {
        Type InspectableType { get; }

        void Inspect(IInteractable inspectableObject);
        void EndInspection();
    }
    #endregion

    public class ObjectInspectController : PlayerUIBehaviour
    {
        private Dictionary<Type, IObjectInspector> m_ObjectInspectors = new Dictionary<Type, IObjectInspector>();

        private IInteractionHandler m_InteractionHandler;
        private IInventoryInspectManager m_InventoryInspector;

        private IObjectInspector m_ActiveInspector;


        public override void OnAttachment()
        {
            Player.TryGetModule(out m_InteractionHandler);
            Player.TryGetModule(out m_InventoryInspector);

            m_InteractionHandler.onInteract += OnInteract;
            m_InventoryInspector.onInspectEnded += OnInventoryInspectionEnded;

            IObjectInspector[] objInspectors = GetComponentsInChildren<IObjectInspector>(true);

            foreach (IObjectInspector inspector in objInspectors)
            {
                if (!m_ObjectInspectors.ContainsKey(inspector.InspectableType))
                    m_ObjectInspectors.Add(inspector.InspectableType, inspector);
            }
        }

        private void OnInteract(IInteractable interactable)
        {
            if (interactable == null)
                return;

            if (m_ObjectInspectors.TryGetValue(interactable.GetType(), out IObjectInspector inspector))
            {
                inspector.Inspect(interactable);
                m_ActiveInspector = inspector;

                m_InventoryInspector.TryInspect(InventoryInspectState.External, interactable as IExternalContainer);
            }
        }

        private void OnInventoryInspectionEnded()
        {
            m_ActiveInspector?.EndInspection();
            m_ActiveInspector = null;
        }
    }
}