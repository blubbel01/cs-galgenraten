using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Playground.Manager.Inventory
{
    [DataContract]
    public class Inventory
    {
        [DataMember(Name = "title")]
        private string _title;
        
        [DataMember(Name = "maxWeight")]
        private double _maxWeight = 0;
        
        [DataMember(Name = "items")]
        private List<ItemStack> _items;
        
        public Inventory(string title, double maxWeight, List<ItemStack> items)
        {
            _title = title;
            _maxWeight = maxWeight;
            _items = items;
        }
        public Inventory(string title, double maxWeight)
        {
            _title = title;
            _maxWeight = maxWeight;
            _items = new List<ItemStack>();
        }
        
        public List<ItemStack> AddItems(List<ItemStack> items)
        {
            List<ItemStack> remaining = new List<ItemStack>();

            for (var i = 0; i < items.Count; i++)
            {
                ItemStack curr = items[i];
                
                int index = _findItemStackIndex(curr);
                
                double emptySpace = MaxWeight - CurrentWeight();
                long transferAmount = (long)(Math.Round(curr.MaterialWeight() / emptySpace) > curr.Amount
                    ? curr.Amount
                    : Math.Round(curr.MaterialWeight() / emptySpace));

                if (MaxWeight == 0)
                {
                    transferAmount = curr.Amount;
                }

                ItemStack toAdd = (ItemStack)curr.Clone();
                toAdd.Amount = transferAmount;
                curr.Amount -= transferAmount;
                if (curr.Amount > 0)
                {
                    remaining.Add(curr);
                }

                if (index == -1)
                {
                    Items.Add(toAdd);
                }
                else
                {
                    Items[index].Amount += toAdd.Amount;
                }
            }

            return items;
        }
        
        public List<ItemStack> RemoveItems(List<ItemStack> items)
        {
            List<ItemStack> remaining = new List<ItemStack>();

            for (var i = 0; i < items.Count; i++)
            {
                ItemStack curr = items[i];
                int index = _findItemStackIndex(curr);

                if (index == -1)
                {
                    remaining.Add(curr);
                }
                else
                {
                    if (Items[index].Amount >= curr.Amount)
                    {
                        Items[index].Amount -= curr.Amount;
                    }
                    else
                    {
                        curr.Amount -= Items[index].Amount;
                        Items[index].Amount = 0;
                        remaining.Add(curr);
                    }
                }
            }

            return items;
        }

        public long AddItem(ItemStack item)
        {
            List<ItemStack> items = new List<ItemStack>();
            items.Add(item);
            
            List<ItemStack> remainder = AddItems(items);
            if (remainder.Count > 0)
            {
                return remainder[0].Amount;
            }

            return 0;
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
    }
}