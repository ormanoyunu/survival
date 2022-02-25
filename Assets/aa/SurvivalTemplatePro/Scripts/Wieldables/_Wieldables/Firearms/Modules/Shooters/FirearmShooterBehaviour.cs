namespace SurvivalTemplatePro.WieldableSystem
{
    public abstract class FirearmShooterBehaviour : FirearmAttachmentBehaviour, IFirearmShooter
    {
        public virtual int AmmoPerShot => 1;


        public abstract void Shoot(float value);

        protected virtual void OnEnable() => Firearm.SetShooter(this);
    }
}
