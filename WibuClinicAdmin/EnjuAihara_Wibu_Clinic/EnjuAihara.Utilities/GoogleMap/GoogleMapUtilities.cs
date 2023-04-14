using EnjuAihara.ViewModels.GoogleMap;
using EnjuAihara.ViewModels.MasterData;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using System.Web.Configuration;

namespace EnjuAihara.Utilities.GoogleMap
{
    public static class GoogleMapUtilities
    {
        public static HttpClient GetHttpClient()
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/112.0.0.0 Safari/537.36");
            return client;
        }


        public static Coordinate GetCoordinate(string Address)
        {
            try
            {
                using (var client = GetHttpClient())
                {
                    var EncodeString = HttpUtility.UrlEncode(Address).ToString();
                    string GeoUrl = WebConfigurationManager.AppSettings["GeocoderUrl"];
                    string key = WebConfigurationManager.AppSettings["GoogleMapApi"];
                    StringBuilder url = new StringBuilder();
                    url.Append(GeoUrl);
                    url.Append(string.Format("address={0}", EncodeString));
                    url.Append(string.Format("&key={0}", key));
                    var result = client.GetAsync(url.ToString()).Result;
                    if (!result.IsSuccessStatusCode)
                        return null;
                    GoogleMapResponseViewModel FinalResult = JsonConvert.DeserializeObject<GoogleMapResponseViewModel>(result.Content.ReadAsStringAsync().Result.ToString());
                    if (FinalResult.status.Equals("ZERO_RESULTS") || FinalResult.results.Count == 0)
                        return null;
                    if (FinalResult.status.Equals("OK") && FinalResult.results.Count > 0)
                    {
                        Coordinate coord = new Coordinate()
                        {
                            Latitude = FinalResult.results[0].geometry.location.lat,
                            Longitude = FinalResult.results[0].geometry.location.lng
                        };
                        return coord;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                string i = ex.Message.ToString();
                return null;
            }
        }


    }
}
