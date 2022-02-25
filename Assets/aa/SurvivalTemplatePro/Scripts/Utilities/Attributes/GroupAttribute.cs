using UnityEngine;

namespace SurvivalTemplatePro
{
    public class GroupAttribute : PropertyAttribute
    {
        public string PrefixText { get; private set; } = "";
        public bool OnlyShowPrefixIfNotExpanded { get; private set; } = true;


        public GroupAttribute()
        {
            PrefixText = "";
            OnlyShowPrefixIfNotExpanded = false;
        }
    }
}