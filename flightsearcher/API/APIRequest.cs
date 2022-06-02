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
            try
            {
                List<Flight> flights = new List<Flight>();
                var response = await url.GetJsonAsync();
                foreach(KeyValuePair<string, object> row in response)
                {
                    if(row.Value is List<object>)
                    {
                        var responsetest = await $"https://data-live.flightradar24.com/clickhandler/?flight={row.Key}"
                            .GetJsonAsync();
                       if (responsetest.status.live != false)
                       {
                           TimeSpan flightDuration = Utils.GetFlightDuration(responsetest.time.scheduled.departure, responsetest.time.scheduled.arrival);
                            if (flightDuration <= new TimeSpan(1,0,0))
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
                Console.WriteLine(e.Message);
                return null;
            }
        }
    }
}