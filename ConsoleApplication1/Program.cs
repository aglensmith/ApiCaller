using System;
using System.Collections.Generic;
using ApiCaller;
using System.Configuration;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> Candidates = new List<string>();
            Candidates.Add("25073877");
            Candidates.Add("15745368");
            var BearerToken = ConfigurationManager.AppSettings["TwitterBearer"];
            var resp = TwitterCaller.CallTwitterAsync(Candidates, BearerToken).Result;
            Console.WriteLine(resp[0].followers_count);
            Console.WriteLine(resp[1].followers_count);

            List<string> FBCandidates = new List<string>();
            FBCandidates.Add("153080620724");
            FBCandidates.Add("138691142964027");
            string Token = ConfigurationManager.AppSettings["FacebookToken"];
            var fbresponse = FacebookCaller.CallFacebookAsync(FBCandidates, Token).Result;

            Console.WriteLine(fbresponse["153080620724"].likes);
            //Console.WriteLine(fbresponse[1].likes);
        }
    }
}
