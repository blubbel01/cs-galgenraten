using System;
using System.Collections.Generic;
using System.IO;
using Playground.Manager.Storage;

namespace Playground
{
    class Program
    {
        static void Main(string[] args)
        {
            Storage testStorage = StorageManager.GetStorage("inventory", 0);

            foreach (var item in testStorage.Items)
            {
                string metas = "";
                foreach (var keyValuePair in item.Metas)
                {
                    metas += $"{keyValuePair.Key} = {keyValuePair.Value.Value}, ";
                }
                Console.WriteLine($"{item.Amount}x {item.ItemId}: {metas}");
            }
        }

    }
}