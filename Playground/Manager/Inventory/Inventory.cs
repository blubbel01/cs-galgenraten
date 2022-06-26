using System;
using System.Collections.Generic;
using System.Linq;

namespace Playground.Manager.Inventory
{
    public class Inventory
    {
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

        private string _title;
        private double _maxWeight = 0;
        private List<ItemStack> _items;

        public List<ItemStack> AddItems(List<ItemStack> items)
        {
            List<ItemStack> remaining = new List<ItemStack>();

            for (var i = 0; i < items.Count; i++)
            {
                ItemStack curr = items[i];
                
                int index = _findItemStackIndex(curr);

                double emptySpace = _maxWeight - CurrentWeight();
                int transferAmount = (int)(Math.Round(curr.MaterialWeight() / emptySpace) > curr.Amount
                    ? curr.Amount
                    : Math.Round(curr.MaterialWeight() / emptySpace));

                ItemStack toAdd = (ItemStack)curr.Clone();
                toAdd.Amount = transferAmount;
                curr.Amount -= transferAmount;
                if (curr.Amount > 0)
                {
                    remaining.Add(curr);
                }

                if (index == -1)
                {
                    _items.Add(toAdd);
                }
                else
                {
                    _items[index].Amount += toAdd.Amount;
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
                    if (_items[index].Amount >= curr.Amount)
                    {
                        _items[index].Amount -= curr.Amount;
                    }
                    else
                    {
                        curr.Amount -= _items[index].Amount;
                        _items[index].Amount = 0;
                        remaining.Add(curr);
                    }
                }
            }

            return items;
        }

        public int AddItem(ItemStack item)
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

        public List<ItemStack> Items
        {
            get => _items;
            set => _items = value;
        }

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