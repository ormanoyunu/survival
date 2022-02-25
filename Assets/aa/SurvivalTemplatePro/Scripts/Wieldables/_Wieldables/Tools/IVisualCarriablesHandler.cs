namespace SurvivalTemplatePro.WieldableSystem
{
    public interface IVisualCarriablesHandler
    {
        IWieldable AttachedWieldable { get; set; }

        void UpdateVisuals(int carriedCount);
    }
}