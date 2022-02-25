using UnityEngine;

namespace SurvivalTemplatePro.WieldableSystem
{
    public interface IRayGenerator
    {
        Ray GenerateRay(float raySpreadMod, Vector3 localOffset = default);
        float GetRaySpread();
    }
}