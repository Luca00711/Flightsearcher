using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;
using flightsearcher.Models;
using flightsearcher.Utils;
using Flurl.Http;
using Newtonsoft.Json;

namespace flightsearcher.API
{
    public class APIRequest
    {
        public static async Task<List<Flight>> Request(string url, string airline)
        {
            List<Flight> flights = new List<Flight>();
            try
            {
                var response = await url.WithHeaders(Utils.Utils.GetHeaders()).GetJsonAsync();
                Database db = new Database();
                foreach(KeyValuePair<string, object> row in response)
                {
                    if(row.Value is List<object>)
                    {
                        List<object> rowlist = row.Value as List<object>;
                        if (rowlist?[11].ToString() == "" || rowlist?[12].ToString() == "") { continue; }
                        var depart = await Utils.Utils.GetAirport(rowlist?[11].ToString());
                        var arrival = await Utils.Utils.GetAirport(rowlist?[12].ToString());
                        TimeSpan flightTime = await Utils.Utils.GetFlightDuration(depart, arrival);
                        Console.WriteLine(flightTime);
                        if (flightTime <= new TimeSpan(1, 10, 0))
                        {
                            Console.WriteLine("One added");
                            dynamic flight = new ExpandoObject();
                            flight.flightduration = flightTime;
                            flight.fnac = $"{rowlist?[13]} | {rowlist?[16]}";
                            flight.departure = depart.icao;
                            flight.arrival = arrival.icao;
                            flight.aircraft = rowlist?[8];
                            flight.registration = rowlist?[9];
                            flight.airline = rowlist?[18];
                            db.Query($"INSERT INTO flights (aircraft, fnac, depart, arrival, fd, registration, airline) VALUES ('{flight.aircraft}', '{flight.fnac}', '{flight.departure}', '{flight.arrival}', '{flight.flightduration}', '{flight.registration}', '{flight.airline}')");
                            flights.Add(JsonConvert.DeserializeObject<Flight>(JsonConvert.SerializeObject(flight)));
                        }
                    }
                }
                List<Flight> databaselist = db.GetQuery($"SELECT aircraft, fnac, depart, arrival, fd, registration FROM flights WHERE airline = '{airline}'");
                foreach (Flight flight in databaselist)
                {
                    Console.WriteLine(flight.registration);
                }
                if (databaselist.Count == 0) {return flights;}
                foreach (var flight in databaselist)
                {
                    if (flights.Find(x => x.fnac == flight.fnac) != null) { continue; }
                    Console.WriteLine("One added from database");
                    flights.Add(flight);
                }
                return flights;
            }
            catch (FlurlHttpException e)
            {
                Console.WriteLine(e.Message);
                return flights;
            }
        }
    }
}