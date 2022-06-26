using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace Playground.Manager.Inventory
{
    public enum Item : long
    {
        NULL = 0,
        TEST_MATERIAL = 1,
        WEAPON_SMG = 12,
    }

    public class MaterialObject
    {
        private static Dictionary<Item, MaterialObject> _items = new Dictionary<Item, MaterialObject>();

        public static MaterialObject GetItemData(Item id)
        {
            return _items[id];
        }

        public static void Init()
        {
            string commandString = "SELECT * FROM items";
            MySqlConnection con = DatabaseHandler.GetConnection();
            MySqlCommand command = new MySqlCommand(commandString, con);
            MySqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                MaterialObject materialObject = new MaterialObject(
                    (long)reader["id"],
                    (string)reader["name"],
                    reader["description"] == DBNull.Value ? null : (string)reader["description"],
                    (double)reader["weight"],
                    (short)reader["durability"],
                    (bool)reader["legal"],
                    (bool)reader["disabled"],
                    (int)reader["heal"],
                    (int)reader["food"],
                    (int)reader["priceMin"],
                    (int)reader["priceMax"],
                    (bool)reader["allowTrade"],
                    (bool)reader["sync"]
                );
                _items[(Item)materialObject.Id] = materialObject;
            }
        }

        private long _id;
        private string _name;
        private string _description;
        private double _weight;
        private short _durability;
        private bool _legal;
        private bool _disabled;
        private int _heal;
        private int _food;
        private int _priceMin;
        private int _priceMax;
        private bool _allowTrade;
        private bool _sync;

        public MaterialObject(long id, string name, string description, double weight, short durability, bool legal, bool disabled, int heal, int food, int priceMin, int priceMax, bool allowTrade, bool sync)
        {
            _id = id;
            _name = name;
            _description = description;
            _weight = weight;
            _durability = durability;
            _legal = legal;
            _disabled = disabled;
            _heal = heal;
            _food = food;
            _priceMin = priceMin;
            _priceMax = priceMax;
            _allowTrade = allowTrade;
            _sync = sync;
        }

        public static Dictionary<Item, MaterialObject> Items => _items;

        public long Id => _id;

        public string Name => _name;

        public string Description => _description;

        public double Weight => _weight;

        public short Durability => _durability;

        public bool Legal => _legal;

        public bool Disabled => _disabled;

        public int Heal => _heal;

        public int Food => _food;

        public int PriceMin => _priceMin;

        public int PriceMax => _priceMax;

        public bool AllowTrade => _allowTrade;

        public bool Sync => _sync;
    }
}