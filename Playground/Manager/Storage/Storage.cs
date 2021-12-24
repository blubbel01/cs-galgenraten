using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace Playground.Manager.Storage
{
    
    public class Storage
    {
        private long _id;
        private double _maxSize;
        private long _referenceId;
        private string _ownerType;
        private List<ItemStack> _items;
        
        public Storage(long id, double maxSize, long referenceId, string ownerType, List<ItemStack> items)
        {
            this._id = id;
            this._maxSize = maxSize;
            this._referenceId = referenceId;
            this._ownerType = ownerType;
            this._items = items;
        }

        public long Id => _id;

        public double MaxSize
        {
            get => _maxSize;
            set => _maxSize = value;
        }

        public string OwnerType => _ownerType;

        public long ReferenceId => _referenceId;

        public List<ItemStack> Items => _items;

        public void AddItem(long itemId, long amount)
        {
            ItemStack itemStack = GetItem(itemId);
            if (itemStack != null)
            {
                itemStack.Amount += amount;
            }
            else
            {
                Item item = Item.GetItemById(itemId);
                string commandString = @"
                INSERT INTO storages_items (
                    items_id,
                    storage_id,
                    amount
                )
                VALUES(@itemId, @storageId, @amount)";
                MySqlConnection con = DatabaseHandler.GetConnection();
                MySqlCommand command = new MySqlCommand(commandString, con);
                command.Parameters.AddWithValue("itemId", itemId);
                command.Parameters.AddWithValue("storageId", this.Id);
                command.Parameters.AddWithValue("amount", amount);
                command.ExecuteNonQuery();
                con.Close();
                this.Items.Add(new ItemStack(
                    command.LastInsertedId,
                    item,
                    this.Id,
                    amount,
                    new Dictionary<string, ItemMeta>()
                ));
            }
        }

        public void AddItem(int itemId, int amount, Dictionary<string, string> metas)
        {
            ItemStack itemStack = GetItem(itemId, metas);
            if (itemStack != null)
            {
                itemStack.Amount += amount;
            }
            else
            {
                Item item = Item.GetItemById(itemId);
                string commandString = @"
                INSERT INTO storages_items (
                    items_id,
                    storage_id,
                    amount
                )
                VALUES(@itemId, @storageId, @amount)";
                MySqlConnection con = DatabaseHandler.GetConnection();
                MySqlCommand command = new MySqlCommand(commandString, con);
                command.Parameters.AddWithValue("itemId", itemId);
                command.Parameters.AddWithValue("storageId", this.Id);
                command.Parameters.AddWithValue("amount", amount);
                command.ExecuteNonQuery();
                con.Close();
                
                Dictionary<string, ItemMeta> itemMetas = new Dictionary<string, ItemMeta>();
                itemStack = new ItemStack(
                    command.LastInsertedId,
                    item,
                    this.Id,
                    amount,
                    itemMetas
                );
                this.Items.Add(itemStack);
                
                foreach (var meta in metas)
                {
                    itemStack.AddMeta(new ItemMeta(meta.Key, meta.Value));
                }
            }
        }

        public ItemStack GetItem(long itemId, Dictionary<string, string> metas)
        {
            ItemStack existingItemStack = null;
            foreach (var item in this.Items)
            {
                if (item.ItemId == itemId)
                {
                    bool sameMeta = (item.Metas.Count == metas.Count);

                    if (sameMeta)
                    {
                        // check if same meta is true
                        foreach (var keyValuePair in item.Metas)
                        {
                            bool metaFound = false;
                            foreach (var valuePair in metas)
                            {
                                // check if there are equal meta keys
                                if (keyValuePair.Key.Equals(valuePair.Key) && keyValuePair.Value.Value.Equals(valuePair.Value))
                                {
                                    metaFound = true;
                                    break;
                                }
                            }

                            if (!metaFound)
                            {
                                sameMeta = false;
                                break;
                            }
                        }
                    }

                    if (!sameMeta)
                    {
                        break;
                    }

                    existingItemStack = item;
                }
            }

            return existingItemStack;
        }

        public ItemStack GetItem(long itemId)
        {
            ItemStack existingItemStack = null;
            foreach (var item in this.Items)
            {
                if (item.ItemId == itemId && item.Metas.Count == 0)
                {
                    existingItemStack = item;
                    break;
                }
            }

            return existingItemStack;
        }

        public List<ItemStack> GetItemsByItemId(long itemId)
        {
            List<ItemStack> items = new List<ItemStack>();
            foreach (var item in this.Items)
            {
                if (item.ItemId == itemId)
                {
                    items.Add(item);
                }
            }
            return items;
        }
    }
}