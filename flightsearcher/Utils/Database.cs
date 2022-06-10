using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using Flightsearcher.Models;

namespace Flightsearcher.Utils;

public class Database
{
    public Database()
    {
        Console.WriteLine(Environment.OSVersion.Platform);
        string path = "";
        string seperator = RuntimeInformation.OSDescription.Contains("Windows") ? "\\" : "/";
        foreach (var sub in AppDomain.CurrentDomain.BaseDirectory.Split(seperator))
        {
            if (RuntimeInformation.OSDescription.Contains("Windows"))
            {
                if (sub == "Flightsearcher.Wpf") break;
            } 
            else if (RuntimeInformation.OSDescription.Contains("Darwin"))
            {
                if (sub == "Flightsearcher.Mac") break;
            }
            else
            {
                if (sub == "Flightsearcher.Gtk") break;
            }
            path += sub + seperator;
        }

        path += $"Flightsearcher{seperator}flights.db";
        DbConnection = new SQLiteConnection($"Data Source={path};Version=3;");
    }

    SQLiteConnection DbConnection { get; set; }

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