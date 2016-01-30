using System.Net.Http;
using System.Web;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Configuration;

namespace ApiCaller
{
    public class TwitterCaller : Caller
    {
        public static async Task<List<TwitterResponse>> CallTwitterAsync(List<string> twitterIDs, string Token)
        {
            string BearerToken = "Bearer " + Token;

            string IDs = string.Join(",", twitterIDs);

            string URI = "https://api.twitter.com/1.1/users/lookup.json?user_id=" + IDs;

            using (var client = new HttpClient())
            {    
                //TryAdd... skips validation that can throw error false-posititve
                client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", BearerToken);
                client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "polagora");

                //GET Request
                var Response = await client.GetAsync(URI);
                var ResponseContent = await Response.Content.ReadAsStringAsync();

                //Deserialize into list of Response objects
                JavaScriptSerializer Serializer = new JavaScriptSerializer();
                return Serializer.Deserialize<List<TwitterResponse>>(ResponseContent);
            }
        }

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
            public string id_str { get; set; }
        }

        public string GetBearer(string ConsumerKey, string ConsumerSecret)
        {
            //GetBearerAsync(ConsumerKey, ConsumerSecret).Wait();
            string ResponseString = GetBearerAsync(ConsumerKey, ConsumerSecret).Result;

            //Deserialize Response JSON
            JavaScriptSerializer Serializer = new JavaScriptSerializer();
            TwitterResponse Response = Serializer.Deserialize<TwitterResponse>(ResponseString);

            return Response.access_token;
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
