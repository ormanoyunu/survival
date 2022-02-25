using UnityEngine;

namespace SurvivalTemplatePro
{
    public class BHeaderAttribute : PropertyAttribute
    {
        public string Name { get; private set; }


        public BHeaderAttribute(string name)
        {
            Name = name;
        }
    }
}