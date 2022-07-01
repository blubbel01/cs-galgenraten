using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
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

            string commandString = @"
                SELECT
                    itemstacks.index,
                    itemstacks.item_id,
                    itemstacks.amount,
                    itemstack_metas.displayName,
                    itemstack_metas.lore,
                    itemstack_metas.flags,
                    itemstack_attributes.attribute as 'attribute_key',
                    itemstack_attributes.value as 'attribute_value'
                FROM
                    inventories
                INNER JOIN itemstacks ON itemstacks.inventory_id = inventories.id
                LEFT JOIN itemstack_metas ON itemstack_metas.itemstack_index = itemstacks.index AND itemstack_metas.inventory_id = inventories.id
                LEFT JOIN itemstack_attributes ON itemstack_attributes.itemmeta_itemstack_index = itemstack_metas.itemstack_index AND itemstack_attributes.inventory_id = inventories.id
                WHERE
                    inventories.id = @id;";

            MySqlConnection con = DatabaseHandler.GetConnection();
            MySqlCommand command = new MySqlCommand(commandString, con);
            command.Parameters.AddWithValue("id", id);
            MySqlDataReader reader = command.ExecuteReader();

            Dictionary<long, ItemStack> items = new Dictionary<long, ItemStack>();

            while (reader.Read())
            {
                long index = (long)reader["index"];
                if (!items.ContainsKey(index))
                {
                    Item item = (Item)reader["item_id"];
                    long amount = (long)reader["amount"];
                    string displayName = reader["displayName"] == System.DBNull.Value
                        ? null
                        : (string)reader["displayName"];
                    string lore = reader["lore"] == System.DBNull.Value ? null : (string)reader["lore"];
                    if (reader["flags"] != DBNull.Value)
                    {
                        BitArray flagBits = new BitArray(new int[] { (int)reader["flags"] });

                        HashSet<ItemFlags> flagsList = new HashSet<ItemFlags>();
                        for (var i = 0; i < flagBits.Count; i++)
                        {
                            if (!flagBits[i]) continue;
                            flagsList.Add((ItemFlags)i);
                        }

                        Dictionary<ItemAttribute, double> attributeModifiers = new Dictionary<ItemAttribute, double>();

                        if (reader["attribute_key"] != DBNull.Value)
                        {
                            attributeModifiers[(ItemAttribute)reader["attribute_key"]] =
                                (double)reader["attribute_value"];
                        }

                        ItemMeta meta = new ItemMeta(displayName, lore, flagsList, attributeModifiers);
                        ItemStack itemStack = new ItemStack(item, amount, meta);

                        items[index] = itemStack;
                    }
                    else
                    {
                        ItemStack itemStack = new ItemStack(item, amount);
                        items[index] = itemStack;
                    }
                }
                else if (reader["attribute_key"] != DBNull.Value)
                {
                    items[index].Meta.AttributeModifiers[(ItemAttribute)reader["attribute_key"]] =
                        (double)reader["attribute_value"];
                }
            }

            con.Close();
            con.Dispose();
            reader.Close();
            reader.Dispose();

            return new Inventory(inv.Title, inv.MaxWeight, items.Values.ToList(), inv.Attributes);
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

            Inventory currentDbInventory = GetInventory(id);

            if (currentDbInventory != null)
            {
                if (!currentDbInventory.Title.Equals(inv.Title))
                {
                    MySqlCommand commandUpdateInventoryTitle =
                        new MySqlCommand(@"UPDATE inventories SET title = @title WHERE id = @id", con);
                    commandUpdateInventoryTitle.Parameters.AddWithValue("id", id);
                    commandUpdateInventoryTitle.Parameters.AddWithValue("title", inv.Title);
                    commandUpdateInventoryTitle.ExecuteNonQuery();
                    commandUpdateInventoryTitle.Dispose();
                }

                // Macht man wohl seperat...
                /*
                if (currentDbInventory.MaxWeight != inv.MaxWeight)
                {
                    MySqlCommand commandUpdateInventoryMaxWeight =
                        new MySqlCommand(@"UPDATE inventories SET maxWeight = @maxWeight WHERE id = @id", con);
                    commandUpdateInventoryMaxWeight.Parameters.AddWithValue("id", id);
                    commandUpdateInventoryMaxWeight.Parameters.AddWithValue("maxWeight", inv.MaxWeight);
                    commandUpdateInventoryMaxWeight.ExecuteNonQuery();
                    commandUpdateInventoryMaxWeight.Dispose();
                }
                */
                
                int i;
                for (i = 0; i < currentDbInventory.Items.Count; i++)
                {
                    ItemStack currDb = currentDbInventory.Items[i];
                    ItemStack newDb = i < inv.Items.Count ? inv.Items[i] : null;
                    
                    
                    
                    

                    if (currDb.IsSameType(newDb))
                    {
                        
                        if (currDb.Amount != newDb.Amount)
                        {
                            MySqlCommand commandUpdateItemStackAmount =
                                new MySqlCommand(
                                    @"UPDATE `itemstacks` SET `amount` = @amount WHERE `itemstacks`.`index` = @index AND `itemstacks`.`inventory_id` = @invId; ",
                                    con);
                            commandUpdateItemStackAmount.Parameters.AddWithValue("amount", newDb.Amount);
                            commandUpdateItemStackAmount.Parameters.AddWithValue("index", i);
                            commandUpdateItemStackAmount.Parameters.AddWithValue("invId", id);
                            commandUpdateItemStackAmount.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        
                        MySqlCommand commandUpdateItemStackAmount =
                            new MySqlCommand(@"
                                    DELETE
                                    FROM
                                        itemstacks
                                    WHERE
                                        `itemstacks`.`index` = @index AND `itemstacks`.`inventory_id` = @invId",
                                con);
                        commandUpdateItemStackAmount.Parameters.AddWithValue("index", i);
                        commandUpdateItemStackAmount.Parameters.AddWithValue("invId", id);
                        commandUpdateItemStackAmount.ExecuteNonQuery();
                        if (newDb != null)
                        {
                            _createItemStack(id, i, newDb);
                        }
                    }
                }

                for (; i < inv.Items.Count; i++)
                {
                    _createItemStack(id, i, inv.Items[i]);
                }

                foreach (var (key, value) in currentDbInventory.Attributes)
                {
                    if (inv.Attributes.ContainsKey(key))
                    {
                        if (inv.Attributes[key] != currentDbInventory.Attributes[key])
                        {
                            string cmd = @"
                                UPDATE
                                    `inventory_attributes`
                                SET
                                    `value` = @value
                                WHERE
                                    `inventory_attributes`.`inventory_id` = @invId AND `inventory_attributes`.`attribute` = @attribute;";
                            MySqlCommand commandInventoryAttribute = new MySqlCommand(cmd, con);
                            commandInventoryAttribute.Parameters.AddWithValue("attribute", (int)key);
                            commandInventoryAttribute.Parameters.AddWithValue("value", inv.Attributes[key]);
                            commandInventoryAttribute.Parameters.AddWithValue("invId", id);
                            commandInventoryAttribute.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        string cmd = @"
                            DELETE
                            FROM
                                inventory_attributes
                            WHERE
                                `inventory_attributes`.`inventory_id` = @invId AND `inventory_attributes`.`attribute` = @attribute";
                        MySqlCommand commandInventoryAttribute = new MySqlCommand(cmd, con);
                        commandInventoryAttribute.Parameters.AddWithValue("attribute", (int)key);
                        commandInventoryAttribute.Parameters.AddWithValue("invId", id);
                        commandInventoryAttribute.ExecuteNonQuery();
                    }
                }

                foreach (var (key, value) in inv.Attributes)
                {
                    if (!currentDbInventory.Attributes.ContainsKey(key))
                    {
                        string cmd = @"
                            INSERT INTO `inventory_attributes` (`inventory_id`, `attribute`, `value`)
                            VALUES (@invId, @attribute, @value)";
                        MySqlCommand commandInventoryAttribute = new MySqlCommand(cmd, con);
                        commandInventoryAttribute.Parameters.AddWithValue("attribute", (int)key);
                        commandInventoryAttribute.Parameters.AddWithValue("value", value);
                        commandInventoryAttribute.Parameters.AddWithValue("invId", id);
                        commandInventoryAttribute.ExecuteNonQuery();
                    }
                }
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


                // Erstelle Items des Inventars

                for (var i = 0; i < inv.Items.Count; i++)
                {
                    _createItemStack(id, i, inv.Items[i]);
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
        }

        static private Inventory _createInventoryFromDbId(long id)
        {
            MySqlConnection con = DatabaseHandler.GetConnection();

            string commandString = @"
                SELECT
                    *
                FROM
                    `inventories`
                LEFT JOIN `inventory_attributes` ON `inventory_attributes`.`inventory_id` = `inventories`.`id`
                WHERE
                    `inventories`.`id` = @id;";
            MySqlCommand command = new MySqlCommand(commandString, con);
            command.Parameters.AddWithValue("id", id);
            MySqlDataReader reader = command.ExecuteReader();

            if (!reader.Read()) return null;
            string title = (string)reader["title"];
            double maxWeight = (double)reader["maxWeight"];

            Dictionary<InventoryAttribute, double> attributes = new Dictionary<InventoryAttribute, double>();

            if (reader["attribute"] != DBNull.Value)
            {
                do
                {
                    attributes[(InventoryAttribute)reader["attribute"]] = (double)reader["value"];
                } while (reader.Read());
            }


            reader.Close();
            reader.Dispose();
            command.Dispose();
            con.Close();
            con.Dispose();

            return new Inventory(title, maxWeight, new List<ItemStack>(), attributes);
        }

        static private void _createItemStack(long inventoryId, long index, ItemStack itemStack)
        {
            MySqlConnection con = DatabaseHandler.GetConnection();

            string itemStackCommandString = @"
                    INSERT INTO `itemstacks`(
                        `index`,
                        `inventory_id`,
                        `item_id`,
                        `amount`
                    )
                    VALUES(@index, @invId, @itemId, @amount);";

            MySqlCommand itemStackCommand = new MySqlCommand(itemStackCommandString, con);
            itemStackCommand.Parameters.AddWithValue("index", index);
            itemStackCommand.Parameters.AddWithValue("invId", inventoryId);
            itemStackCommand.Parameters.AddWithValue("itemId", (long)itemStack.Item);
            itemStackCommand.Parameters.AddWithValue("amount", itemStack.Amount);
            itemStackCommand.ExecuteNonQuery();

            
            if (itemStack.Meta.IsNotEmpty())
            {
                string metaCommandString = @"
                    INSERT INTO `itemstack_metas`(
                        `inventory_id`,
                        `itemstack_index`,
                        `displayName`,
                        `lore`,
                        `flags`
                    )
                    VALUES(@invId, @index, @displayName, @lore, @flags)";

                MySqlCommand metaCommand = new MySqlCommand(metaCommandString, con);
                metaCommand.Parameters.AddWithValue("invId", inventoryId);
                metaCommand.Parameters.AddWithValue("index", index);
                metaCommand.Parameters.AddWithValue("displayName", itemStack.Meta.DisplayName);
                metaCommand.Parameters.AddWithValue("lore", itemStack.Meta.Lore);

                double flagValue = 0;
                foreach (ItemFlags flag in itemStack.Meta.FlagsList)
                {
                    flagValue += Math.Pow(2, (int)flag);
                }

                metaCommand.Parameters.AddWithValue("flags", (int)flagValue);
                metaCommand.ExecuteNonQuery();

                foreach (var attributeModifier in itemStack.Meta.AttributeModifiers)
                {
                    string attributeCommandString = @"
                        INSERT INTO `itemstack_attributes`(
                            `inventory_id`,
                            `itemmeta_itemstack_index`,
                            `attribute`,
                            `value`
                        )
                        VALUES(@invId, @index, @attribute, @value);";

                    MySqlCommand attributeCommand = new MySqlCommand(attributeCommandString, con);
                    attributeCommand.Parameters.AddWithValue("invId", inventoryId);
                    attributeCommand.Parameters.AddWithValue("index", index);
                    attributeCommand.Parameters.AddWithValue("attribute", (int)attributeModifier.Key);
                    attributeCommand.Parameters.AddWithValue("value", attributeModifier.Value);
                    attributeCommand.ExecuteNonQuery();
                    attributeCommand.Dispose();
                }

                metaCommand.Dispose();
            }

            itemStackCommand.Dispose();
        }
    }
}