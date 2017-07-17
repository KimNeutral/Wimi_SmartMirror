using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using static DSWeather.SKWeather;
using System.Diagnostics;

namespace DSWeather
{
    public partial class Weather
    {
        public bool isLoadedWeather = false;
        public bool isLoadedDust = false;
        SKWeather skWeather = new SKWeather();
        HttpClient httpClient = null;
        Uri resourceUri = null;
        string resource = "http://apis.skplanetx.com/weather/current/minutely?version=1&&stnid=828";
        CancellationTokenSource cts;
        //SKDust skDust = new SKDust();

        //var httpClient = new HttpClient();
        //var resourceUri = new Uri("http://apis.skplanetx.com/weather/current/minutely?version=1&&stnid=828");
        //var cts = new CancellationTokenSource();

        public Weather()
        {
            httpClient = new HttpClient();
            resourceUri = new Uri(resource);
            cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(30));
            httpClient.DefaultRequestHeaders.Add("appKey", "f9674a68-c998-3d7f-a744-697aeb6678ae");
        }

        public async Task<Minutely> GetCurrentWeatherAsync()
        {
            HttpResponseMessage response = await httpClient.GetAsync(resourceUri, cts.Token);

            try
            {
                response.EnsureSuccessStatusCode();

                string httpResponseBody = await response.Content.ReadAsStringAsync();

                skWeather = JsonConvert.DeserializeObject<SKWeather>(httpResponseBody);

                List<Minutely> info = skWeather.weather.minutely;
                Minutely curInfo = info.FirstOrDefault();

                return curInfo;
            }
            catch(Exception e)
            {
                Debug.WriteLine("GetCurrentWeatherAsync - {0}", e.Message);
                return null;
            }
        }
    }
}
