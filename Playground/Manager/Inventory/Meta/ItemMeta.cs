using System;
using System.Collections.Generic;
using Playground.Manager.Inventory.Meta.Attributes;
using Attribute = Playground.Manager.Inventory.Meta.Attributes.Attribute;

namespace Playground.Manager.Inventory.Meta
{
    public class ItemMeta : IEquatable<ItemMeta>, ICloneable
    {
        private string _displayName;
        private string _lore;
        private short _damage;
        private List<ItemFlags> _flagsList;
        private List<AttributeModifier> _attributeModifiers;

        public ItemMeta()
        {
            _displayName = null;
            _lore = null;
            _damage = 0;
            _flagsList = new List<ItemFlags>();
            _attributeModifiers = new List<AttributeModifier>();
        }

        public ItemMeta(string displayName, string lore, short damage, List<ItemFlags> flagsList, List<AttributeModifier> attributeModifiers)
        {
            _displayName = displayName;
            _lore = lore;
            _damage = damage;
            _flagsList = flagsList;
            _attributeModifiers = attributeModifiers;
        }

        public List<ItemFlags> FlagsList => _flagsList;
        
        public List<AttributeModifier> AttributeModifiers => _attributeModifiers;

        public string DisplayName
        {
            get => _displayName;
            set => _displayName = value;
        }

        public string Lore
        {
            get => _lore;
            set => _lore = value;
        }

        public short Damage
        {
            get => _damage;
            set => _damage = value;
        }


        public bool Equals(ItemMeta other)
        {
            if (other == null)
                return false;
            if (!_displayName.Equals(other.DisplayName))
                return false;
            if (!_lore.Equals(other.Lore))
                return false;
            return true;
        }

        public object Clone()
        {
            List<ItemFlags> flagsList = new List<ItemFlags>();
            foreach (var itemFlag in FlagsList)
            {
                flagsList.Add(itemFlag);
            }
            return new ItemMeta(DisplayName, Lore, Damage, FlagsList, AttributeModifiers);
        }
    }
}