using System;
using System.Collections;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using Playground.Manager.Inventory.Meta;
using Playground.Manager.Inventory.Meta.Attributes;

namespace Playground.Manager.Inventory
{
    public class InventoryManager
    {
        public static Inventory GetInventory(long id)
        {
            Inventory inv = _createInventoryFromDbId(id);
            if (inv == null) return null;

            string commandString = @"SELECT 
                itemstacks.id,
	            itemstacks.item_id,
                itemstacks.amount,
                itemmetas.displayName,
                itemmetas.lore,
                itemmetas.flags,
                itemmetas.damage
            FROM inventories
            INNER JOIN itemstacks ON itemstacks.inventory_id  = inventories.id
            LEFT JOIN itemmetas ON itemmetas.itemstack_id  = itemstacks.id
            WHERE inventories.id = @id;";

            MySqlConnection con = DatabaseHandler.GetConnection();
            MySqlCommand command = new MySqlCommand(commandString, con);
            command.Parameters.AddWithValue("id", id);
            MySqlDataReader reader = command.ExecuteReader();

            List<ItemStack> items = new List<ItemStack>();
            while (reader.Read())
            {
                Item item = (Item)reader["item_id"];
                long amount = (long)reader["amount"];
                string displayName = reader["displayName"] == System.DBNull.Value ? null : (string)reader["displayName"];
                string lore = reader["lore"] == System.DBNull.Value ? null : (string)reader["lore"];
                short damage = reader["damage"] == System.DBNull.Value ? (short)0 : (short)reader["damage"];

                if (reader["flags"] != DBNull.Value)
                {
                    BitArray flagBits = new BitArray(new int[] { (int)reader["flags"] });

                    HashSet<ItemFlags> flagsList = new HashSet<ItemFlags>();
                    for (var i = 0; i < flagBits.Count; i++)
                    {
                        if (!flagBits[i]) continue;
                        flagsList.Add((ItemFlags) i);
                    }
                
                    string attributeCommandString = @"
                    SELECT * FROM attributemodifiers
                    WHERE attributemodifiers.itemmeta_id = @id;";

                
                    MySqlConnection con2 = DatabaseHandler.GetConnection();
                    MySqlCommand attributeCommand = new MySqlCommand(attributeCommandString, con2);
                    attributeCommand.Parameters.AddWithValue("id", (long)reader["id"]);
                    MySqlDataReader attributeCommandReader = attributeCommand.ExecuteReader();

                    Dictionary<ItemAttribute, double> attributeModifiers = new Dictionary<ItemAttribute, double>();
                
                    while (attributeCommandReader.Read())
                    {
                        attributeModifiers[(ItemAttribute) attributeCommandReader["attribute"]] = (double)attributeCommandReader["value"];
                    }
                
                    ItemMeta meta = new ItemMeta(displayName, lore, damage, flagsList, attributeModifiers);
                    ItemStack itemStack = new ItemStack(item, amount, meta);
                    items.Add(itemStack);
                }
                else
                {
                    ItemStack itemStack = new ItemStack(item, amount);
                    items.Add(itemStack);
                }
            }
            con.Close();
            con.Dispose();
            reader.Close();
            reader.Dispose();

            return new Inventory(inv.Title, inv.MaxWeight, items, inv.Attributes);
        }

        public static void DeleteInventory(long id)
        {
            string commandString = "DELETE FROM inventories WHERE `inventories`.`id` = @id";
            
            MySqlConnection con = DatabaseHandler.GetConnection();
            MySqlCommand command = new MySqlCommand(commandString, con);
            command.Parameters.AddWithValue("id", id);
            command.ExecuteNonQuery();
            
            con.Close();
            con.Dispose();
        }

