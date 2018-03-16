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
    // SKWeather API 종료로 인한 클래스 사용 불가.
    public partial class Weather
    {
        public bool isLoadedWeather = false;
        public bool isLoadedDust = false;
        SKWeather skWeather = new SKWeather();
        HttpClient httpClient = null;
        Uri resourceUri = null;
        string resource = "http://apis.skplanetx.com/weather/current/minutely?version=1&&stnid=828";
        CancellationTokenSource cts;

        public Weather()
        {
            httpClient = new HttpClient();
            resourceUri = new Uri(resource);
            cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(30));
            httpClient.DefaultRequestHeaders.Add("appKey", "1afe336d-2063-3773-ba04-8431599ee11c");
        }

        public async Task<Minutely> GetCurrentWeatherAsync()
        {
            try
            {
                HttpResponseMessage response = await httpClient.GetAsync(resourceUri, cts.Token);

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
