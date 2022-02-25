using UnityEngine;

namespace SurvivalTemplatePro.Demo
{
    public class InteractiveButton : Interactable
    {
        [BHeader("Settings (Interactive Button)")]

        [SerializeField, Range(0f, 10f)]
        private float m_PressThreshold = 0.3f;

        [SerializeField]
        private bool m_HasPressLimit;

        [SerializeField, EnableIf("m_HasPressLimit", true)]
        private int m_MaxPressTimes = 0;

        [SerializeField, AnimatorParameter(AnimatorControllerParameterType.Trigger)]
        private string m_PressButtonTrigger;

        [SerializeField, AnimatorParameter(AnimatorControllerParameterType.Trigger)]
        private string m_HideButtonTrigger;

        [Space]

        [SerializeField]
        private Animator m_Animator;

        [SerializeField]
        private SoundPlayer m_PressAudio;

        private int m_PressedTimes;
        private float m_NextTimeCanPress;


        public override void OnInteract(ICharacter character)
        {
            if (Time.time < m_NextTimeCanPress || (m_HasPressLimit && m_PressedTimes >= m_MaxPressTimes))
                return;

            m_NextTimeCanPress = Time.time + m_PressThreshold;
            m_PressedTimes++;

            m_Animator.SetTrigger(m_PressedTimes < m_MaxPressTimes || !m_HasPressLimit ? m_PressButtonTrigger : m_HideButtonTrigger);
            m_PressAudio.PlayAtPosition(transform.position);

            base.OnInteract(character);
        }
    }
}