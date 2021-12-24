using MySql.Data.MySqlClient;

namespace Playground.Manager.Storage
{
    
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