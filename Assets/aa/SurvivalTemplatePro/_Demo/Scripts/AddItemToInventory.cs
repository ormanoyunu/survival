using SurvivalTemplatePro.InventorySystem;
using UnityEngine;

namespace SurvivalTemplatePro.Demo
{
    public class AddItemToInventory : MonoBehaviour
    {
        [SerializeField]
        private ItemCategoryReference[] m_Categories;


        public void AddItemToCharacter(ICharacter character) 
        {
            var category = m_Categories.SelectRandom().GetItemCategory();
            Item itemToAdd = new Item(category.Items.SelectRandom());
            character.Inventory.AddItem(itemToAdd);
        }
    }
}