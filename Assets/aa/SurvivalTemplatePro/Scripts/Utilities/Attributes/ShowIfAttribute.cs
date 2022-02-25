using System;
using UnityEngine;

namespace SurvivalTemplatePro
{
    public class ShowIfAttribute : PropertyAttribute
    {
        public readonly string m_ConditionProperty;

        public readonly bool m_RequiredBool = false;
		public readonly int m_RequiredInt = -1;
		public readonly float m_RequiredFloat = -1f;
        public readonly string m_RequiredString = "s";
        public readonly Vector3 m_RequiredVector3 = new Vector3(1, 1, 1);


		public ShowIfAttribute(string condition, bool requiredValue)
        {
            m_ConditionProperty = condition;
            m_RequiredBool = requiredValue;
        }

		public ShowIfAttribute(string condition, int requiredValue)
		{
			m_ConditionProperty = condition;
			m_RequiredInt = requiredValue;
        }

		public ShowIfAttribute(string condition, float requiredValue)
		{
			m_ConditionProperty = condition;
			m_RequiredFloat = requiredValue;
        }
			
        public ShowIfAttribute(string condition, string requiredValue)
        {
            m_ConditionProperty = condition;
            m_RequiredString = requiredValue;
        }
			
		public ShowIfAttribute(string condition, Vector3 requiredValue)
        {
            m_ConditionProperty = condition;
            m_RequiredVector3 = requiredValue;
        }
    }
}