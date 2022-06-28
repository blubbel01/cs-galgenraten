using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Playground.Manager.Inventory;
using Playground.Manager.Inventory.Meta;
using Playground.Manager.Inventory.Meta.Attributes;

namespace Playground
{
    class Program
    {
        static void Main(string[] args)
        {
            MaterialObject.Init();

            Exampe();
        }

        static void Exampe()
        {
            Inventory inventory = new Inventory("TEST", 0);
            
            inventory.Attributes[InventoryAttribute.GROUP_STORAGE] = 5;

            ItemStack item = new ItemStack(Item.WEAPON_SMG, 5);
            item.Meta.Damage = 500;
            item.Meta.AttributeModifiers[ItemAttribute.SSAJFG] = 15;
            item.Meta.FlagsList.Add(ItemFlags.NO_TRADE);
            item.Meta.FlagsList.Add(ItemFlags.NO_WEIGHT);
            inventory.AddItem(item);
            ItemStack item2 = new ItemStack(Item.WEAPON_SMG, 1);
            inventory.AddItem(item2);

            HashSet<ItemFlags> filter = new HashSet<ItemFlags>();
            filter.Add(ItemFlags.NO_WEIGHT);
            List<int> slots = inventory.SlotsOfItem(Item.WEAPON_SMG, filter);
            
            InventoryManager.SaveInventory(2, inventory);
            
            // Console.WriteLine(JsonConvert.SerializeObject(InventoryManager.GetInventory(2)));
            Console.WriteLine(JsonConvert.SerializeObject(inventory));
            Console.WriteLine(JsonConvert.SerializeObject(MaterialObject.AllItems()));

            /*
            foreach (var itemStack in inv.Items)
            {
                string s = "";
                s += $"Type: {itemStack.Item}, Name: {itemStack.Name}, Amount: {itemStack.Amount}, Type: {itemStack.Item}, ";
                s += $"DisplayName: {itemStack.Meta.DisplayName}, Lore: {itemStack.Meta.Lore}, Damage: {itemStack.Meta.Damage}, ";
                
                s += "Attributes: [";
                foreach (var attributeModifier in itemStack.Meta.AttributeModifiers)
                {
                    s += $"{attributeModifier.Key}:{attributeModifier.Value},";
                }
                s += "]";
                Console.WriteLine(s);
            }
            */
        }

    }
}