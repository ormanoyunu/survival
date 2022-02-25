using UnityEditor;
using UnityEngine;

namespace SurvivalTemplatePro
{
    [CustomPropertyDrawer(typeof(SeparatorAttribute))]
    public class SeparatorDrawer : DecoratorDrawer
    {
        public override void OnGUI(Rect position)
        {
            float height = ((SeparatorAttribute)attribute).Height;

            var indentedRect = EditorGUI.IndentedRect(new Rect(position.x, position.y -2 + (position.height / 2), position.width, height));
            STPEditorGUI.Separator(indentedRect);
        }

        public override float GetHeight() => base.GetHeight() + ((SeparatorAttribute)attribute).Height;
    }
}