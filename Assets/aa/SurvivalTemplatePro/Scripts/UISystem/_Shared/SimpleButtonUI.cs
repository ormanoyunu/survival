using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SurvivalTemplatePro.UISystem
{
    public class SimpleButtonUI : Button
    {
        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);

            onClick?.Invoke();
        }
    }
}