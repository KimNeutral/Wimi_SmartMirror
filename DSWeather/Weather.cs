using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
//using WimiSample.Dust;
//using WimiSample.Weather;
using static DSWeather.SKWeather;
using DSWeather;

namespace DSWeather
{
    public class Weather
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

        public Weather(){} //생성자 필요없음

        public async Task<Minutely> GetCurrentWeatherAsync()
        {
            HttpResponseMessage response = await httpClient.GetAsync(resourceUri, cts.Token);
            response.EnsureSuccessStatusCode();
            string httpResponseBody = await response.Content.ReadAsStringAsync();

            skWeather = JsonConvert.DeserializeObject<SKWeather>(httpResponseBody);

            List<Minutely> info = skWeather.weather.minutely;
            Minutely curInfo = info.FirstOrDefault();

            return curInfo;
        }
    }
}
