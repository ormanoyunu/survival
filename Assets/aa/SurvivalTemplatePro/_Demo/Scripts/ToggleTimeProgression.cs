using SurvivalTemplatePro.WorldManagement;
using UnityEngine;

namespace SurvivalTemplatePro.Demo
{
    public class ToggleTimeProgression : MonoBehaviour
    {
        public void ToggleWorldProgrss()
        {
            WorldManagerBase.Instance.TimeProgressionEnabled = !WorldManagerBase.Instance.TimeProgressionEnabled;
        }
    }
}