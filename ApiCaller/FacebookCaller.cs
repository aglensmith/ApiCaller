using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace ApiCaller
{
    public class FacebookCaller : Caller

    {
        public static async Task<Dictionary<string, FacebookResponse>> 
            CallFacebookAsync(List<string> FacebookIDs, string Token)
        {
            using (var client = new HttpClient())
            {
                string uri = "https://graph.facebook.com/v2.5/?ids=";
                string IDs = string.Join(",", FacebookIDs);
                string param = "&fields=likes&access_token=";

                string url = uri + IDs + param + Token;

                //GET
                var response = await client.GetAsync(url);
                var payload = await response.Content.ReadAsStringAsync();

                Console.WriteLine(response);
                Console.WriteLine(payload);

                //Deserialize
                JavaScriptSerializer Serializer = new JavaScriptSerializer();

                return Serializer.Deserialize<Dictionary<string, FacebookResponse>>(payload);            
            }
        }

        public class FacebookResponse
        {
            public string likes { get; set; }
            public string id { get; set; }
        }
    }
}
