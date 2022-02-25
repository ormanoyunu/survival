using UnityEngine;

namespace SurvivalTemplatePro
{
    public class ArrayElementAttribute : PropertyAttribute
    {
        public readonly string Prefix, Suffix;
        public readonly int LeftIndent, RightIndent;


        public ArrayElementAttribute(string prefix, string suffix, int leftIndent, int rightIndent)
        {
            Prefix = prefix;
            Suffix = suffix;
            LeftIndent = leftIndent;
            RightIndent = rightIndent;
        }
    }
}