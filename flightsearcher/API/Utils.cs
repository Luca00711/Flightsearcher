using System;
using System.Threading.Tasks;
using Eto.Drawing;
using flightsearcher.Models;
using Flurl.Http;

namespace flightsearcher.API
{
    public class Utils
    {
        public static TimeSpan GetFlightDuration(Int64 unixDeparture, Int64 unixArrival)
        {
            DateTime dateTimeDepature = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTimeDepature = dateTimeDepature.AddSeconds(unixDeparture).ToLocalTime();
            DateTime dateTimeArrival = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTimeArrival = dateTimeArrival.AddSeconds(unixArrival).ToLocalTime();
            
            TimeSpan duration = dateTimeArrival - dateTimeDepature;
            return duration;
        }

        public static async Task<Image> GetPhoto(string registration)
        {
            Livery photo = await $"https://api.planespotters.net/pub/photos/reg/{registration}".WithHeader("User-Agent", "Other").GetJsonAsync<Livery>();
            var response = await $"{photo.photos[0].thumbnail_large.src}".GetBytesAsync();
            return new Bitmap(response);
        }
    }
}