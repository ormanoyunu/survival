using UnityEditor;

namespace SurvivalTemplatePro
{
    [CustomPropertyDrawer(typeof(PlaceableReference))]
    public class PlaceableReferenceDrawer : PopupIdElementsDrawer
    {
        private static string[] m_AllNames;
        private static string[] m_AllNamesFullPath;


        protected override string[] GetElementNames()
        {
            if (m_AllNames == null)
                m_AllNames = PlaceableDatabase.GetPlaceableNames().ToArray();

            return m_AllNames;
        }

        protected override string[] GetElementNamesFullPath()
        {
            if (m_AllNamesFullPath == null)
                m_AllNamesFullPath = PlaceableDatabase.GetPlaceableNamesFull().ToArray();

            return m_AllNamesFullPath;
        }

        protected override int IdOfElement(int index) => PlaceableDatabase.GetPlaceableAtIndex(index).PlaceableID;
        protected override int IndexOfElement(int id) => PlaceableDatabase.GetIndexOfPlaceable(id);
    }
}
