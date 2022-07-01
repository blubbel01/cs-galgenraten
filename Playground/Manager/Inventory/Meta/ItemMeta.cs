using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Playground.Manager.Inventory.Meta.Attributes;

namespace Playground.Manager.Inventory.Meta
{
    [DataContract]
    public class ItemMeta : IEquatable<ItemMeta>, ICloneable
    {
        [DataMember(Name = "displayName")] private string _displayName;

        [DataMember(Name = "lore")] private string _lore;

        [DataMember(Name = "flagList")] private HashSet<ItemFlags> _flagsList;

        [DataMember(Name = "attributeModifiers")]
        private Dictionary<ItemAttribute, double> _attributeModifiers;

        public ItemMeta()
        {
            _displayName = null;
            _lore = null;
            _flagsList = new HashSet<ItemFlags>();
            _attributeModifiers = new Dictionary<ItemAttribute, double>();
        }

        public ItemMeta(string displayName, string lore, HashSet<ItemFlags> flagsList,
            Dictionary<ItemAttribute, double> attributeModifiers)
        {
            _displayName = displayName;
            _lore = lore;
            _flagsList = flagsList;
            _attributeModifiers = attributeModifiers;
        }

        public HashSet<ItemFlags> FlagsList => _flagsList;

        public Dictionary<ItemAttribute, double> AttributeModifiers => _attributeModifiers;

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

        public bool IsNotEmpty()
        {
            if (DisplayName != null) return true;
            if (Lore != null) return true;
            if (FlagsList.Count > 0) return true;
            if (AttributeModifiers.Count > 0) return true;
            return false;
        }


        public bool Equals(ItemMeta other)
        {
            if (other == null)
                return false;
            

            if (DisplayName == null ^ other.DisplayName == null)
                return false;
            if (Lore == null ^ other.Lore == null)
                return false;

            if (DisplayName != null)
                if (!DisplayName.Equals(other.DisplayName))
                    return false;

            
            if (Lore != null)
                if (!Lore.Equals(other.Lore))
                    return false;
            
            

            if (AttributeModifiers.Count != other.AttributeModifiers.Count)
                return false;

            
            
            if (FlagsList.Count != other.FlagsList.Count)
                return false;
            

            foreach (var (key, value) in AttributeModifiers)
            {
                if (other.AttributeModifiers[key] != value)
                    return false;
            }
            

            foreach (var itemFlags in FlagsList)
            {
                if (!other.FlagsList.Contains(itemFlags))
                    return false;
            }
            

            return true;
        }

        public object Clone()
        {
            List<ItemFlags> flagsList = new List<ItemFlags>();
            foreach (var itemFlag in FlagsList)
            {
                flagsList.Add(itemFlag);
            }

            return new ItemMeta(DisplayName, Lore, FlagsList, AttributeModifiers);
        }
    }
}