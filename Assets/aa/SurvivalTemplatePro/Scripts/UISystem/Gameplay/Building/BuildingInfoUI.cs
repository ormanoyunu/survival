using SurvivalTemplatePro.BuildingSystem;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace SurvivalTemplatePro.UISystem
{
    public class BuildingInfoUI : PlayerUIBehaviour
    {
        #region Internal
        [Serializable]
        public class PlaceableChangedEvent : UnityEvent<Placeable> { }
        #endregion

        [SerializeField]
        private UnityEvent m_OnCustomBuildingStarted;

        [SerializeField]
        private UnityEvent m_OnPremadeBuildingStarted;

        [SerializeField]
        private UnityEvent m_OnBuildingEnded;

        [SerializeField]
        private PlaceableChangedEvent m_OnPlaceableChanged;

        [SerializeField]
        private UnityEvent m_OnObjectPlace;

        private IBuildingController m_BuildingController;


        public override void OnAttachment()
        {
            GetModule(out m_BuildingController);

            m_BuildingController.onBuildingStart += OnBuildingModeUpdate;
            m_BuildingController.onBuildingEnd += OnBuildingEnd;
            m_BuildingController.onObjectChanged += OnObjectChanged;
            m_BuildingController.onObjectPlaced += OnObjectPlace;
        }

        public override void OnDetachment()
        {
            if (m_BuildingController != null)
            {
                m_BuildingController.onBuildingStart -= OnBuildingModeUpdate;
                m_BuildingController.onBuildingEnd -= OnBuildingEnd;
                m_BuildingController.onObjectChanged -= OnObjectChanged;
                m_BuildingController.onObjectPlaced -= OnObjectPlace;
            }
        }

        private void OnBuildingModeUpdate()
        {
            if (m_BuildingController.BuildingMode == BuildableType.SocketBased)
                m_OnCustomBuildingStarted?.Invoke();
            else
                m_OnPremadeBuildingStarted?.Invoke();
        }

        private void OnBuildingEnd() => m_OnBuildingEnded?.Invoke();

        private void OnObjectChanged(Placeable placeable) => m_OnPlaceableChanged?.Invoke(placeable);
        private void OnObjectPlace() => m_OnObjectPlace?.Invoke();
    }
}