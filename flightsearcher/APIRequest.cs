using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Flurl.Http;
using Flurl.Util;
using Newtonsoft;
using Newtonsoft.Json;

namespace flightsearcher
{
    
    public class APIRequest
    {
        public async Task<string> Request(string url)
        {
            try
            {
                var response = await url.GetJsonAsync();
                int i = 0;
                foreach (KeyValuePair<string, object> row in response)
                {
                    if (row.Value is List<object>)
                    {
                        i++;
                    }
                }
                Console.WriteLine(i);
                return "list";
            }
            catch (FlurlHttpException e)
            {
                //Console.WriteLine(e.Message);
                return null;
            }
        }
        public static DateTime UnixTimeStampToDateTime( double unixTimeStamp )
        {
            // Unix timestamp is seconds past epoch
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds( unixTimeStamp ).ToLocalTime();
            return dateTime;
        }
    }
}