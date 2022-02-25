using UnityEngine;

namespace SurvivalTemplatePro
{
    /// <summary>
    /// Useful for getting access to callbacks and modules of the Parent Character.
    /// </summary>
    public abstract class CharacterBehaviour : MonoBehaviour, ICharacterBehaviour
    {
        protected ICharacter Character
        {
            get
            {
                if (m_Character == null)
                    m_Character = GetComponentInParent<ICharacter>();

                return m_Character;
            }
        }

        protected bool IsInitialized { get; private set; }

        private ICharacter m_Character;


        /// <summary>
        /// Initialize this module.
        /// </summary>
        public void InititalizeBehaviour(ICharacter character)
        {
            m_Character = character;
            IsInitialized = true;

            OnInitialized();
        } 

        /// <summary>
        /// Gets called after the parent character has been initialized (acts similarly to the Monobehaviour Start Callback).
        /// </summary>
        public virtual void OnInitialized() { }

        /// <summary>
        /// <para> Returns module of specified type from the parent character. </para>
		/// Use this if you are NOT sure the parent character has this module.
        /// </summary>
        protected bool TryGetModule<T>(out T module) where T : ICharacterModule
        {
            return m_Character.TryGetModule(out module);
        }

        /// <summary>
        /// <para> Returns module of specified type from the parent character. </para>
		/// Use this if you ARE sure the parent character has this module.
        /// </summary>
        protected void GetModule<T>(out T module) where T : ICharacterModule
        {
            m_Character.GetModule(out module);
        }

        /// <summary>
        /// <para> Returns module of specified type from the parent character. </para>
		/// Use this if you ARE sure the parent character has this module.
        /// </summary>
        protected T GetModule<T>() where T : ICharacterModule
        {
            return m_Character.GetModule<T>();
        }
    }
}