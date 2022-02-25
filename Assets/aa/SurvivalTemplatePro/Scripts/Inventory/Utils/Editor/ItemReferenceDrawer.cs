using UnityEditor;

namespace SurvivalTemplatePro
{
    [CustomPropertyDrawer(typeof(ItemReference))]
    public class ItemReferenceDrawer : PopupIdElementsDrawer
    {
        private static string[] m_AllNames;
        private static string[] m_AllNamesFullPath;


        protected override string[] GetElementNames()
        {
            if (m_AllNames == null)
                m_AllNames = ItemDatabase.GetItemNames().ToArray();

            return m_AllNames;
        }

        protected override string[] GetElementNamesFullPath()
        {
            if (m_AllNamesFullPath == null)
                m_AllNamesFullPath = ItemDatabase.GetItemNamesFullPath().ToArray();
            
            return m_AllNamesFullPath;
        }

        protected override int IdOfElement(int index) => ItemDatabase.GetItemAtIndex(index).Id;
        protected override int IndexOfElement(int id) => ItemDatabase.GetIndexOfItem(id);
    }
}