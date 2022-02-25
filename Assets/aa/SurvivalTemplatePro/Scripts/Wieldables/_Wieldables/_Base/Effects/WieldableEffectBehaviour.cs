using UnityEngine;

namespace SurvivalTemplatePro.WieldableSystem
{
    public abstract class WieldableEffectBehaviour : MonoBehaviour
    {
        public abstract void DoEffect(ICharacter character);
    }
}