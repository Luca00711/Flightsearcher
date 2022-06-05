using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using flightsearcher.Models;
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
                var response = await url.WithHeader("User-Agent", "Other").GetJsonAsync();
                foreach(KeyValuePair<string, object> row in response)
                {
                    if(row.Value is List<object>)
                    {
                        var responsetest = await $"https://data-live.flightradar24.com/clickhandler/?flight={row.Key}"
                            .WithHeader("User-Agent", "Other").GetJsonAsync();
                       if (responsetest.status.live != false)
                       {
                           TimeSpan flightDuration = Utils.GetFlightDuration(responsetest.time.scheduled.departure, responsetest.time.scheduled.arrival);
                            if (flightDuration <= new TimeSpan(1,50,0))
                            {
                                Console.WriteLine("One Added");
                                responsetest.flightduration = flightDuration;
                                responsetest.fnac = $"{responsetest.identification.number.@default} | {responsetest.identification.callsign}";
                                flights.Add(JsonConvert.DeserializeObject<Flight>(JsonConvert.SerializeObject(responsetest)));
                            }
                        }
                    }
                }
                return flights;
            }
            catch (FlurlHttpException e)
            {
                return flights;
            }
        }
    }
}