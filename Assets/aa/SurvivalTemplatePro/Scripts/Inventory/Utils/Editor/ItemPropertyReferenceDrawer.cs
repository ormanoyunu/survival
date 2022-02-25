using UnityEditor;

namespace SurvivalTemplatePro
{
    [CustomPropertyDrawer(typeof(ItemPropertyReference))]
    public class ItemPropertyReferenceDrawer : PopupIdElementsDrawer
    {
        private static string[] m_AllItems;
        private static string[] m_AllItemsFullPath;


        protected override string[] GetElementNames()
        {
            if (m_AllItems == null)
                m_AllItems = ItemDatabase.GetPropertyNames();

            return m_AllItems;
        }

        protected override string[] GetElementNamesFullPath()
        {
            if (m_AllItemsFullPath == null)
                m_AllItemsFullPath = ItemDatabase.GetPropertyNames();

            return m_AllItemsFullPath;
        }

        protected override int IdOfElement(int index) => ItemDatabase.GetPropertyAtIndex(index).Id;
        protected override int IndexOfElement(int id) => ItemDatabase.GetIndexOfProperty(id);
    }
}