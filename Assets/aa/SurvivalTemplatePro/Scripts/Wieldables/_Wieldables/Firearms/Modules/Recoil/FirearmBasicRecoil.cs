using UnityEngine;

namespace SurvivalTemplatePro.WieldableSystem
{
    public class FirearmBasicRecoil : FirearmRecoilBehaviour
    {
        public override float RecoilForce => m_RecoilForce;

        [Space]

        [SerializeField, Range(0f, 10f)]
        private float m_RecoilForce = 1f;

        [BHeader("Local Effects")]

        [SerializeField, WieldableEffect]
        private int[] m_RecoilEffects;


        public override void DoRecoil(float forceMod)
        {
            float recoilForce = forceMod * m_RecoilForce;
            EventManager.PlayEffects(m_RecoilEffects, recoilForce);
        }
    }
}