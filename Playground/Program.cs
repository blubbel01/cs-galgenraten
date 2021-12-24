using System;
using System.Collections.Generic;
using System.IO;
using Playground.Manager;

namespace Playground
{
    class Program
    {
        static void Main(string[] args)
        {
            Storage testStorage = StorageManager.GetStorage("inventory", 0);
            
            testStorage.AddItem(1, 50000);
            
            foreach (var item in testStorage.Items)
            {
                Console.WriteLine($"{item.Amount}x {item.ItemId}");
            }
        }

    }
}