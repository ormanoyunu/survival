using UnityEngine;

namespace SurvivalTemplatePro.UISystem
{
    public class PlayerUIBehaviour : MonoBehaviour, IPlayerUIBehaviour
	{
        public ICharacter Player { get; private set; }
        public IInventory PlayerInventory => Player.Inventory;

        protected bool IsInitialized { get; private set; }


        /// <summary>
        /// Initialize this UI Behaviour.
        /// </summary>
        public void InitBehaviour(ICharacter player)
        {
            Player = player;
            IsInitialized = true;
        }

        /// <summary>
        /// Gets called when the UI gets attached to a Player
        /// </summary>
        public virtual void OnAttachment() { }

        /// <summary>
        /// Gets called when the UI gets detached from the Player
        /// </summary>
        public virtual void OnDetachment() { }

        /// <summary>
        /// Gets called when the UI gets updated
        /// </summary>
        public virtual void OnInterfaceUpdate() { }

        /// <summary>
        /// <para> Returns module of specified type from the attached Player. </para>
        /// Use this if you are NOT sure the attached Player has this module.
        /// </summary>
        protected bool TryGetModule<T>(out T module) where T : ICharacterModule
        {
            return Player.TryGetModule(out module);
        }

        /// <summary>
        /// <para> Returns module of specified type from the attached Player. </para>
        /// Use this if you are sure the attached Player has this module.
        /// </summary>
        protected void GetModule<T>(out T module) where T : ICharacterModule
        {
            Player.GetModule(out module);
        }

        /// <summary>
        /// <para> Returns module of specified type from the attached Player. </para>
		/// Use this if you are sure the attached Player has this module.
        /// </summary>
        protected T GetModule<T>() where T : ICharacterModule
        {
            return Player.GetModule<T>();
        }
    }
}
