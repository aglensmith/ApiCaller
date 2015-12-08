using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Configuration;

namespace ApiCaller
{
    public class Program
    {
        public class TwitterResponse
        {
            public string token_type { get; set; }
            public string access_token { get; set; }
            public string followers_count { get; set; }
        }

        public static string Base64Encode(string stringText)
        {
            var stringTextBytes = System.Text.Encoding.UTF8.GetBytes(stringText);
            return System.Convert.ToBase64String(stringTextBytes);
        }

        public static void Main(string[] args)
        {

            //For loop that iteratres over list of usernames?
            // TwitterResponse rubio_response = GetTwitterDataAsync("marcorubio").Result;
            //  Console.WriteLine(rubio_response.followers_count);

            List<string> screenNames = new List<string>();
            screenNames.Add("marcorubio");
            screenNames.Add("tedcruz");

            string bearerToken = ConfigurationManager.AppSettings["TwitterBearer"];

            List<TwitterResponse> twitterResponse = CallTwitter(screenNames, bearerToken);

            foreach(TwitterResponse response in twitterResponse)
            {
                Console.WriteLine(response.followers_count);
            }
        }
        static List<TwitterResponse> CallTwitter(List<string> screenNames, string beaerToken)
        {
            List<TwitterResponse> twitterResponses = new List<TwitterResponse>();

            foreach (string screenName in screenNames)
            {
                TwitterResponse response = CallTwitterAsync(screenName).Result;
                twitterResponses.Add(response);
            }

            return twitterResponses;
        }
        static async Task<TwitterResponse> CallTwitterAsync(string screen_name)
        {
     
            using (var client = new HttpClient())
            {
                string token = ConfigurationManager.AppSettings["TwitterBearer"];
                string bearerToken = "Bearer " + token;
     
                //TryAddWithoutValidation skips validation that can throw error false-posititve
                client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", bearerToken);
                client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "polagora");

                //GET Request
                string uri = "https://api.twitter.com/1.1/users/show.json?screen_name=";
                var response = await client.GetAsync(uri + screen_name);
                var responseContent = await response.Content.ReadAsStringAsync();

                //Deserialize into response object
                JavaScriptSerializer ser = new JavaScriptSerializer();
                TwitterResponse TwitterResponse = ser.Deserialize<TwitterResponse>(responseContent);

                Console.WriteLine(responseContent);
              
                return TwitterResponse;
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

        static async Task<string> GetBearerAsync(string CONSUMER_KEY, string CONSUMER_SECRET)
        {
            using (var client = new HttpClient())
            {

                //keys
                string key = "DwSw7pBWrpLqCeSxWhMmAwezP";
                string secret = "or6CkRutVNAPD9WWBcZCC2ZSjaCxg4YqdOkIrXbmPtmpUU5262";

                //URL encoding
                string keyEncoded = HttpUtility.UrlEncode(key);
                string secretEncoded = HttpUtility.UrlEncode(secret);

                //Base64 Encoded Token Credentials
                string tokenCredentials = Base64Encode(keyEncoded + ":" + secretEncoded);

                //Set headers, skip stupid validation
                client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "Basic " + tokenCredentials);
                client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/x-www-form-urlencoded;charset=UTF-8");
                
                //Post Body
                var values = new Dictionary<string, string> { { "grant_type", "client_credentials" } };
                var content = new FormUrlEncodedContent(values);

                //POST
                var response = await client.PostAsync("https://api.twitter.com/oauth2/token", content);
                var responseContent = await response.Content.ReadAsStringAsync();
                
                return responseContent;

            }

            
        }
    }
}

