using UnityEngine;

namespace SurvivalTemplatePro.WieldableSystem
{
    public class FirearmBasicAimer : FirearmAimerBehaviour
    {
        [BHeader("Aim")]

        [SerializeField, Range(0f, 5f)]
        private float m_AimThreshold = 0.3f;

        [BHeader("Audio")]

        [SerializeField]
        private StandardSound m_AimAudio;

        [SerializeField]
        private StandardSound m_AimEndAudio;

        [BHeader("Crosshair")]

        [SerializeField, Range(-1, 100)]
        private int m_AimCrosshairIndex = 0;

        [BHeader("Local Effects")]

        [SerializeField, WieldableEffect]
        private int[] m_OnAimEffects;

        [SerializeField, WieldableEffect]
        private int[] m_OnAimEndEffects;

        [SerializeField, WieldableEffect(WieldableEffectPlayType.StopEffect)]
        private int[] m_AimEndEffectsToStop;

        private ICrosshairHandler m_CrosshairHandler;
        private float m_NextPossibleAimTime;


        public override bool TryStartAim()
        {
            if (IsAiming || Time.time < m_NextPossibleAimTime)
                return false;

            // Crosshair
            m_CrosshairHandler.CrosshairIndex = m_AimCrosshairIndex;

            // Audio
            Firearm.AudioPlayer.PlaySound(m_AimAudio);

            // Local Effects
            EventManager.PlayEffects(m_OnAimEffects, 1f);

            IsAiming = true;

            return true;
        }

        public override bool TryEndAim()
        {
            if (!IsAiming)
                return false;

            m_NextPossibleAimTime = Time.time + m_AimThreshold;

            // Crosshair
            m_CrosshairHandler.ResetCrosshair();

            // Audio
            Firearm.AudioPlayer.PlaySound(m_AimEndAudio);

            // Local Effects
            EventManager.PlayEffects(m_OnAimEndEffects, 1f);
            EventManager.StopEffects(m_AimEndEffectsToStop);

            IsAiming = false;

            return true;
        }

        protected override void Awake()
        {
            base.Awake();

            m_CrosshairHandler = Firearm as ICrosshairHandler;
        }
    }
}
