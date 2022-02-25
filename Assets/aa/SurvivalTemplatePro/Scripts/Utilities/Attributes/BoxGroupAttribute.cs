using UnityEngine;

namespace SurvivalTemplatePro
{
    public class BoxGroupAttribute : PropertyAttribute
    {
        public string GroupName { get; private set; }


        public BoxGroupAttribute(string groupName)
        {

        }
    }
}