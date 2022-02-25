namespace SurvivalTemplatePro.WieldableSystem
{
    public abstract class FirearmRecoilBehaviour : FirearmAttachmentBehaviour, IFirearmRecoil
    {
        public abstract float RecoilForce { get; }


        public abstract void DoRecoil(float value);

        protected virtual void OnEnable() => Firearm.SetRecoil(this);
        protected virtual void OnDisable() { }
    }
}