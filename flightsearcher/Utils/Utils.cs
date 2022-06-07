using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Eto.Drawing;
using flightsearcher.Models;
using Flurl.Http;
using Newtonsoft.Json;

namespace flightsearcher.Utils
{
    public class Utils
    { 
        public static async Task<TimeSpan> GetFlightDuration(Airport depart, Airport arrival)
        {
            var calculated = await $"https://flighttime-calculator.com/calculate?lat1={depart.lat.ToString(CultureInfo.InvariantCulture)}&lng1={depart.lon.ToString(CultureInfo.InvariantCulture)}&lat2={arrival.lat.ToString(CultureInfo.InvariantCulture)}&lng2={arrival.lon.ToString(CultureInfo.InvariantCulture)}&departure_datetime=06/06/2022+10:49+PM".GetJsonAsync();
            return ConvertToTimeSpan(calculated.flight_time);
        }
        
        public static TimeSpan ConvertToTimeSpan(string input)
        {
            var units = new Dictionary<string, int>()
            {
                {@"(\d+)(?:ms|mili(?:secon)?s?)", 1 },
                {@"(\d+)(?:s(?:ec)?|seconds?)", 1000 },
                {@"(\d+)(?:m|mins?)", 60000 },
                {@"(\d+)(?:h|hours?)", 3600000 },
                {@"(\d+)(?:d|days?)", 86400000 },
                {@"(\d+)(?:w|weeks?)", 604800000 },
            };
            var timespan = new TimeSpan();
            foreach(var x in units)
            {
                var matches = Regex.Matches(input, x.Key);
                foreach(Match match in matches)
                {
                    var amount = Convert.ToInt32(match.Groups[1].Value);
                    timespan = timespan.Add(TimeSpan.FromMilliseconds(x.Value * amount));
                }
            }
            return timespan;
        }
        
        public static async Task<Airport> GetAirport(string iata)
        {
            var response = await "https://www.flightradar24.com/_json/airports.php".GetJsonAsync();
            List<Airport> icao = JsonConvert.DeserializeObject<List<Airport>>(JsonConvert.SerializeObject(response.rows));
            return icao.Find(x => x.iata == iata);
        }

        public static async Task<Image> GetPhoto(string registration)
        {
            Livery photo = await $"https://api.planespotters.net/pub/photos/reg/{registration}".WithHeader("User-Agent", "Other").GetJsonAsync<Livery>();
            var response = await $"{photo.photos[0].thumbnail_large.src}".GetBytesAsync();
            return new Bitmap(response);
        }

        public static Dictionary<string,string> GetHeaders()
        {
            var headers = new Dictionary<string, string>
            {
                {"accept-encoding", "gzip, deflate"},
                {"accept-language", "pt-BR,pt;q=0.9,en-US;q=0.8,en;q=0.7"},
                {"cache-control", "max-age=0"},
                {"origin", "https://www.flightradar24.com"},
                {"referer", "https://www.flightradar24.com/"},
                {"sec-fetch-dest", "empty"},
                {"sec-fetch-mode", "cors"},
                {"sec-fetch-site", "same-site"},
                {"user-agent", "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.88 Safari/537.36"},
                {"accept", "application/json"}
            };
            return headers;
        }
        
    }
}