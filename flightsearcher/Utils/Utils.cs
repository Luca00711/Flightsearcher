using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Eto.Drawing;
using Flightsearcher.Models;
using Flurl.Http;
using Newtonsoft.Json;

namespace Flightsearcher.Utils
{
    public class Utils
    {
        static double ConvertToRadians(double angle)
        {
            return (Math.PI / 180) * angle;
        }

        public static TimeSpan GetFlightDuration(Airport depart, Airport arrival)
        {
            //var calculated = await $"https://flighttime-calculator.com/calculate?lat1={depart.lat.ToString(CultureInfo.InvariantCulture)}&lng1={depart.lon.ToString(CultureInfo.InvariantCulture)}&lat2={arrival.lat.ToString(CultureInfo.InvariantCulture)}&lng2={arrival.lon.ToString(CultureInfo.InvariantCulture)}&departure_datetime=06/06/2022+10:49+PM".GetJsonAsync();
            // The radius of the earth in Km.
            // You could also use a better estimation of the radius of the earth
            // using decimals digits, but you have to change then the int to double.
            double latitude1 = depart.lat;
            double latitude2 = arrival.lat;
            double longitude1 = depart.lon;
            double longitude2 = arrival.lon;
            int R = 6371;

            double f1 = ConvertToRadians(latitude1);
            double f2 = ConvertToRadians(latitude2);

            double df = ConvertToRadians(latitude1 - latitude2);
            double dl = ConvertToRadians(longitude1 - longitude2);

            double a = Math.Sin(df / 2) * Math.Sin(df / 2) +
                       Math.Cos(f1) * Math.Cos(f2) *
                       Math.Sin(dl / 2) * Math.Sin(dl / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            // Calculate the distance.
            double d = R * c;

            d = d / 446.1;


            return TimeSpan.FromSeconds(Math.Round(TimeSpan.FromHours(d).TotalSeconds));
        }

        public static async Task<List<Airport>> GetAirports()
        {
            var response = await "https://www.flightradar24.com/_json/airports.php".GetJsonAsync();
            return JsonConvert.DeserializeObject<List<Airport>>(JsonConvert.SerializeObject(response.rows));
        }

        public static async Task<List<Airline>> GetAirlines()
        {
            var response = await "https://www.flightradar24.com/_json/airlines.php".GetJsonAsync();
            List<Airline> airlines =
                JsonConvert.DeserializeObject<List<Airline>>(JsonConvert.SerializeObject(response.rows));
            return airlines;
        }

        public static async Task<Image> GetPhoto(string registration)
        {
            Livery photo = await $"https://api.planespotters.net/pub/photos/reg/{registration}"
                .WithHeader("User-Agent", "Other").GetJsonAsync<Livery>();
            var response = await $"{photo.photos[0].thumbnail_large.src}".GetBytesAsync();
            return new Bitmap(response);
        }

        public static Dictionary<string, string> GetHeaders()
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
                {
                    "user-agent",
                    "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.88 Safari/537.36"
                },
                {"accept", "application/json"}
            };
            return headers;
        }
    }
}