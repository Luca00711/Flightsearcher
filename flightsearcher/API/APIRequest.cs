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
        public static async Task<List<Flight>> Request(string url)
        {
            List<Flight> flights = new List<Flight>();
            try
            {
                var response = await url.WithHeaders(Utils.Utils.GetHeaders()).GetJsonAsync();
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
                            Console.WriteLine("One Added");
                            dynamic flight = new ExpandoObject();
                            flight.flightduration = flightTime;
                            flight.fnac = $"{rowlist?[13]} | {rowlist?[16]}";
                            flight.departure = depart.icao;
                            flight.arrival = arrival.icao;
                            flight.aircraft = rowlist?[8];
                            flight.registration = rowlist?[9];
                            Database db = new Database();
                            db.Query($"INSERT INTO flights (aircraft, fnac, depart, arrival, fd) VALUES ('{flight.aircraft}', '{flight.fnac}', '{flight.departure}', '{flight.arrival}', '{flight.flightduration}')");
                            flights.Add(JsonConvert.DeserializeObject<Flight>(JsonConvert.SerializeObject(flight)));
                        }
                    }
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