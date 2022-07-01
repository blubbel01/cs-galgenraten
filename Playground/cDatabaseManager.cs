using System;
using System.Collections.Generic;
using System.Data;
using MySqlConnector;
using Playground;

public class cDatabaseManager
{
    /* Variables */
    private string connStr = "server=127.0.0.1;uid=root;pwd=;database=test";
    Dictionary<string, MySqlDataAdapter> dataAdapters;

    public static cDatabaseManager instance;
    public static bool connected;
    bool beta;
    /* Constructor */

    public cDatabaseManager()
    {
        instance = this;
        connected = false;
        beta = true;
    }

    /* Exports */

    public DataTable executeQueryWithResultNew(string sql)
    {
        using (MySqlConnection conn = new MySqlConnection(connStr))
        {
            try
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader rdr = cmd.ExecuteReader();
                DataTable results = new DataTable();
                results.Load(rdr);
                rdr.Close();
                //conn.Close();
                return results;
            }
            catch (Exception ex)
            {

                //conn.Close();
                return null;
            }
        }
    }

    public bool isReady()
    {
        return (connected);
    }

    public DataTable executePreparedQueryWithResultNew(string sql, Dictionary<string, string> parameters)
    {
        using (MySqlConnection conn = new MySqlConnection(connStr))
        {
            try
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(sql, conn);


                foreach (KeyValuePair<string, string> entry in parameters)
                {
                    cmd.Parameters.AddWithValue(entry.Key, entry.Value);
                }

                MySqlDataReader rdr = cmd.ExecuteReader();
                DataTable results = new DataTable();
                results.Load(rdr);
                rdr.Close();
                //conn.Close();
                return results;
            }
            catch (Exception ex)
            {
                DataTable results = new DataTable();
                //conn.Close();
                return results;
            }
        }
    }

    public long ExecutePreparedQueryWithLastInsertResult(string sql, Dictionary<string, object> parameters)
    {
        using (MySqlConnection conn = new MySqlConnection(connStr))
        {
            try
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(sql, conn);


                foreach (KeyValuePair<string, object> entry in parameters)
                {
                    cmd.Parameters.AddWithValue(entry.Key, entry.Value);
                }

                cmd.ExecuteNonQuery();
                return cmd.LastInsertedId;
            }
            catch (Exception ex)
            {
                return -1;
            }
        }
    }

    public DataTable executePreparedQueryWithResultNew(string sql, Dictionary<string, object> parameters)
    {
        using (MySqlConnection conn = new MySqlConnection(connStr))
        {
            try
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(sql, conn);


                foreach (KeyValuePair<string, object> entry in parameters)
                {
                    cmd.Parameters.AddWithValue(entry.Key, entry.Value);
                }

                MySqlDataReader rdr = cmd.ExecuteReader();
                DataTable results = new DataTable();
                results.Load(rdr);
                rdr.Close();
                //conn.Close();
                return results;
            }
            catch (Exception ex)
            {
                DataTable results = new DataTable();
                //conn.Close();
                return results;
            }
        }
    }

    public async void executeQueryNew(string sql)
    {
        await System.Threading.Tasks.Task.Run(() =>
        {
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                try
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                }
                //conn.Close();
            }
        });//.ConfigureAwait(false);
    }

    public async void executePreparedSyncQueryNew(string sql, Dictionary<string, string> parameters)
    {
        await System.Threading.Tasks.Task.Run(() =>
        {
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                try
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    foreach (KeyValuePair<string, string> entry in parameters)
                    {
                        cmd.Parameters.AddWithValue(entry.Key, entry.Value);
                    }
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                }
                //conn.Close();
            }
        });//.ConfigureAwait(false);
    }

    public async void executePreparedQueryNew(string sql, Dictionary<string, string> parameters)
    {
        await System.Threading.Tasks.Task.Run(() =>
        {
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                try
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    foreach (KeyValuePair<string, string> entry in parameters)
                    {
                        cmd.Parameters.AddWithValue(entry.Key, entry.Value);
                    }
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                }
                //conn.Close();
            }
        });//.ConfigureAwait(false);
    }


    public async void executePreparedQueryNew(string sql, Dictionary<string, object> parameters)
    {
        await System.Threading.Tasks.Task.Run(() =>
        {
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                try
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    foreach (KeyValuePair<string, object> entry in parameters)
                    {
                        cmd.Parameters.AddWithValue(entry.Key, entry.Value);
                    }
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                }
                //conn.Close();
            }
        });//.ConfigureAwait(false);
    }

    public DataTable createDataTable(string sql, string unique_name)
    {
        using (MySqlConnection conn = new MySqlConnection(connStr))
        {
            try
            {
                MySqlDataAdapter dataAdapter;
                DataTable dataTable;
                dataAdapter = new MySqlDataAdapter(sql, conn);
                MySqlCommandBuilder cb = new MySqlCommandBuilder(dataAdapter);
                dataAdapters[unique_name] = dataAdapter;
                dataTable = new DataTable();
                dataAdapter.Fill(dataTable);
                //conn.Close();
                return dataTable;
            }
            catch (Exception ex)
            {
                //conn.Close();
                return null;
            }
        }
    }
    
    
    public void onResourceStart()
    {
        dataAdapters = new Dictionary<string, MySqlDataAdapter>();

        connStr =
            "server=127.0.0.1;user=root;database=test;port=3306;password=;min pool size=5;max pool size=40;SslMode=none;charset=utf8mb4;";

        using (MySqlConnection conn = new MySqlConnection(connStr))
        {
            try
            {
                connected = false;
                conn.Open();

                if (conn.State == ConnectionState.Open)
                {
                    connected = true;
                    startServerMySQLConnection();
                }
            }
            catch (Exception ex)
            {
                connected = false;
            }

        }
    }
    
    
    
    private void startServerMySQLConnection()
    {
        Program.Example();
    }

}