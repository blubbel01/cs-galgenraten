using MySql.Data.MySqlClient;

namespace Playground.Manager.Storage
{
    public class Item
    {
        private long _id;
        private string _name;
        private double _weight;
        private bool _isLegal;
        private string _description;
        
        public static Item GetItemById(long id)
        {
            string commandString = @"
                SELECT
                    items.id,
                    items.name,
                    items.weight,
                    items.legal,
                    items.description
                FROM
                    items
                WHERE
                    items.id = @id;";
            MySqlConnection con = DatabaseHandler.GetConnection();
            MySqlCommand command = new MySqlCommand(commandString, con);
            command.Parameters.AddWithValue("id", id);
            MySqlDataReader reader = command.ExecuteReader();
            reader.Read();
            Item item = new Item(
                (long) reader["id"],
                (string) reader["name"],
                (double) reader["weight"],
                (bool) reader["legal"],
                (string) reader["description"]
            );
            reader.Close();
            con.Close();
            return item;
        }

        public Item(long id, string name, double weight, bool isLegal, string description)
        {
            _id = id;
            _name = name;
            _weight = weight;
            _isLegal = isLegal;
            _description = description;
        }

        public long Id => _id;

        public string Name => _name;

        public double Weight => _weight;

        public bool IsLegal => _isLegal;

        public string Description => _description;
    }
}