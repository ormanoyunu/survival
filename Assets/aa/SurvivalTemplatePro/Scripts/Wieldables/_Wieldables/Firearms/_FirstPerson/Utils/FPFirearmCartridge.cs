using UnityEngine;

namespace SurvivalTemplatePro.WieldableSystem
{
    public class FPFirearmCartridge : MonoBehaviour
    {
        [SerializeField]
        private Renderer m_RendererToDisable;

        [SerializeField]
        private Renderer m_RendererToEnable;


        public void ChangeState (bool enable)
        {
            if (m_RendererToDisable != null)
                m_RendererToDisable.enabled = enable;

            if (m_RendererToEnable != null)
                m_RendererToEnable.enabled = !enable;
        }
    }
}
