using UnityEngine;

namespace SurvivalTemplatePro.WieldableSystem
{
    public abstract class MeleeSwingBehaviour : MonoBehaviour, IMeleeSwing
    {
		public virtual float SwingDuration => 1f;
		public virtual float AttackEffort => 0.1f;

		protected IWieldable Wieldable { get; private set; }


		public abstract bool CanSwing();
		public abstract void DoSwing(ICharacter user);

		protected virtual void Awake()
		{
			Wieldable = GetComponent<IWieldable>();
		}

		protected void ConsumeItemDurability(float amount) 
		{
			// Lower the durability...
			if (Wieldable.ItemDurability != null)
				Wieldable.ItemDurability.Float = Mathf.Max(Wieldable.ItemDurability.Float - amount, 0f);
		}
    }
}