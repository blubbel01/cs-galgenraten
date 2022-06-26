using System;
using MySql.Data.MySqlClient;

namespace Playground
{
    public static class DatabaseHandler
    {
        public static MySqlConnection GetConnection()
        {
            MySqlConnection Connection = new MySqlConnection("server=127.0.0.1;uid=root;pwd=;database=test");
            try
            {
                Connection.Open();
                return Connection;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Connection to Database failed!");
                Console.WriteLine(ex.ToString());
                return null;
            }
        }
    }
}