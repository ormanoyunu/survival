namespace SurvivalTemplatePro.WieldableSystem
{
    public interface IFirearmAimer : IFirearmAttachment
    {
        bool IsAiming { get; }

        float HipShootSpread { get; }
        float AimShootSpread { get; }

        bool TryStartAim();
        bool TryEndAim();
    }
}