using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;
using flightsearcher.Models;

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
    
    public List<Flight> GetQuery(string query)
    {
        List<Flight> flights = new List<Flight>();
        try
        {
            DbConnection.Open();
            Console.WriteLine(DbConnection.FileName);
            SQLiteCommand command = new SQLiteCommand(query, DbConnection);
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                Flight flight = new Flight();
                flight.flightduration = reader.GetString(4);
                flight.fnac = reader.GetString(1);
                flight.departure = reader.GetString(2);
                flight.arrival = reader.GetString(3);
                flight.aircraft = reader.GetString(0);
                flight.registration = reader.GetString(5);
                flights.Add(flight);
            }
            DbConnection.Close();
            return flights;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            DbConnection.Close();
            return flights;
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