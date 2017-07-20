using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using static DSWeather.SKWeatherHourly;
using System.Diagnostics;

namespace DSWeather
{
    public partial class Weather
    {
        SKWeatherHourly skHourly = new SKWeatherHourly();

        public async Task<Hourly> GetCurrentWeatherHourlyAsync()
        {
            string hourlyResource = "http://apis.skplanetx.com/weather/current/hourly?lon=&village=구지면&county=달성군&lat=&city=대구&version=1";
            Uri hourlyUri = new Uri(hourlyResource);

            try
            {
                HttpResponseMessage response = await httpClient.GetAsync(hourlyUri, cts.Token);

                response.EnsureSuccessStatusCode();

                string httpResponseBody = await response.Content.ReadAsStringAsync();

                skHourly = JsonConvert.DeserializeObject<SKWeatherHourly>(httpResponseBody);

                List<Hourly> info = skHourly.weather.hourly;
                Hourly curInfo = info.FirstOrDefault();

                return curInfo;
            }
            catch (Exception e)
            {
                Debug.WriteLine("GetCurrentWeatherAsync - {0}", e.Message);
                return null;
            }
        }
    }
}
