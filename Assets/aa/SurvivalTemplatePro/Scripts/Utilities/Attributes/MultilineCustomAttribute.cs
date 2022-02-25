using UnityEngine;

namespace SurvivalTemplatePro
{
    public class MultilineCustomAttribute : PropertyAttribute
    {
        public readonly int Lines;


        public MultilineCustomAttribute(int lines = 3)
        {
            Lines = lines;
        }
    }
}