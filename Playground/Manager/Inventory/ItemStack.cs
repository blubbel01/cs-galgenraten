using System;
using System.Runtime.Serialization;
using Playground.Manager.Inventory.Meta;

namespace Playground.Manager.Inventory
{
    [DataContract]
    public class ItemStack : IEquatable<ItemStack>, ICloneable
    {
        [DataMember(Name = "item")] private Item _item = Manager.Inventory.Item.NULL;

        [DataMember(Name = "amount")] private long _amount = 0;

        [DataMember(Name = "meta")] private ItemMeta _meta;

        public ItemStack(Item item, long amount)
        {
            _item = item;
            _amount = amount;
            _meta = new ItemMeta();
        }

        public ItemStack(Item item, long amount, ItemMeta meta)
        {
            _item = item;
            _amount = amount;
            _meta = meta;
        }

        public bool IsSameType(ItemStack other)
        {
            if (other == null)
            {
                return false;
            }

            if (_item != other.Item)
            {
                return false;
            }

            if (!_meta.Equals(other._meta))
            {
                return false;
            }

            return true;
        }

        public Item Item => _item;

        public ItemMeta Meta => _meta;

        public string Name => MaterialObject.GetItemData(_item).Name;

        public MaterialObject Data => MaterialObject.GetItemData(_item);

        public long Amount
        {
            get => _amount;
            set => _amount = value;
        }

        public double MaterialWeight()
        {
            return MaterialObject.GetItemData(_item).Weight;
        }

        public double Weight()
        {
            return MaterialWeight() * Amount;
        }

        public object Clone()
        {
            return new ItemStack(_item, _amount, (ItemMeta)_meta.Clone());
        }

        public bool Equals(ItemStack other)
        {
            if (!Name.Equals(other.Name))
            {
                return false;
            }

            if (Amount != other.Amount)
            {
                return false;
            }

            if (!Meta.Equals(other.Meta))
            {
                return false;
            }

            return true;
        }
    }
}