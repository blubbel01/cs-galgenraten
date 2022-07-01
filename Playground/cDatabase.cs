using System.Collections.Generic;
using System.Data;

namespace Database
{
    namespace Syncronisation
    {
        public static class Query
        {
            public static void Execute(string sql, Dictionary<string, string> parameters)
            {
                cDatabaseManager.instance.executePreparedSyncQueryNew(sql, parameters);
            }
        }
    }

    public static class Query
    {
        public static void Execute(string sql)
        {
            cDatabaseManager.instance.executeQueryNew(sql);
        }
        public static void Execute(string sql, Dictionary<string, string> parameters)
        {
            cDatabaseManager.instance.executePreparedQueryNew(sql, parameters);
        }
        public static void Execute(string sql, Dictionary<string, object> parameters)
        {
            cDatabaseManager.instance.executePreparedQueryNew(sql, parameters);
        }

        public static long ExecuteWithLastInsertedId(string sql, Dictionary<string, string> parameters)
        {
            return cDatabaseManager.instance.ExecutePreparedQueryWithLastInsertResult(sql, parameters);
        }

        public static long ExecuteWithLastInsertedId(string sql, Dictionary<string, object> parameters)
        {
            return cDatabaseManager.instance.ExecutePreparedQueryWithLastInsertResult(sql, parameters);
        }

        public static DataTable ExecuteWithResult(string sql)
        {
            return cDatabaseManager.instance.executeQueryWithResultNew(sql);
        }

        public static DataTable ExecuteWithResult(string sql, Dictionary<string, object> parameters)
        {
            return cDatabaseManager.instance.executePreparedQueryWithResultNew(sql, parameters);
        }

        public static DataTable ExecuteWithResult(string sql, Dictionary<string, string> parameters)
        {
            return cDatabaseManager.instance.executePreparedQueryWithResultNew(sql, parameters);
        }
    }
}