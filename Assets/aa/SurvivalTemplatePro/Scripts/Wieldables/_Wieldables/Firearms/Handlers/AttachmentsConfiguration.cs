using UnityEngine;

namespace SurvivalTemplatePro.WieldableSystem
{
    [System.Serializable]
    public class AttachmentsConfiguration
    {
        public int CorrespondingItem => m_Item;

        [SerializeField]
        private ItemReference m_Item;

        [SerializeField]
        private FirearmAttachmentBehaviour m_Attachment;


        public void Attach() => m_Attachment.Attach();
    }
}