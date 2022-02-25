namespace SurvivalTemplatePro.WieldableSystem
{
    public interface IFirearmShooter : IFirearmAttachment
    {
        int AmmoPerShot { get; }

        void Shoot(float value);
    }
}