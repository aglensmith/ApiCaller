using System.Text;

namespace ApiCaller
{
    public class Caller
    {
        public static string Base64Encode(string stringText)
        {
            var stringTextBytes = Encoding.UTF8.GetBytes(stringText);
            return System.Convert.ToBase64String(stringTextBytes);
        }
    }
}
