using UnityEngine;

namespace SurvivalTemplatePro
{
    [System.Serializable]
    public class FPRecoilForce
    {
        [SerializeField]
        private SpringForce m_PositionForce;

        [SerializeField]
        private SpringForce m_RotationForce;

        [Range(0, 1)]
        public float xRandomness;

        [Range(0, 1)]
        public float yRandomness;

        [Range(0, 1)]
        public float zRandomness;


        public SpringForce GetPositionForce() => new SpringForce(m_PositionForce.Force.Jitter(xRandomness, yRandomness, zRandomness), m_PositionForce.Distribution);
        public SpringForce GetPositionForce(float forceMod) => new SpringForce(m_PositionForce.Force.Jitter(xRandomness, yRandomness, zRandomness) * forceMod, m_PositionForce.Distribution);

        public SpringForce GetRotationForce() => new SpringForce(m_RotationForce.Force.Jitter(xRandomness, yRandomness, zRandomness), m_RotationForce.Distribution);
        public SpringForce GetRotationForce(float forceMod) => new SpringForce(m_RotationForce.Force.Jitter(xRandomness, yRandomness, zRandomness) * forceMod, m_RotationForce.Distribution);
    }
}