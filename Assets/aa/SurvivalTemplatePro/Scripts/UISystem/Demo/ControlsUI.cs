using UnityEngine;
using UnityEngine.UI;

namespace SurvivalTemplatePro.UISystem
{
    public class ControlsUI : PlayerUIBehaviour
    {
        #region Internal
        [System.Serializable]
        private class ControlsBundleUI
        {
            public string ControlsBundleName;
            public GameObject[] CorrespondingControls;


            public void EnableControlsUI()
            {
                foreach (var control in CorrespondingControls)
                    control.SetActive(true);
            }

            public void DisableControlsUI()
            {
                foreach (var control in CorrespondingControls)
                    control.SetActive(false);
            }
            
        }
        #endregion

        [SerializeField]
        private Text m_ControlsNameText;

        [Space]

        [SerializeField]
        private ControlsBundleUI m_GeneralControls;

        [SerializeField]
        private ControlsBundleUI m_InventoryControls;

        private ControlsBundleUI m_ActiveControlsBundle;


        public override void OnAttachment()
        {
            if (TryGetModule(out IInventoryInspectManager inventoryInspection))
            {
                inventoryInspection.onInspectStarted += EnableInventoryControls;
                inventoryInspection.onInspectEnded += DisableInventoryControls;
            }

            m_GeneralControls.DisableControlsUI();
            m_InventoryControls.DisableControlsUI();

            SetActiveControls(m_GeneralControls);
        }

        public override void OnDetachment()
        {
            if (TryGetModule(out IInventoryInspectManager inventoryInspection))
            {
                inventoryInspection.onInspectStarted -= EnableInventoryControls;
                inventoryInspection.onInspectEnded -= DisableInventoryControls;
            }
        }

        private void EnableInventoryControls(InventoryInspectState arg0) => SetActiveControls(m_InventoryControls);
        private void DisableInventoryControls() => SetActiveControls(m_GeneralControls);

        private void SetActiveControls(ControlsBundleUI controlsBundle) 
        {
            if (controlsBundle == m_ActiveControlsBundle)
                return;

            if (m_ActiveControlsBundle != null)
                m_ActiveControlsBundle.DisableControlsUI();

            m_ActiveControlsBundle = controlsBundle;
            m_ActiveControlsBundle.EnableControlsUI();

            if (m_ControlsNameText != null)
                m_ControlsNameText.text = m_ActiveControlsBundle.ControlsBundleName;
        }
    }
}