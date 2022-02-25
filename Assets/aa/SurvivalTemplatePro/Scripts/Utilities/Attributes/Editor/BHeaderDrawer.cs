using UnityEditor;
using UnityEngine;

namespace SurvivalTemplatePro
{
    [CustomPropertyDrawer(typeof(BHeaderAttribute))]
    public class BHeaderDrawer : DecoratorDrawer
    {
        private const float HEADER_SPACING = 6f;


        public override void OnGUI(Rect position)
        {
            var attr = (BHeaderAttribute)attribute;

            Vector2 textSize = EditorStyles.boldLabel.CalcSize(new GUIContent(attr.Name));
            position = new Rect(position.x - 3f, (position.y + position.height * 0.5f) - 5f, position.width + 3f, textSize.y);

            EditorGUI.DrawRect(position, new Color(0.4f, 0.4f, 0.4f, 0.085f));

            Color prevColor = GUI.color;
            GUI.color = new Color(1f, 1f, 1f, 0.75f);
            EditorGUI.LabelField(position, attr.Name, STPEditorGUI.SmallTitleLabel);
            GUI.color = prevColor;
        }

        public override float GetHeight() => base.GetHeight() + HEADER_SPACING;
    }
}