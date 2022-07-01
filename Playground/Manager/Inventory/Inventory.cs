using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Playground.Manager.Inventory.Meta;

namespace Playground.Manager.Inventory
{
    public enum InventoryAttribute
    {
        GROUP_STORAGE
    }

    [DataContract]
    public class Inventory
    {
        [DataMember(Name = "title")] private string _title;

        [DataMember(Name = "maxWeight")] private double _maxWeight = 0;

        [DataMember(Name = "items")] private List<ItemStack> _items;

        [DataMember(Name = "attributes")] private Dictionary<InventoryAttribute, double> _attributes;

        public Inventory(string title, double maxWeight, List<ItemStack> items,
            Dictionary<InventoryAttribute, double> attributes)
        {
            _title = title;
            _maxWeight = maxWeight;
            _items = items;
            _attributes = attributes;
        }

        public Inventory(string title, double maxWeight)
        {
            _title = title;
            _maxWeight = maxWeight;
            _items = new List<ItemStack>();
            _attributes = new Dictionary<InventoryAttribute, double>();
        }

        public bool AddItems(List<ItemStack> items, bool ignore = false, bool forced = false, bool syncIfAmmo = true)
        {
            if (!ignore && !forced)
            {
                double addWeight = 0;
                foreach (var itemStack in items)
                {
                    addWeight += itemStack.Weight();
                }

                if (addWeight + CurrentWeight() > MaxWeight)
                {
                    return false;
                }
            }


            for (var i = 0; i < items.Count; i++)
            {
                ItemStack curr = items[i];

                int index = _findItemStackIndex(curr);

                double emptySpace = MaxWeight - CurrentWeight();
                long transferAmount = (long)(Math.Round(curr.MaterialWeight() / emptySpace) > curr.Amount
                    ? curr.Amount
                    : Math.Round(curr.MaterialWeight() / emptySpace));

                if (MaxWeight == 0 || forced)
                {
                    transferAmount = curr.Amount;
                }

                ItemStack toAdd = (ItemStack)curr.Clone();
                toAdd.Amount = transferAmount;
                curr.Amount -= transferAmount;

                if (index == -1)
                {
                    Items.Add(toAdd);
                }
                else
                {
                    Items[index].Amount += toAdd.Amount;
                }
            }

            return true;
        }

        public void SetItemAmount(int index, long amount)
        {
            if (amount > 0)
            {
                Items[index].Amount = amount;
            }
            else
            {
                Items.RemoveAt(index);
            }
        }

        public void RemoveItemAmount(int index, long amount, bool allowNegative = false)
        {
            if (Items[index].Amount - amount > 0 && !allowNegative)
            {
                Items[index].Amount -= amount;
            }
            else
            {
                Items.RemoveAt(index);
            }
        }

        public List<int> SlotsOfItem(Item item)
        {
            return SlotsOfItem(item, null);
        }

        public List<int> SlotsOfItem(Item item, HashSet<ItemFlags> flags)
        {
            List<int> slots = new List<int>();
            for (var i = 0; i < Items.Count; i++)
            {
                var itemStack = Items[i];
                if (itemStack.Item == item)
                {
                    if (flags != null)
                    {
                        if (itemStack.Meta.FlagsList.Overlaps(flags))
                        {
                            slots.Add(i);
                        }
                    }
                    else
                    {
                        slots.Add(i);
                    }
                }
            }

            return slots;
        }

        public bool AddItem(ItemStack item)
        {
            List<ItemStack> items = new List<ItemStack>();
            items.Add(item);

            return AddItems(items);
        }

        public double CurrentWeight()
        {
            double weight = 0;
            foreach (var item in _items)
            {
                weight += item.Weight();
            }

            return weight;
        }

        private int _findItemStackIndex(ItemStack item)
        {
            if (Items.Count == 0) return -1;
            for (var i = 0; i < Items.Count; i++)
            {
                ItemStack curr = Items[i];
                if (curr.IsSameType(item))
                {
                    return i;
                }
            }

            return -1;
        }

        public List<ItemStack> Items => _items;

        public Dictionary<InventoryAttribute, double> Attributes => _attributes;

        public double MaxWeight
        {
            get => _maxWeight;
            set => _maxWeight = value;
        }

        public string Title
        {
            get => _title;
            set => _title = value;
        }

        public Dictionary<int, int> LegacyFormat()
        {
            Dictionary<int, int> legacyInventory = new Dictionary<int, int>();
            foreach (var itemStack in Items)
            {
                if (!itemStack.Meta.IsNotEmpty())
                {
                    legacyInventory[(int)itemStack.Item] = (int)itemStack.Amount;
                }
            }

            return legacyInventory;
        }
    }
}