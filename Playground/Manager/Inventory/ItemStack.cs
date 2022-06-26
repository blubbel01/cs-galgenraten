using System;
using Playground.Manager.Inventory.Meta;

namespace Playground.Manager.Inventory
{
    public class ItemStack : ICloneable
    {
        private Material _type = Material.NULL;
        private int _amount = 0;
        private ItemMeta _meta;

        public ItemStack(Material type, int amount)
        {
            _type = type;
            _amount = amount;
            _meta = new ItemMeta();
        }

        public ItemStack(Material type, int amount, ItemMeta meta)
        {
            _type = type;
            _amount = amount;
            _meta = meta;
        }

        public bool IsSameType(ItemStack other)
        {
            if (!_type.Equals(other.Type))
                return false;
            if (!_meta.Equals(other._meta))
                return false;
            return true;
        }

        public Material Type => _type;

        public ItemMeta Meta => _meta;

        public string Name => MaterialObject.GetItemData(_type).Name;

        public MaterialObject Data => MaterialObject.GetItemData(_type);

        public int Amount
        {
            get => _amount;
            set => _amount = value;
        }

        public double MaterialWeight()
        {
            return MaterialObject.GetItemData(_type).Weight;
        }
        
        public double Weight()
        {
            return MaterialWeight() * Amount;
        }

        public object Clone()
        {
            return new ItemStack(_type, _amount, (ItemMeta) _meta.Clone());
        }
    }
}