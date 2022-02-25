using SurvivalTemplatePro.InventorySystem;
using UnityEngine;

namespace SurvivalTemplatePro.UISystem
{
    public abstract class ItemActionUI : PlayerUIBehaviour
    {
        public string ActionName => m_ActionName;

        [SerializeField]
        private string m_ActionName;

        [SerializeField]
        private string m_ActionVerb;

        [SerializeField, Range(0f, 15f)]
        private float m_Duration = 0f;

        [Space]

        [SerializeField]
        private SoundPlayer m_ActionStartSound;

        [SerializeField]
        private SoundPlayer m_ActionEndSound;

        [SerializeField]
        private SoundPlayer m_ActionCanceledSound;

        private ItemSlot m_CurrentSlot;


        public abstract bool IsViableForItem(ItemSlot itemSlot);
        public abstract bool CanPerformAction(ItemSlot itemSlot);
        protected abstract void PerformAction(ItemSlot itemSlot);
        protected virtual void CancelAction(ItemSlot itemSlot) { }

        protected virtual float GetDuration(ItemSlot itemSlot) => m_Duration;

        public void StartAction(ItemSlot itemSlot)
        {
            if (!CanPerformAction(itemSlot))
                return;

            m_CurrentSlot = itemSlot;
            m_ActionStartSound.Play2D(1f);

            if (GetDuration(m_CurrentSlot) > 0.01f && Player.TryGetModule(out ICustomActionManager customActionManager))
            {
                customActionManager.StartAction(new CustomActionParams(m_ActionName, m_ActionVerb + "...", m_Duration, true , StartPerformingAction, CancelPerformingAction));

                return;
            }

            StartPerformingAction();
        }

        private void StartPerformingAction()
        {
            m_ActionEndSound.Play2D(1f);
            PerformAction(m_CurrentSlot);
        }

        private void CancelPerformingAction() 
        {
            m_ActionCanceledSound.Play2D(1f);
            CancelAction(m_CurrentSlot);
        }

        private void OnValidate()
        {
            if (Application.isEditor && !Application.isPlaying)
                gameObject.name = this.GetType().Name;
        }
    }
}