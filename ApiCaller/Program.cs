using System;
using System.Net.Http;
using System.Web;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Configuration;

namespace ApiCaller
{
    public class Program
    {
        public class TwitterResponse
        {
            //For getting bearer -- Make seperate Response Object?
            public string token_type { get; set; }
            public string access_token { get; set; }

            //Twitter Data
            public string name { get; set; }
            public string followers_count { get; set; }
            public string created_at { get; set; }
            public string profile_image_url { get; set; }
            public string url { get; set; }
            public string friends_count { get; set; }
            public string screen_name { get; set; }
        }

        public class FacebookResponse
        {
            public string likes { get; set; }
            public string id { get; set; }
        }

        public static string Base64Encode(string stringText)
        {
            var stringTextBytes = System.Text.Encoding.UTF8.GetBytes(stringText);
            return System.Convert.ToBase64String(stringTextBytes);
        }

        public static void Main(string[] args)
        {
            List<string> candidates = new List<string>();
            candidates.Add("25073877");
            candidates.Add("15745368");
            var resp = CallTwitterAsync(candidates).Result;
            Console.WriteLine(resp[0].followers_count);
            Console.WriteLine(resp[1].followers_count);       
        }

        static async Task<List<TwitterResponse>> CallTwitterAsync(List<string> twitterIDs)
        {

            string IDs = string.Join(",", twitterIDs);

            using (var client = new HttpClient())
            {
                string token = ConfigurationManager.AppSettings["TwitterBearer"];
                string bearerToken = "Bearer " + token;
     
                //TryAdd... skips validation that can throw error false-posititve
                client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", bearerToken);
                client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "polagora");

                //GET Request
                string uri = "https://api.twitter.com/1.1/users/lookup.json?user_id=";
                var response = await client.GetAsync(uri + IDs);
                var responseContent = await response.Content.ReadAsStringAsync();
               
                //Deserialize into list of response objects
                JavaScriptSerializer ser = new JavaScriptSerializer();
                return ser.Deserialize<List<TwitterResponse>>(responseContent);
            }
        }

        public string GetBearer()
        {

            string CONSUMER_KEY = ConfigurationManager.AppSettings["TwitterKey"];
            string CONSUMER_SECRET = ConfigurationManager.AppSettings["TwitterSecret"];
            GetBearerAsync(CONSUMER_KEY, CONSUMER_SECRET).Wait();
            string responseString = GetBearerAsync(CONSUMER_KEY, CONSUMER_SECRET).Result;
            
            //Deserialize response JSON
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            TwitterResponse response = serializer.Deserialize<TwitterResponse>(responseString);
            return response.access_token;
        }

        static async Task<FacebookResponse> CallFacebookAsync(string facebookID)
        {
            using (var client = new HttpClient())
            {
                string uri = "https://graph.facebook.com/v2.5/";
                string id = facebookID;
                string param = "?fields=likes&access_token=";
                string token = ConfigurationManager.AppSettings["FacebookToken"];
                string url = uri + id + param + token;

                //GET
                var response = await client.GetAsync(url);
                var payload = await response.Content.ReadAsStringAsync();

                //Deserialize
                JavaScriptSerializer ser = new JavaScriptSerializer();
                FacebookResponse FacebookResponse = ser.Deserialize<FacebookResponse>(payload);

                return FacebookResponse;
            }
        }

        static async Task<string> GetBearerAsync(string ApiKey, string ApiSecret)
        {
            using (var client = new HttpClient())
            {
		        //keys
                string key = ConfigurationManager.AppSettings["TwitterKey"];
                string secret = ConfigurationManager.AppSettings["TwitterSecret"];
		
		        string uri = "https://api.twitter.com/oauth2/token";
		        string contentType = "application/x-www-form-urlencoded;charset=UTF-8";

                //URL encoding
                string keyEncoded = HttpUtility.UrlEncode(key);
                string secretEncoded = HttpUtility.UrlEncode(secret);

                //Base64 Encode Token Credentials
                string keySecretEncoded = Base64Encode(keyEncoded + ":" + secretEncoded);
		        string credentials = ("Basic " + keySecretEncoded);

                //Set headers, skip validation
                client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", credentials);
                client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", contentType);
                
                //Post Body
                var values = new Dictionary<string, string> { { "grant_type", "client_credentials" } };
                var body = new FormUrlEncodedContent(values);

                //POST
                var response = await client.PostAsync(uri, body);
                var content = await response.Content.ReadAsStringAsync();
                
                return content;
            }
        }
    }
}

