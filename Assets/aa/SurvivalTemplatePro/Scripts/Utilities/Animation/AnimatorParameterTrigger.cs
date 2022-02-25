using UnityEngine;

namespace SurvivalTemplatePro
{
    [System.Serializable]   
    public class AnimatorParameterTrigger
    {
        public AnimatorControllerParameterType ParameterType => m_Type;

        [SerializeField]
        private AnimatorControllerParameterType m_Type;

        [SerializeField]
        private string m_Name;

        [SerializeField]
        private float m_Value;


        public AnimatorParameterTrigger(AnimatorControllerParameterType type, string name, float value)
        {
            this.m_Type = type;
            this.m_Name = name;
            this.m_Value = value;
        }

        public void TriggerParameter(Animator animator)
        {
            switch (m_Type)
            {
                case AnimatorControllerParameterType.Float:
                    animator.SetFloat(m_Name, m_Value);
                    break;
                case AnimatorControllerParameterType.Int:
                    animator.SetInteger(m_Name, (int)m_Value);
                    break;
                case AnimatorControllerParameterType.Bool:
                    animator.SetBool(m_Name, m_Value > 0f);
                    break;
                case AnimatorControllerParameterType.Trigger:
                    animator.SetTrigger(m_Name);
                    break;
            }
        }

        public void TriggerParameter(Animator animator, float valueMod)
        {
            float value = m_Value * valueMod;

            switch (m_Type)
            {
                case AnimatorControllerParameterType.Float:
                    animator.SetFloat(m_Name, value);
                    break;
                case AnimatorControllerParameterType.Int:
                    animator.SetInteger(m_Name, (int)value);
                    break;
                case AnimatorControllerParameterType.Bool:
                    animator.SetBool(m_Name, value > 0f);
                    break;
                case AnimatorControllerParameterType.Trigger:
                    animator.SetTrigger(m_Name);
                    break;
            }
        }
    }
}