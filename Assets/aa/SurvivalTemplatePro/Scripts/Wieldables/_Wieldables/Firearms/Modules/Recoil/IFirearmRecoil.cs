namespace SurvivalTemplatePro.WieldableSystem
{
    public interface IFirearmRecoil : IFirearmAttachment
    {
        float RecoilForce { get; }

        void DoRecoil(float value);
    }
}