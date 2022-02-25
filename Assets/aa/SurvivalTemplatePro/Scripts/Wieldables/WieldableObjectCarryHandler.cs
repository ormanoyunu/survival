using SurvivalTemplatePro.BuildingSystem;
using System.Collections.Generic;

namespace SurvivalTemplatePro.WieldableSystem
{
    public class WieldableObjectCarryHandler : CharacterBehaviour
    {
        private Dictionary<CarriableDefinition, IVisualCarriablesHandler> m_Carriables;

        private IObjectCarryController m_ObjectCarry;
        private IWieldablesController m_WieldablesController;
        private IWieldableSelectionHandler m_SelectionHandler;

        private IVisualCarriablesHandler m_CurrentVisualHandler;


        public override void OnInitialized()
        {
            GetModule(out m_ObjectCarry);
            GetModule(out m_WieldablesController);
            GetModule(out m_SelectionHandler);

            m_Carriables = new Dictionary<CarriableDefinition, IVisualCarriablesHandler>();

            m_ObjectCarry.onObjectCarryStart += OnObjectCarryStart;
            m_ObjectCarry.onObjectCarryEnd += OnObjectCarryEnd;
            m_ObjectCarry.onCarriedCountChanged += UpdateVisuals;
        }

        private void UpdateVisuals(int carriedCount)
        {
            if (m_CurrentVisualHandler != null)
                m_CurrentVisualHandler.UpdateVisuals(carriedCount);
        } 

        private void OnObjectCarryStart()
        {
            var carriedObject = m_ObjectCarry.CarriedObject;

            if (carriedObject != null)
            {
                if (!m_Carriables.ContainsKey(m_ObjectCarry.CarriedObject))
                    AddCarriableToDictionary(m_ObjectCarry.CarriedObject);

                if (m_Carriables.TryGetValue(carriedObject, out var handler))
                {
                    m_CurrentVisualHandler = handler;
                    m_WieldablesController.TryEquipWieldable(handler.AttachedWieldable, null, 1.3f);

                    m_WieldablesController.onWieldableEquipped += OnWieldableChanged;
                }
            }
        }

        private void AddCarriableToDictionary(CarriableDefinition carriableDefinition)
        {
            var spawnedWieldable = m_WieldablesController.SpawnWieldable(carriableDefinition.TargetWieldable);
            var visualHandler = spawnedWieldable.GetComponentsInChildren<IVisualCarriablesHandler>(true)[0];
            visualHandler.AttachedWieldable = spawnedWieldable;

            m_Carriables.Add(carriableDefinition, visualHandler);
        }

        private void OnObjectCarryEnd()
        {
            m_SelectionHandler.Refresh();
            m_WieldablesController.onWieldableEquipped -= OnWieldableChanged;
            m_CurrentVisualHandler = null;
        }

        private void OnWieldableChanged(IWieldable wieldable)
        {
            if (m_CurrentVisualHandler == null || m_CurrentVisualHandler.AttachedWieldable == null)
                return;

            if (m_WieldablesController.ActiveWieldable != m_CurrentVisualHandler.AttachedWieldable)
                m_ObjectCarry.DropCarriedObjects(m_ObjectCarry.CarriedObjectsCount);
        }
    }
}