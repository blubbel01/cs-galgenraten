using System;
using System.Collections.Generic;

namespace Playground.Manager.Inventory.Meta
{
    public class ItemMeta : IEquatable<ItemMeta>, ICloneable
    {
        private string _displayName;
        private string _lore;
        private short _damage;
        private List<ItemFlags> _flagsList;

        public ItemMeta()
        {
            _displayName = null;
            _lore = null;
            _damage = 0;
            _flagsList = new List<ItemFlags>();
        }

        public ItemMeta(string displayName, string lore, short damage, List<ItemFlags> flagsList)
        {
            _displayName = displayName;
            _lore = lore;
            _damage = damage;
            _flagsList = flagsList;
        }

        public List<ItemFlags> FlagsList => _flagsList;

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
            return new ItemMeta(_displayName, _lore, _damage, flagsList);
        }
    }
}