        public static void SaveInventory(int id, Inventory inv)
        {
            MySqlConnection con = DatabaseHandler.GetConnection();
            if (_createInventoryFromDbId(id) != null)
            {
                string commandStringItems = @"DELETE FROM itemstacks WHERE inventory_id = @id";
                MySqlCommand commandItems = new MySqlCommand(commandStringItems, con);
                commandItems.Parameters.AddWithValue("id", id);
                commandItems.ExecuteNonQuery();
                commandItems.Dispose();
                
                string commandStringAttributes = @"DELETE FROM inventory_attributes WHERE inventory_id = @id";
                MySqlCommand commandAttributes = new MySqlCommand(commandStringAttributes, con);
                commandAttributes.Parameters.AddWithValue("id", id);
                commandAttributes.ExecuteNonQuery();
                commandAttributes.Dispose();

                MySqlCommand commandUpdateInventoryTitle = new MySqlCommand(@"UPDATE inventories SET title = @title WHERE id = @id", con);
                commandUpdateInventoryTitle.Parameters.AddWithValue("id", id);
                commandUpdateInventoryTitle.Parameters.AddWithValue("title", inv.Title);
                commandUpdateInventoryTitle.ExecuteNonQuery();
                commandUpdateInventoryTitle.Dispose();
                
                MySqlCommand commandUpdateInventoryMaxWeight = new MySqlCommand(@"UPDATE inventories SET maxWeight = @maxWeight WHERE id = @id", con);
                commandUpdateInventoryMaxWeight.Parameters.AddWithValue("id", id);
                commandUpdateInventoryMaxWeight.Parameters.AddWithValue("maxWeight", inv.MaxWeight);
                commandUpdateInventoryMaxWeight.ExecuteNonQuery();
                commandUpdateInventoryMaxWeight.Dispose();
            }
            else
            {
                string commandString = @"
                    INSERT INTO `inventories`(`id`, `title`, `maxWeight`)
                    VALUES(
                        @id,
                        @title,
                        @maxWeight
                )";
            
                MySqlCommand command = new MySqlCommand(commandString, con);
                command.Parameters.AddWithValue("id", id);
                command.Parameters.AddWithValue("title", inv.Title);
                command.Parameters.AddWithValue("maxWeight", inv.MaxWeight);
                command.ExecuteNonQuery();
                command.Dispose();
            }

            // Erstelle Items des Inventars
            
            foreach (var itemStack in inv.Items)
            {
                string itemStackCommandString = @"
                    INSERT INTO `itemstacks`(
                        `id`,
                        `inventory_id`,
                        `item_id`,
                        `amount`
                    )
                    VALUES(NULL, @invId, @itemId, @amount);";
                
                MySqlCommand itemStackCommand = new MySqlCommand(itemStackCommandString, con);
                itemStackCommand.Parameters.AddWithValue("invId", id);
                itemStackCommand.Parameters.AddWithValue("itemId", (long) itemStack.Item);
                itemStackCommand.Parameters.AddWithValue("amount", itemStack.Amount);
                itemStackCommand.ExecuteNonQuery();
                
                Console.WriteLine(itemStack.Meta.IsNotEmpty());
                if (itemStack.Meta.IsNotEmpty())
                {
                    string metaCommandString = @"
                    INSERT INTO `itemmetas`(`itemstack_id`, `displayName`, `lore`, `flags`, `damage`)
                    VALUES(@itemStackId, @displayName, @lore, @flags, @damage)";
            
                    MySqlCommand metaCommand = new MySqlCommand(metaCommandString, con);
                    metaCommand.Parameters.AddWithValue("itemStackId", itemStackCommand.LastInsertedId);
                    metaCommand.Parameters.AddWithValue("displayName", itemStack.Meta.DisplayName);
                    metaCommand.Parameters.AddWithValue("lore", itemStack.Meta.Lore);
                    metaCommand.Parameters.AddWithValue("damage", itemStack.Meta.Damage);

                    double flagValue = 0;
                    foreach (ItemFlags flag in itemStack.Meta.FlagsList)
                    {
                        flagValue += Math.Pow(2, (int)flag);
                    }
                
                    metaCommand.Parameters.AddWithValue("flags", (int)flagValue);
                    metaCommand.ExecuteNonQuery();
                
                    foreach (var attributeModifier in itemStack.Meta.AttributeModifiers)
                    {
                        string attributeCommandString = @"INSERT INTO `itemstack_attributes` (`itemmeta_id`, `attribute`, `value`) VALUES (@itemMetaId, @attribute, @value); ";
            
                        MySqlCommand attributeCommand = new MySqlCommand(attributeCommandString, con);
                        attributeCommand.Parameters.AddWithValue("itemMetaId", itemStackCommand.LastInsertedId);
                        attributeCommand.Parameters.AddWithValue("attribute", (int)attributeModifier.Key);
                        attributeCommand.Parameters.AddWithValue("value", attributeModifier.Value);
                        attributeCommand.ExecuteNonQuery();
                        attributeCommand.Dispose();
                    }
                
                    metaCommand.Dispose();
                }
                itemStackCommand.Dispose();
            }
            
            
            // Erstelle Attribute
            
            foreach (var (inventoryAttribute, value) in inv.Attributes)
            {
                
                string invAttrCommandString = @"
                    INSERT INTO `inventory_attributes`(`inventory_id`, `attribute`, `value`)
                    VALUES(@invId, @attribute, @value);";
                MySqlCommand invAttrCommand = new MySqlCommand(invAttrCommandString, con);
                invAttrCommand.Parameters.AddWithValue("invId", id);
                invAttrCommand.Parameters.AddWithValue("attribute", (int)inventoryAttribute);
                invAttrCommand.Parameters.AddWithValue("value", value);
                invAttrCommand.ExecuteNonQuery();
                invAttrCommand.Dispose();
            }
            
            con.Close();
            con.Dispose();
        }

        static private Inventory _createInventoryFromDbId(long id)
        {
            MySqlConnection con = DatabaseHandler.GetConnection();
            
            string commandString = @"
                SELECT * FROM
                    inventories
                WHERE
                    inventories.id = @id;";
            MySqlCommand command = new MySqlCommand(commandString, con);
            command.Parameters.AddWithValue("id", id);
            MySqlDataReader reader = command.ExecuteReader();

            if (!reader.Read()) return null;
            string title = (string)reader["title"];
            double maxWeight = (double)reader["maxWeight"];
            reader.Close();
            reader.Dispose();
            command.Dispose();

            commandString = @"
                SELECT * FROM
                    inventory_attributes
                WHERE
                    inventory_attributes.inventory_id = @id;";
            command = new MySqlCommand(commandString, con);
            command.Parameters.AddWithValue("id", id);
            reader = command.ExecuteReader();

            Dictionary<InventoryAttribute, double> attributes = new Dictionary<InventoryAttribute, double>();
            while (reader.Read())
            {
                attributes[(InventoryAttribute)reader["attribute"]] = (double)reader["value"];
            }
            
            reader.Close();
            reader.Dispose();
            command.Dispose();
            
            con.Close();
            con.Dispose();
            
            return new Inventory(title, maxWeight, new List<ItemStack>(), attributes);
        }
    }
}