using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace Playground.Manager.Storage
{
    
    public class ItemStack
    {
        private long _id;
        private Item _item;
        private long _amount;
        private long _storageId;
        private Dictionary<string, ItemMeta> _metas;
        
        public ItemStack(long id, Item item, long storageId, long amount, Dictionary<string, ItemMeta> metas)
        {
            this._id = id;
            this._item = item;
            this._storageId = storageId;
            this._amount = amount;
            this._metas = metas;
        }

        public long Id => _id;

        public Item Item => _item;

        public long ItemId => Item.Id;
        
        public double Weight => (Item.Weight * Amount);

        public string Name => Item.Name;

        public string Description => Item.Description;

        public bool IsLegal => Item.IsLegal;

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

        public Dictionary<string, ItemMeta> Metas => _metas;

        public void AddMeta(ItemMeta meta)
        {
            meta.ItemRelationId = this.Id;
            this._metas.Add(meta.Identifier, meta);
        }
    }
}