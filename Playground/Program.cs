using System;
using Newtonsoft.Json;
using Playground.Manager.Inventory;

namespace Playground
{
    class Program
    {
        static void Main(string[] args)
        {
            MaterialObject.Init();

            Example();
        }

        static void Example()
        {
            Console.WriteLine(JsonConvert.SerializeObject(InventoryManager.GetInventory(2)));
        }

    }
}