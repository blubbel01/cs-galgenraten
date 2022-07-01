using System;
using Newtonsoft.Json;
using Playground.Manager.Inventory;

namespace Playground
{
    class Program
    {
        static void Main(string[] args)
        {
            new cDatabaseManager();
            cDatabaseManager.instance.onResourceStart();
        }

        public static void Example()
        {
            MaterialObject.Init();
            Console.WriteLine(JsonConvert.SerializeObject(InventoryManager.GetInventory(2)));
        }

    }
}