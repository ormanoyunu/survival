using System;
using UnityEngine;

namespace SurvivalTemplatePro
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public class EnableIfAttribute : PropertyAttribute
    {
        public readonly string m_PropertyName;
        public readonly float m_Indentation = 0f;

        public readonly bool m_RequiredBool = false;
        public readonly int m_RequiredInt = -1;


        public EnableIfAttribute(string propertyName, bool requiredValue, float indentation = 0f)
        {
            m_PropertyName = propertyName;
            m_RequiredBool = requiredValue;

            m_Indentation = indentation;
        }

        public EnableIfAttribute(string propertyName, int requiredValue, float indentation = 0f)
        {
            m_PropertyName = propertyName;
            m_RequiredInt = requiredValue;

            m_Indentation = indentation;
        }
    }
}
