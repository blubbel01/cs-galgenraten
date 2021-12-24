using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace Playground.Manager
{
    public static class StorageManager
    {
        public static Storage GetStorage(string type, long id)
        {
            string commandString = @"
                SELECT
                    storages.id AS storage_id,
                    storages.max_size,
                    items.id AS item_id,
                    items.name AS item_name,
                    items.weight AS item_weight,
                    items.legal AS item_legal,
                    items.description AS item_description,
                    items.script_type AS item_script_type,
                    items.script_value AS item_script_value,
                    storages_items.amount,
                    storages_items.id AS storage_relation_id,
                    item_metas.script_key AS meta_key,
                    item_metas.script_value AS meta_value
                FROM
                    storages
                INNER JOIN storage_types ON storage_types.id = storages.types_id
                INNER JOIN storages_items ON storages_items.storage_id = storages.id
                INNER JOIN items ON items.id = storages_items.items_id
                LEFT JOIN item_metas ON item_metas.storages_items_id = storages_items.id
                WHERE
                    storages.reference_id = @id
                    AND
                    storage_types.name = @type;";
            MySqlConnection con = DatabaseHandler.GetConnection();
            MySqlCommand command = new MySqlCommand(commandString, con);
            command.Parameters.AddWithValue("id", id);
            command.Parameters.AddWithValue("type", type);
            MySqlDataReader reader = command.ExecuteReader();

            List<Item> items = new List<Item>();
            
            
            string commandStringData = @"
                SELECT
                    storages.id,
                    storages.max_size
                FROM
                    storages
                INNER JOIN storage_types ON storage_types.id = storages.types_id
                WHERE
                    storages.reference_id = @id
                    AND
                    storage_types.name = @type;";
            MySqlConnection conData = DatabaseHandler.GetConnection();
            MySqlCommand commandData = new MySqlCommand(commandStringData, conData);
            commandData.Parameters.AddWithValue("id", id);
            commandData.Parameters.AddWithValue("type", type);
            MySqlDataReader readerData = commandData.ExecuteReader();
            readerData.Read();
            
            Storage storage = new Storage(
                (long) readerData["id"],
                (double) readerData["max_size"],
                id,
                type,
                items
            );
            readerData.Close();
            conData.Close();

            while (reader.Read())
            {
                List<ItemMeta> metas = new List<ItemMeta>();;

                bool contains = false;
                foreach (var item in items)
                {
                    if (item.Id == (long) reader["storage_relation_id"])
                    {
                        contains = true;
                        metas = item.Metas;
                        break;
                    }
                }

                if (!contains)
                {
                    items.Add(new Item(
                        (long) reader["storage_relation_id"],
                        (long) reader["item_id"],
                        (long) reader["storage_id"],
                        (double) reader["item_weight"],
                        (long) reader["amount"],
                        (string) reader["item_name"],
                        (string) reader["item_description"],
                        (bool) reader["item_legal"],
                        metas
                    ));
                }
                if (reader["meta_key"] != System.DBNull.Value)
                {
                    metas.Add(new ItemMeta(
                        (string) reader["meta_key"],
                        (string) reader["meta_value"]
                    ));
                }
            }
            reader.Close();
            con.Close();

            return storage;
        }
    }
    
    
    
    public class Storage
    {
        private long _id;
        private double _maxSize;
        private long _referenceId;
        private string _ownerType;
        private List<Item> _items;
        
        public Storage(long id, double maxSize, long referenceId, string ownerType, List<Item> items)
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

        public List<Item> Items => _items;

        public void AddItem(long itemId, long amount)
        {
            Item existingItem = null;
            foreach (var item in this.Items)
            {
                if (item.ItemId == itemId && item.Amount == amount && item.Metas.Count == 0)
                {
                    existingItem = item;
                }
            }

            if (existingItem == null)
            {
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
            }
            else
            {
                existingItem.Amount += amount;
            }
        }

        public void AddItem(long itemId, long amount, List<ItemMeta> metas)
        {
            Item existingItem = null;
            foreach (var item in this.Items)
            {
                if (item.ItemId == itemId && item.Amount == amount && item.Metas.Count == metas.Count)
                {
                    bool isMetasEqual = true;
                    foreach (var itemMeta in item.Metas)
                    {
                        ItemMeta addMeta = null;
                        foreach (var addItemMeta in metas)
                        {
                            if (itemMeta.Identifier.Equals(addItemMeta.Identifier))
                            {
                                addMeta = addItemMeta;
                                break;
                            }
                        }

                        if (addMeta == null)
                        {
                            isMetasEqual = false;
                            break;
                        }

                        if (!addMeta.Value.Equals(itemMeta.Value))
                        {
                            isMetasEqual = false;
                            break;
                        }
                    }

                    if (isMetasEqual)
                    {
                        existingItem = item;
                    }
                }
            }

            if (existingItem == null)
            {
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
                MySqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    foreach (var itemMeta in metas)
                    {
                        itemMeta.ItemRelationId = (long) reader["id"];
                    }
                }
                reader.Close();
                con.Close();
            }
            else
            {
                existingItem.Amount += amount;
            }
        }
        
        
    }
    public class Item
    {
        private long _id;
        private long _itemId;
        private long _amount;
        private long _storageId;
        private double _weight;
        private string _name;
        private string _description;
        private bool _isLegal;
        private List<ItemMeta> _metas;
        
        public Item(long id, long itemId, long storageId, double weight, long amount, string name, string description, bool isLegal, List<ItemMeta> metas)
        {
            this._id = id;
            this._itemId = itemId;
            this._storageId = storageId;
            this._weight = weight;
            this._amount = amount;
            this._name = name;
            this._description = description;
            this._isLegal = isLegal;
            this._metas = metas;
        }

        public long Id => _id;

        public long ItemId => _itemId;
        
        public double Weight => _weight;

        public string Name => _name;

        public string Description => _description;

        public bool IsLegal => _isLegal;

        public long Amount
        {
            get => _amount;
            
            set
            {
                string commandString = @"
                    UPDATE
                        storages_items
                    SET
                        amount = @amount
                    WHERE
                        storages_items.id = @id;";
                MySqlConnection con = DatabaseHandler.GetConnection();
                MySqlCommand command = new MySqlCommand(commandString, con);
                command.Parameters.AddWithValue("id", this.Id);
                command.Parameters.AddWithValue("amount", value);
                command.ExecuteNonQuery();
                con.Close();
                this._amount = value;
            }
        }

        public List<ItemMeta> Metas => _metas;

        public void AddMeta(ItemMeta meta)
        {
            meta.ItemRelationId = this.Id;
            this._metas.Add(meta);
        }
    }
    public class ItemMeta
    {
        private string _identifier;
        private string _value;
        private long _itemRelationId;

        public ItemMeta(string identifier, string value)
        {
            this._identifier = identifier;
            this._value = value;
        }

        public string Identifier => _identifier;

        public string Value => _value;

        public long ItemRelationId
        {
            get => _itemRelationId;

            set
            {
                string commandString = @"
                    INSERT INTO item_metas (
                        script_key,
                        script_value,
                        storages_items_id
                    )
                    VALUES(@key, @value, @relationId);";
                MySqlConnection con = DatabaseHandler.GetConnection();
                MySqlCommand command = new MySqlCommand(commandString, con);
                command.Parameters.AddWithValue("key", this.Identifier);
                command.Parameters.AddWithValue("value", this.Value);
                command.Parameters.AddWithValue("relationId", value);
                command.ExecuteNonQuery();
                con.Close();
            }
        }
    }
}