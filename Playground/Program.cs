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
            Exampe();
        }

        static void Exampe()
        {
            Storage testStorage = StorageManager.GetStorage("inventory", 0);

            
            // get Item without any meta
            ItemStack itemStack1 = testStorage.GetItem(1);

            // get Item with exact meta match
            Dictionary<string, string> metas1 = new Dictionary<string, string>();
            metas1.Add("police_marker", "1");
            ItemStack itemStack2 = testStorage.GetItem(1, metas1);
            
            // get all Items by ItemId in List
            List<ItemStack> itemStacks = testStorage.GetItemsByItemId(1);
            
            
            // add item on storage without meta
            testStorage.AddItem(1, 153);
            
            // add item on storage with meta
            Dictionary<string, string> metas2 = new Dictionary<string, string>();
            metas2.Add("police_marker", "1");
            testStorage.AddItem(1, 500, metas2);
            
            // add item by itemObject
            itemStack1.Amount += 50;
            
            // remove item
            // only on itemObject
            itemStack1.Amount -= 100;
            
            // create storage
            Storage newStorage = StorageManager.CreateStorage("inventory", 7227, 200);
            
            // exampe start items in new storage
            newStorage.AddItem(1, 1000);
            newStorage.AddItem(11, 4500);
            
            
            // delete storage by type and reference id
            StorageManager.DeleteStorage("inventory", 7227);
            
            // delete storage by storage id
            StorageManager.DeleteStorage(6);
        }

    }
}