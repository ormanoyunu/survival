using UnityEngine;

namespace SurvivalTemplatePro.WieldableSystem
{
    [CreateAssetMenu(menuName = "Survival Template Pro/Wieldables/FP Wieldable Animator Preset")]
    public class FPWieldableAnimatorPreset : ScriptableObject
    {
        public RuntimeAnimatorController AnimatorController => m_Controller;
        public FPWieldableAnimator.AnimatorParamaterEffect[] StatePresets => m_StatePresets;


        [SerializeField]
        private RuntimeAnimatorController m_Controller;

        [Space]

        [SerializeField]
        private FPWieldableAnimator.AnimatorParamaterEffect[] m_StatePresets;
    }
}