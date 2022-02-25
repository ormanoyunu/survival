using SurvivalTemplatePro.InventorySystem;
using UnityEngine;

namespace SurvivalTemplatePro.WieldableSystem
{
    public class FirearmAttachmentsHandler : MonoBehaviour
    {
        //[SerializeField, DatabaseProperty]
        //private int m_AttachmentProperty;

        //[Space]

        //[SerializeField, Reorderable]
        //private FirearmAttachmentsConfigurationList m_Configurations;

        //private IFirearm m_Firearm;


        //private void OnEnable() 
        //{
        //    //m_Firearm.onEquippingStarted += OnEquip;
        //    //m_Firearm.onHolsteringStarted += OnHolster;
        //}

        ///// <summary>
        ///// Check for every active attachment on the item and enable them
        ///// </summary>
        //private void OnEquip()
        //{
        //    m_Firearm.AttachedItem.onPropertyChanged += UpdateAttachments;
        //    UpdateAttachments(null);
        //}

        ///// <summary>
        ///// Update the state of every attachment
        ///// </summary>
        //private void UpdateAttachments(ItemProperty property) 
        //{
        //    var attachedItem = m_Firearm.AttachedItem;
        //    var foundAttachments = attachedItem.GetAllPropertiesWithId(m_AttachmentProperty);

        //    for (int i = 0; i < foundAttachments.Length; i++)
        //    {
        //        for (int j = 0; j < m_Configurations.Length; j++)
        //        {
        //            if (m_Configurations[j].CorrespondingItem == attachedItem.Info.Id)
        //            {
        //                m_Configurations[j].Attach();
        //                break;
        //            }
        //        }
        //    }
        //}

        ///// <summary>
        ///// Disable every previously enabled attachments
        ///// </summary>
        //private void OnHolster()
        //{
        //    for (int i = 0; i < m_Configurations.Length; i++)
        //        m_Configurations[i].Detach();
        //}
    }
}