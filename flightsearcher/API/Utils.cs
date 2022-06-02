using System;

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
    }
}