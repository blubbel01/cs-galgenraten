using System.Collections.Generic;
using System.Data;

public class cDatabase {
    //static attributes
    private static cDatabase instance;
    public static cDatabase Instance{
        get{
            if (instance == null){
                new cDatabase();
            }
            return instance;
        }
    }


    //private attributes	

    //public attributes

    //constructor
    public cDatabase() {
        instance = this;
	}

    //destructor


    //private methods

    //public methods
    public void simpleExecute(string sql) {
        cDatabaseManager.instance.executeQueryNew(sql);
    }

    public void executePreparedQuery(string sql, Dictionary<string, string> parameters)
    {
        cDatabaseManager.instance.executePreparedQueryNew(sql, parameters);
    }

    public void executePreparedQuery(string sql, Dictionary<string, object> parameters) {
        cDatabaseManager.instance.executePreparedQueryNew(sql, parameters);
	}

    public void executePreparedSyncQuery(string sql, Dictionary<string, string> parameters)
    {
        cDatabaseManager.instance.executePreparedSyncQueryNew(sql, parameters);
    }

    public DataTable simpleQuery(string sql) {
		return cDatabaseManager.instance.executeQueryWithResultNew(sql);
	}

    public long ExecutePreparedQueryWithLastInsertResult(string sql, Dictionary<string, object> parameters)
    {
        return cDatabaseManager.instance.ExecutePreparedQueryWithLastInsertResult(sql, parameters);
    }

    public DataTable ExecutePreparedQueryWithResult(string sql, Dictionary<string,object> parameters) {
		return cDatabaseManager.instance.executePreparedQueryWithResultNew(sql, parameters);
	}

    public DataTable ExecutePreparedQueryWithResult(string sql, Dictionary<string, string> parameters)
    {
        return cDatabaseManager.instance.executePreparedQueryWithResultNew(sql, parameters);
    }

    public DataTable createDataTable(string sql, string name) {
		return  cDatabaseManager.instance.createDataTable(sql, name);
	}
	
	//GETTERS
	
	//SETTERS

}