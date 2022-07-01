using System;
using Newtonsoft.Json;
using Playground.Manager.Inventory;
using Playground.Manager.Inventory.Meta.Attributes;

namespace Playground
{
    class Program
    {
        static void Main(string[] args)
        {
            new cDatabaseManager();
            cDatabaseManager.instance.tryConnect();
        }

        public static void Example()
        {
            MaterialObject.Init();

            for (int i = 0; i < 20; i++)
            {
                _testCreateItemMeta();
                _testCreateItemNormal();
                _testUpdateAmount();
                _testDeleteItem();
            }
        }

        private static void _testCreateItemNormal()
        {
            Console.WriteLine("[CHECK] Create Item Normal!");
            
            Inventory inv = new Inventory("Test", 10000);
            ItemStack newItem = new ItemStack(Item.WEAPON_SMG, 12);
            inv.AddItem(newItem);
            
            int invId = InventoryManager.CreateInventory(inv);
            
            Inventory checkInf = InventoryManager.GetInventory(invId);

            if (!inv.Equals(checkInf))
            {
                Console.WriteLine("[ERROR] Create Item Normal!");
                string s1 = JsonConvert.SerializeObject(inv);
                string s2 = JsonConvert.SerializeObject(checkInf);
                Console.WriteLine(s1);
                Console.WriteLine(s2);
            }
            else
            {
                Console.WriteLine("[SUCCESS] Create Item Normal!");
            }
        }

        private static void _testCreateItemMeta()
        {
            Console.WriteLine("[CHECK] Create Item Meta!");
            Inventory inv = new Inventory("Test", 10000);
            ItemStack newItem = new ItemStack(Item.WEAPON_SMG, 12);
            newItem.Meta.DisplayName = "Nee Nam";
            newItem.Meta.AttributeModifiers[ItemAttribute.SSAJFG] = 15;
            inv.AddItem(newItem);
            int invId =  InventoryManager.CreateInventory(inv);
            
            
            Inventory checkInf = InventoryManager.GetInventory(invId);

            if (!inv.Equals(checkInf))
            {
                Console.WriteLine("[ERROR] Create Item Meta!");
                string s1 = JsonConvert.SerializeObject(inv);
                string s2 = JsonConvert.SerializeObject(checkInf);
                Console.WriteLine(s1);
                Console.WriteLine(s2);
            }
            else
            {
                Console.WriteLine("[SUCCESS] Create Item Meta!");
            }
        }

        private static void _testUpdateAmount()
        {
            Console.WriteLine("[CHECK] Update Amount!");
            Inventory inv = new Inventory("Test", 10000);
            ItemStack newItem = new ItemStack(Item.WEAPON_SMG, 253);
            newItem.Meta.DisplayName = "Nee Nam";
            newItem.Meta.AttributeModifiers[ItemAttribute.SSAJFG] = 15;
            inv.AddItem(newItem);
            int invId =  InventoryManager.CreateInventory(inv);

            inv = InventoryManager.GetInventory(invId);
            inv.RemoveItemAmount(inv.SlotsOfItem(Item.WEAPON_SMG)[0], 12);
            InventoryManager.SaveInventory(invId, inv);
            
            
            Inventory checkInf = InventoryManager.GetInventory(invId);

            if (!inv.Equals(checkInf))
            {
                Console.WriteLine("[ERROR] Update Amount!");
                string s1 = JsonConvert.SerializeObject(inv);
                string s2 = JsonConvert.SerializeObject(checkInf);
                Console.WriteLine(s1);
                Console.WriteLine(s2);
            }
            else
            {
                Console.WriteLine("[SUCCESS] Update Amount!");
            }
        }

        private static void _testDeleteItem()
        {
            Console.WriteLine("[CHECK] Update Amount!");
            Inventory inv = new Inventory("Test", 10000);
            ItemStack newItem = new ItemStack(Item.WEAPON_SMG, 253);
            newItem.Meta.DisplayName = "Nee Nam";
            newItem.Meta.AttributeModifiers[ItemAttribute.SSAJFG] = 15;
            inv.AddItem(newItem);
            int invId =  InventoryManager.CreateInventory(inv);

            inv = InventoryManager.GetInventory(invId);
            inv.RemoveItemAmount(inv.SlotsOfItem(Item.WEAPON_SMG)[0], 253);
            InventoryManager.SaveInventory(invId, inv);
            
            
            Inventory checkInf = InventoryManager.GetInventory(invId);

            if (!inv.Equals(checkInf))
            {
                Console.WriteLine("[ERROR] Delete Amount!");
                string s1 = JsonConvert.SerializeObject(inv);
                string s2 = JsonConvert.SerializeObject(checkInf);
                Console.WriteLine(s1);
                Console.WriteLine(s2);
            }
            else
            {
                Console.WriteLine("[SUCCESS] Delete Amount!");
            }
        }

    }
}