using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Flurl.Http;
using Newtonsoft;
using Newtonsoft.Json;

namespace flightsearcher
{
    
    public class APIRequest
    {
        public async Task<List<Class>> Request<Class>(string url)
        {
            try
            {
                List<Class> list = new List<Class>();
                var response = await url.GetJsonAsync();
                int i = 0;
                foreach (var row in response.rows)
                {
                    if (i > 20)
                    {
                        break;
                    }
                    list.Add(JsonConvert.DeserializeObject<Class>(JsonConvert.SerializeObject(row)));
                    i++;
                }
                return list;
            }
            catch (FlurlHttpException e)
            {
                //Console.WriteLine(e.Message);
                return null;
            }
        }        
    }
}