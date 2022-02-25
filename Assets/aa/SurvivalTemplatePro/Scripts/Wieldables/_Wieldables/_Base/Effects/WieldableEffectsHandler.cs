using UnityEngine;

namespace SurvivalTemplatePro.WieldableSystem
{
    public abstract class WieldableEffectsHandler : MonoBehaviour
    {
        protected IWieldable Wieldable { get; private set; }
        private ICharacter m_Character;


        /// <summary>
        /// 
        /// </summary>
        public abstract WieldableEffect[] GetAllEffects();

        public virtual void StopEffects() { }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void InitModule(ICharacter character) { }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void OnWieldableEquipped() 
        {
            if (m_Character != Wieldable.Character)
            {
                InitModule(Wieldable.Character);
                m_Character = Wieldable.Character;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void OnWieldableHolstered(float holsterSpeed) { }

        private void Awake()
        {
            Wieldable = GetComponentInParent<IWieldable>();

            Wieldable.onEquippingStarted += OnWieldableEquipped;
            Wieldable.onHolsteringStarted += OnWieldableHolstered;
        }
    }
}