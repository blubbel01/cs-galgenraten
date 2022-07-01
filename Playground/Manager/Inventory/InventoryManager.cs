using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
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

            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@id", id);
            DataTable itemQueryResults = cDatabase.Instance.ExecutePreparedQueryWithResult(commandString, parameters);

            Dictionary<long, ItemStack> items = new Dictionary<long, ItemStack>();

            
            foreach(DataRow row in itemQueryResults.Rows)
            {
                long index = (long)row["index"];
                if (!items.ContainsKey(index))
                {
                    Item item = (Item)row["item_id"];
                    long amount = (long)row["amount"];
                    string displayName = row["displayName"] == System.DBNull.Value
                        ? null
                        : (string)row["displayName"];
                    string lore = row["lore"] == System.DBNull.Value ? null : (string)row["lore"];
                    if (row["flags"] != DBNull.Value)
                    {
                        BitArray flagBits = new BitArray(new int[] { (int)row["flags"] });

                        HashSet<ItemFlags> flagsList = new HashSet<ItemFlags>();
                        for (var i = 0; i < flagBits.Count; i++)
                        {
                            if (!flagBits[i]) continue;
                            flagsList.Add((ItemFlags)i);
                        }

                        Dictionary<ItemAttribute, double> attributeModifiers = new Dictionary<ItemAttribute, double>();

                        if (row["attribute_key"] != DBNull.Value)
                        {
                            attributeModifiers[(ItemAttribute)row["attribute_key"]] =
                                (double)row["attribute_value"];
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
                else if (row["attribute_key"] != DBNull.Value)
                {
                    items[index].Meta.AttributeModifiers[(ItemAttribute)row["attribute_key"]] =
                        (double)row["attribute_value"];
                }
            }

            return new Inventory(inv.Title, inv.MaxWeight, items.Values.ToList(), inv.Attributes);
        }

        public static void DeleteInventory(long id)
        {
            string commandString = "DELETE FROM inventories WHERE `inventories`.`id` = @id";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@id", id);
            cDatabase.Instance.executePreparedQuery(commandString, parameters);
        }

        public static void SaveInventory(int id, Inventory inv)
        {
            Inventory currentDbInventory = GetInventory(id);

            if (currentDbInventory != null)
            {
                if (!currentDbInventory.Title.Equals(inv.Title))
                {
                    Dictionary<string, object> parameters = new Dictionary<string, object>();
                    parameters.Add("@id", id);
                    parameters.Add("@title", inv.Title);
                    cDatabase.Instance.executePreparedQuery("UPDATE inventories SET title = @title WHERE id = @id", parameters);
                }

                if (currentDbInventory.MaxWeight != inv.MaxWeight)
                {
                    Dictionary<string, object> parameters = new Dictionary<string, object>();
                    parameters.Add("@id", id);
                    parameters.Add("@maxWeight", inv.MaxWeight);
                    cDatabase.Instance.executePreparedQuery("UPDATE inventories SET maxWeight = @maxWeight WHERE id = @id", parameters);
                }
                
                int i;
                for (i = 0; i < currentDbInventory.Items.Count; i++)
                {
                    ItemStack currDb = currentDbInventory.Items[i];
                    ItemStack newDb = i < inv.Items.Count ? inv.Items[i] : null;

                    if (currDb.IsSameType(newDb))
                    {
                        
                        if (currDb.Amount != newDb.Amount)
                        {
                            Dictionary<string, object> parameters = new Dictionary<string, object>();
                            parameters.Add("@invId", id);
                            parameters.Add("@index", i);
                            parameters.Add("@amount", newDb.Amount);
                            cDatabase.Instance.executePreparedQuery("UPDATE `itemstacks` SET `amount` = @amount WHERE `itemstacks`.`index` = @index AND `itemstacks`.`inventory_id` = @invId", parameters);
                        }
                    }
                    else
                    {
                        Dictionary<string, object> parameters = new Dictionary<string, object>();
                        parameters.Add("@invId", id);
                        parameters.Add("@index", i);
                        cDatabase.Instance.executePreparedQuery(@"
                                    DELETE
                                    FROM
                                        itemstacks
                                    WHERE
                                        `itemstacks`.`index` = @index AND `itemstacks`.`inventory_id` = @invId", parameters);
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
                            Dictionary<string, object> parameters = new Dictionary<string, object>();
                            parameters.Add("@invId", id);
                            parameters.Add("@attribute", (int)key);
                            parameters.Add("@value", inv.Attributes[key]);
                            cDatabase.Instance.executePreparedQuery(@"
                                UPDATE
                                    `inventory_attributes`
                                SET
                                    `value` = @value
                                WHERE
                                    `inventory_attributes`.`inventory_id` = @invId AND `inventory_attributes`.`attribute` = @attribute", parameters);
                        }
                    }
                    else
                    {
                        Dictionary<string, object> parameters = new Dictionary<string, object>();
                        parameters.Add("@invId", id);
                        parameters.Add("@attribute", (int)key);
                        cDatabase.Instance.executePreparedQuery(@"
                            DELETE
                            FROM
                                inventory_attributes
                            WHERE
                                `inventory_attributes`.`inventory_id` = @invId AND `inventory_attributes`.`attribute` = @attribute", parameters);
                    }
                }

                foreach (var (key, value) in inv.Attributes)
                {
                    if (!currentDbInventory.Attributes.ContainsKey(key))
                    {
                        Dictionary<string, object> parameters = new Dictionary<string, object>();
                        parameters.Add("@invId", id);
                        parameters.Add("@attribute", (int)key);
                        parameters.Add("@value", value);
                        cDatabase.Instance.executePreparedQuery(@"
                            INSERT INTO `inventory_attributes` (`inventory_id`, `attribute`, `value`)
                            VALUES (@invId, @attribute, @value)", parameters);
                    }
                }
            }
            else
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters.Add("@id", id);
                parameters.Add("@title", inv.Title);
                parameters.Add("@maxWeight", inv.MaxWeight);
                cDatabase.Instance.ExecutePreparedQueryWithResult(@"
                    INSERT INTO `inventories`(`id`, `title`, `maxWeight`)
                    VALUES(@id, @title, @maxWeight)", parameters);
                
                Thread.Sleep(50);
                
                // Erstelle Items des Inventars

                for (var i = 0; i < inv.Items.Count; i++)
                {
                    _createItemStack(id, i, inv.Items[i]);
                }

                // Erstelle Attribute
                
                foreach (var (inventoryAttribute, value) in inv.Attributes)
                {
                    Dictionary<string, object> parameters1 = new Dictionary<string, object>();
                    parameters1.Add("@invId", id);
                    parameters1.Add("@attribute", (int)inventoryAttribute);
                    parameters1.Add("@value", value);
                    cDatabase.Instance.executePreparedQuery(@"
                    INSERT INTO `inventory_attributes`(`inventory_id`, `attribute`, `value`)
                    VALUES(@invId, @attribute, @value)", parameters1);
                }
            }
        }

        static private Inventory _createInventoryFromDbId(long id)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@id", id);
            DataTable results = cDatabase.Instance.ExecutePreparedQueryWithResult(@"
                SELECT
                    *
                FROM
                    `inventories`
                LEFT JOIN `inventory_attributes` ON `inventory_attributes`.`inventory_id` = `inventories`.`id`
                WHERE
                    `inventories`.`id` = @id", parameters);

            if (results.Rows.Count == 0) return null;
            string title = (string)results.Rows[0]["title"];
            double maxWeight = (double)results.Rows[0]["maxWeight"];

            Dictionary<InventoryAttribute, double> attributes = new Dictionary<InventoryAttribute, double>();

            if (results.Rows[0]["attribute"] != DBNull.Value)
            {
                for (int i = 0; i < results.Rows.Count; i++)
                {
                    attributes[(InventoryAttribute)results.Rows[i]["attribute"]] = (double)results.Rows[i]["value"];
                }
            }

            return new Inventory(title, maxWeight, new List<ItemStack>(), attributes);
        }

        private static void _createItemStack(long inventoryId, long index, ItemStack itemStack)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@index", index);
            parameters.Add("@invId", inventoryId);
            parameters.Add("@itemId", (long)itemStack.Item);
            parameters.Add("@amount", itemStack.Amount);
            cDatabase.Instance.ExecutePreparedQueryWithResult(@"
                INSERT INTO `itemstacks`(
                    `index`,
                    `inventory_id`,
                    `item_id`,
                    `amount`
                )
                VALUES(@index, @invId, @itemId, @amount)", parameters);

            if (itemStack.Meta.IsNotEmpty())
            {
                double flagValue = 0;
                foreach (ItemFlags flag in itemStack.Meta.FlagsList)
                {
                    flagValue += Math.Pow(2, (int)flag);
                }
                
                Dictionary<string, object> metaParameters = new Dictionary<string, object>();
                metaParameters.Add("@index", index);
                metaParameters.Add("@invId", inventoryId);
                metaParameters.Add("@displayName", itemStack.Meta.DisplayName);
                metaParameters.Add("@lore", itemStack.Meta.Lore);
                metaParameters.Add("@flags", flagValue);
                cDatabase.Instance.ExecutePreparedQueryWithResult(@"
                    INSERT INTO `itemstack_metas`(
                        `inventory_id`,
                        `itemstack_index`,
                        `displayName`,
                        `lore`,
                        `flags`
                    )
                    VALUES(@invId, @index, @displayName, @lore, @flags)", metaParameters);
                
                if (itemStack.Meta.AttributeModifiers.Count > 0)
                {
                    foreach (var attributeModifier in itemStack.Meta.AttributeModifiers)
                    {
                        Dictionary<string, object> attributesParameters = new Dictionary<string, object>();
                        attributesParameters.Add("@index", index);
                        attributesParameters.Add("@invId", inventoryId);
                        attributesParameters.Add("@attribute", (int)attributeModifier.Key);
                        attributesParameters.Add("@value", attributeModifier.Value);
                        cDatabase.Instance.executePreparedQuery(@"
                        INSERT INTO `itemstack_attributes`(
                            `inventory_id`,
                            `itemmeta_itemstack_index`,
                            `attribute`,
                            `value`
                        )
                        VALUES(@invId, @index, @attribute, @value)", attributesParameters);
                    }   
                }
            }
        }
    }
}