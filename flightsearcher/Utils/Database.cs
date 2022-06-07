using System;
using System.Data.SQLite;

namespace flightsearcher.Utils;

public class Database
{ 
    SQLiteConnection DbConnection { get; set; }
    
    public Database()
    {
        DbConnection = new SQLiteConnection("Data Source='H:/Programmieren/c-sharp/flightsearcher/flightsearcher/flights.db';Version=3;");
    }
    
    public void Query(string query)
    {
        try
        {
            DbConnection.Open();
            Console.WriteLine(DbConnection.FileName);
            SQLiteCommand command = new SQLiteCommand(query, DbConnection);
            command.ExecuteNonQuery();
            DbConnection.Close();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            DbConnection.Close();
        }
    }

    public void Test()
    {
        try
        {
            DbConnection.Open();
            DbConnection.Close();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            DbConnection.Close();
        }
        
    }
}