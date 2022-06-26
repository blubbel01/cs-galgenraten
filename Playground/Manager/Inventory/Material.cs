using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace Playground.Manager.Inventory
{
    public enum Material : int
    {
        NULL = 0,
        TEST_MATERIAL = 1,
        WEAPON_SMG = 12,
    }

    public class MaterialObject
    {
        private static Dictionary<Material, MaterialObject> _items = new Dictionary<Material, MaterialObject>();

        public static MaterialObject GetItemData(Material id)
        {
            return _items[id];
        }

        public static void Init()
        {
            string commandString = "SELECT * FROM materials";
            MySqlConnection con = DatabaseHandler.GetConnection();
            MySqlCommand command = new MySqlCommand(commandString, con);
            MySqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                MaterialObject materialObject = new MaterialObject(
                    (int)reader["id"],
                    (string)reader["name"],
                    (double)reader["weight"],
                    (short)reader["durability"],
                    (bool)reader["legal"],
                    (bool)reader["useable"],
                    (bool)reader["wearable"],
                    (bool)reader["placeable"],
                    (bool)reader["locked"]
                );
                _items[(Material)materialObject.Id] = materialObject;
            }
        }

        private int _id;
        private string _name;
        private double _weight;
        private short _durability;
        private bool _legal;
        private bool _useable;
        private bool _wearable;
        private bool _placeable;
        private bool _locked;

        public MaterialObject(int id, string name, double weight, short durability, bool legal, bool useable, bool wearable,
            bool placeable, bool locked)
        {
            _id = id;
            _name = name;
            _weight = weight;
            _durability = durability;
            _legal = legal;
            _useable = useable;
            _wearable = wearable;
            _placeable = placeable;
            _locked = locked;
        }

        public int Id => _id;

        public string Name => _name;
        
        public double Weight => _weight;

        public short Durability => _durability;

        public bool Legal => _legal;

        public bool Useable => _useable;

        public bool Wearable => _wearable;

        public bool Placeable => _placeable;

        public bool Locked => _locked;
    }
}