using SurvivalTemplatePro.InventorySystem;

namespace SurvivalTemplatePro.WieldableSystem
{
    public static class AttachmentsSystemHelper
    {
        public static void AttachConfigurationWithID(this AttachmentsConfiguration[] configurations, int id)
        {
            if (id == ItemDatabase.NullItem.Id)
                return;

            for (int i = 0; i < configurations.Length; i++)
            {
                if (configurations[i].CorrespondingItem == id)
                    configurations[i].Attach();
            }
        }

        public static void AttachCorrepondingAttachment(this AttachmentsConfiguration[] configurations, Item item, ItemPropertyReference propertyName, PropertyChangedDelegate propertyChangedCallback)
        {
            if (item.TryGetProperty(propertyName, out var property))
            {
                configurations.AttachConfigurationWithID(property.ItemId);
                property.onChanged += propertyChangedCallback;
            }
        }

        public static void RemovePropertyCallback(Item item, ItemPropertyReference propertyName, PropertyChangedDelegate propertyChangedCallback)
        {
            if (item.TryGetProperty(propertyName, out var property))
                property.onChanged -= propertyChangedCallback;
        }

        public static void AttachCorrepondingAttachment(this AttachmentsConfiguration[] configurations, ItemProperty property)
        {
            configurations.AttachConfigurationWithID(property.ItemId);
        }
    }
}