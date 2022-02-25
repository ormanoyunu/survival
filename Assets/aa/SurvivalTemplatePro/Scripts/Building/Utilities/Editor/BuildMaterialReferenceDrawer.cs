using UnityEditor;
namespace SurvivalTemplatePro
{
    [CustomPropertyDrawer(typeof(BuildMaterialReference))]
    public class BuildMaterialReferenceDrawer : PopupIdElementsDrawer
    {
        private static string[] m_AllNames;


        protected override string[] GetElementNames()
        {
            if (m_AllNames == null)
                m_AllNames = BuildMaterialsDatabase.GetBuildingMaterialNames();

            return m_AllNames;
        }

        protected override string[] GetElementNamesFullPath()
        {
            if (m_AllNames == null)
                m_AllNames = BuildMaterialsDatabase.GetBuildingMaterialNames();

            return m_AllNames;
        }

        protected override int IdOfElement(int index) => BuildMaterialsDatabase.GetBuildingMaterialAtIndex(index).Id;
        protected override int IndexOfElement(int id) => BuildMaterialsDatabase.GetIndexOfBuildingMaterial(id);
    }
}