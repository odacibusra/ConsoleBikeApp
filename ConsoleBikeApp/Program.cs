using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ConsoleBikeApp
{
    class Program
    {
        public async static Task CallBikeIndexApi(int cityId)
        {
            var cityName = GetCityName(cityId);

            if (cityName == "default")
            {
                Console.WriteLine("There is no city found with the given number!!");
            }
            else
            {
                string uri = "https://bikeindex.org:443/api/v3/search/count?location={0}&stolenness=stolen";
                string formattedUri = string.Format(uri, cityName);

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(formattedUri);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = client.GetAsync(formattedUri).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        CountResult countModel = JsonConvert.DeserializeObject<CountResult>(JObject.Parse(result).ToString());

                        Console.WriteLine($"Stolen bike count for {cityName.ToUpper()} : {countModel.proximity}");
                    }
                    else
                    {
                        Console.WriteLine("Internal server Error");
                    }
                }
            }
        }
        private static string GetCityName(int cityId)
        {
            var map = new Dictionary<int, string>()
            {
                {1, "amsterdam"},
                {2, "berlin"},
                {3, "copenhagen"},
                {4, "brussels"},
                {5, "milan"},
                {6, "london"},
                {7, "paris"},
            };
            string output;
            return map.TryGetValue(cityId, out output) ? output : "default";
        }
        static void Main(string[] args)
        {
            Console.WriteLine("**The cities we currently operate** \n 1-Amsterdam, The Netherlands \n 2-Berlin, Germany \n 3-Copenhagen, Denmark \n 4-Brussels, Belgium \n");
            Console.WriteLine("**The cities we want to expand in the next year** \n 5-Milan, Italy \n 6-London, UK \n 7-Paris, France \n \n");
            Console.WriteLine("Enter the city number which you want to see the stolen bike count");
            try
            {
                var cityId = Convert.ToInt32(Console.ReadLine());
                CallBikeIndexApi(cityId).Wait();
            }
            catch (Exception)
            {
                Console.WriteLine("Only numbers allowed!!");
            }
        }
    }
}
