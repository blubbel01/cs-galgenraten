using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace Playground.Manager.Storage
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

            List<ItemStack> items = new List<ItemStack>();

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
                Dictionary<string, ItemMeta> metas = new Dictionary<string, ItemMeta>();

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
                    items.Add(new ItemStack(
                        (long) reader["storage_relation_id"],
                        (Item) Item.GetItemById((long) reader["item_id"]),
                        (long) reader["storage_id"],
                        (long) reader["amount"],
                        metas
                    ));
                }
                if (reader["meta_key"] != System.DBNull.Value)
                {
                    metas.Add(
                        (string) reader["meta_key"],
                        new ItemMeta(
                            (string) reader["meta_key"],
                            (string) reader["meta_value"]
                        )
                    );
                }
            }
            reader.Close();
            con.Close();

            return storage;
        }

        public static Storage CreateStorage(string type, long id, double maxSize)
        {
            string commandString = @"
                INSERT INTO storages (
                    max_size,
                    types_id,
                    reference_id
                )
                VALUES(@maxSize, @typeId, @referenceId);";
            MySqlConnection con = DatabaseHandler.GetConnection();
            MySqlCommand command = new MySqlCommand(commandString, con);
            command.Parameters.AddWithValue("maxSize", maxSize);
            command.Parameters.AddWithValue("typeId", GetStorageTypeId(type));
            command.Parameters.AddWithValue("referenceId", id);
            command.ExecuteNonQuery();
            con.Close();
            return new Storage(command.LastInsertedId, maxSize, id, type, new List<ItemStack>());
        }

        public static void DeleteStorage(string type, long id)
        {
            string commandString = @"
                DELETE
                FROM
                    storages
                WHERE
                    storages.reference_id = @id
                    AND
                    storages.types_id  = @typeId;";
            MySqlConnection con = DatabaseHandler.GetConnection();
            MySqlCommand command = new MySqlCommand(commandString, con);
            command.Parameters.AddWithValue("id", id);
            command.Parameters.AddWithValue("typeId", GetStorageTypeId(type));
            command.ExecuteNonQuery();
            con.Close();
        }
        
        public static void DeleteStorage(long id)
        {
            string commandString = @"
                DELETE
                FROM
                    storages
                WHERE
                    storages.id = @id;";
            MySqlConnection con = DatabaseHandler.GetConnection();
            MySqlCommand command = new MySqlCommand(commandString, con);
            command.Parameters.AddWithValue("id", id);
            command.ExecuteNonQuery();
            con.Close();
        }

        public static long GetStorageTypeId(string type)
        {
            string commandString = @"
                SELECT
                    storage_types.id
                FROM
                    storage_types
                WHERE
                    storage_types.name = @type;";
            MySqlConnection con = DatabaseHandler.GetConnection();
            MySqlCommand command = new MySqlCommand(commandString, con);
            command.Parameters.AddWithValue("type", type);
            MySqlDataReader reader = command.ExecuteReader();
            long id = -1;
            if (reader.Read())
            {
                id = (long) reader["id"];
            }
            reader.Close();
            con.Close();
            return id;
        }
    }

}