using UnityEditor;

namespace SurvivalTemplatePro
{
    [CustomPropertyDrawer(typeof(ItemCategoryReference))]
    public class ItemCategoryReferenceDrawer : PopupStringElementsDrawer
    {
        private static string[] m_AllNames;


        protected override string[] GetElementNames()
        {
            if (m_AllNames == null)
                m_AllNames = ItemDatabase.GetCategoryNames().ToArray();

            return m_AllNames;
        }

        protected override string[] GetElementNamesFullPath()
        {
            if (m_AllNames == null)
                m_AllNames = ItemDatabase.GetCategoryNames().ToArray();

            return m_AllNames;
        }
    }
}