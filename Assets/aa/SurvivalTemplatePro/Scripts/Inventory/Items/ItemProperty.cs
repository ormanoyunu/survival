using System;
using UnityEngine;

namespace SurvivalTemplatePro.InventorySystem
{
    /// <summary>
    /// An item property can hold a float value which can also be manipulated as a bool and integer.
    /// </summary>
    [Serializable]
    public class ItemProperty
    {
        public string PropertyName => m_Name;
        public int PropertyId => m_Id;
        public ItemPropertyType PropertyType => m_Type;

        public bool Boolean
        {
            get => m_Value > 0f;
            set
            {
                if (m_Type == ItemPropertyType.Boolean)
                    SetInternalValue(value == true ? 1 : 0);
            }
        }

        public int Integer
        {
            get => (int)m_Value;
            set
            {
                if (m_Type == ItemPropertyType.Integer)
                    SetInternalValue(value);
            }
        }

        public float Float
        {
            get => m_Value;
            set
            {
                if (m_Type == ItemPropertyType.Float)
                    SetInternalValue(value);
            }
        }

        public int ItemId
        {
            get => (int)m_Value;
            set
            {
                if (m_Type == ItemPropertyType.ItemId)
                    SetInternalValue(Mathf.Clamp(value, -9999999, 9999999));
            }
        }

        public event PropertyChangedDelegate onChanged;

        [SerializeField]
        private string m_Name;

        [SerializeField]
        private int m_Id;

        [SerializeField]
        private ItemPropertyType m_Type;

        [SerializeField]
        private float m_Value;


        public ItemProperty(ItemPropertyInfo propertyInfo)
        {
            m_Name = propertyInfo.Name;
            m_Id = propertyInfo.Id;
            m_Type = propertyInfo.Type;

            m_Value = propertyInfo.GetAsFloat();
        }

        public ItemProperty GetMemberwiseClone() => (ItemProperty)MemberwiseClone();

        private void SetInternalValue(float value)
        {
            float oldValue = m_Value;
            m_Value = value;

            if (oldValue != m_Value)
                onChanged?.Invoke(this);
        }
    }

    public delegate void PropertyChangedDelegate(ItemProperty property);
}