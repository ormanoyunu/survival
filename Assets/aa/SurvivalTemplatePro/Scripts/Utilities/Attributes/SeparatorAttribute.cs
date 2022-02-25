using UnityEngine;

namespace SurvivalTemplatePro
{
    public class SeparatorAttribute : PropertyAttribute
    {
        public readonly float Height;


        public SeparatorAttribute(float height = 1.5f)
        {
            Height = height;
        }
    }
}