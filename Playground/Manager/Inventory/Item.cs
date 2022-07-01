using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.Serialization;
using MySql.Data.MySqlClient;

namespace Playground.Manager.Inventory
{
    public enum Item : long
    {
        NULL = 0,
        TEST_MATERIAL = 1,
        WEAPON_SMG = 12,
    }

    [DataContract]
    public class MaterialObject
    {
        public static Dictionary<Item, MaterialObject> _items = new Dictionary<Item, MaterialObject>();

        public static MaterialObject GetItemData(Item id)
        {
            return _items[id];
        }

        public static Dictionary<Item, MaterialObject> AllItems()
        {
            return _items;
        }

        public static void Init()
        {
            string commandString = "SELECT * FROM items";
            DataTable results = cDatabase.Instance.ExecutePreparedQueryWithResult(commandString, new Dictionary<string, object>());

            foreach(DataRow row in results.Rows)
            {
                MaterialObject materialObject = new MaterialObject(
                    (long)row["id"],
                    (string)row["name"],
                    row["description"] == DBNull.Value ? null : (string)row["description"],
                    (double)row["weight"],
                    (short)row["durability"],
                    (bool)row["legal"],
                    (bool)row["disabled"],
                    (int)row["heal"],
                    (int)row["food"],
                    (int)row["priceMin"],
                    (int)row["priceMax"],
                    (bool)row["allowTrade"],
                    (bool)row["sync"]
                );
                _items[(Item)materialObject.Id] = materialObject;
            }
        }

        [DataMember(Name = "id")] private long _id;

        [DataMember(Name = "name")] private string _name;

        [DataMember(Name = "description")] private string _description;

        [DataMember(Name = "weight")] private double _weight;

        [DataMember(Name = "durability")] private short _durability;

        [DataMember(Name = "legal")] private bool _legal;

        [DataMember(Name = "disabled")] private bool _disabled;

        [DataMember(Name = "heal")] private int _heal;

        [DataMember(Name = "food")] private int _food;

        [DataMember(Name = "priceMin")] private int _priceMin;

        [DataMember(Name = "priceMax")] private int _priceMax;

        [DataMember(Name = "allowTrade")] private bool _allowTrade;

        [DataMember(Name = "sync")] private bool _sync;

        public MaterialObject(long id, string name, string description, double weight, short durability, bool legal,
            bool disabled, int heal, int food, int priceMin, int priceMax, bool allowTrade, bool sync)
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