using UnityEngine;

namespace SurvivalTemplatePro.WieldableSystem
{
    public abstract class FirearmAttachmentBehaviour : MonoBehaviour, IFirearmAttachment
    {
        #region Internal
        public enum EnableMode
        {
            Component,
            Gameobject
        }
        #endregion

        public IFirearm Firearm { get; private set; }
        public IWieldableEffectsManager EventManager { get; private set; }

        public bool AttachOnStart
        {
            get => m_AttachOnStart;
            set => m_AttachOnStart = value;

        }

        [SerializeField]
        private EnableMode m_EnableMode;

        [SerializeField, HideInInspector]
        private bool m_AttachOnStart;


        public void Attach()
        {
            if (m_EnableMode == EnableMode.Gameobject)
                gameObject.SetActive(true);
            else
                enabled = true;
        }

        public void Detach()
        {
            if (m_EnableMode == EnableMode.Gameobject)
                gameObject.SetActive(false);
            else
                enabled = false;
        }

        protected virtual void Awake()
        {
            Firearm = GetComponentInParent<IFirearm>();
            EventManager = Firearm.EventManager;

            if (m_AttachOnStart)
                Attach();
            else
                Detach();
        }
    }
}