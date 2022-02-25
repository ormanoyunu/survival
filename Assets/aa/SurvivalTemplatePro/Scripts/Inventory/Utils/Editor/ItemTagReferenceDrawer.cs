using UnityEditor;

namespace SurvivalTemplatePro
{
    [CustomPropertyDrawer(typeof(ItemTagReference))]
    public class ItemTagReferenceDrawer : PopupStringElementsDrawer
    {
        private static string[] m_AllNames;


        protected override string[] GetElementNames()
        {
            if (m_AllNames == null)
                m_AllNames = ItemDatabase.GetAllTags().ToArray();

            return m_AllNames;
        }

        protected override string[] GetElementNamesFullPath()
        {
            if (m_AllNames == null)
                m_AllNames = ItemDatabase.GetAllTags().ToArray();

            return m_AllNames;
        }
    }
}