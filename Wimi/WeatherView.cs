using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using DSWeather;
using Windows.UI.Xaml;

namespace Wimi
{
    public partial class MainPage : Page
    {
        Weather weather = new Weather();
        List<ForecastInfo> lForcastInfo = new List<ForecastInfo>();

        async Task<string> GetCurTempertureAsync()
        {
            SKWeather.Minutely min = await weather.GetCurrentWeatherAsync();
            string temp = min.temperature.tc;
            temp = temp.Substring(0, temp.Length - 1);
            return temp;
        }

        private async void tbWeather_Loaded(object sender, RoutedEventArgs e)
        {
            //string temperature = await GetCurTempertureAsync();
            //tbTc.Text = temperature;
        }

        async Task GetForecastInfo()
        {
            lForcastInfo = await weather.GetForecastInfoByCountAsync();
            lbForcastInfo.ItemsSource = lForcastInfo;
        }

        async Task<int> CheckIfRainyAsync()
        {
            if(lForcastInfo.Count <= 0)
            {
                await GetForecastInfo();
            }
            int hour = 0;

            foreach(var forcast in lForcastInfo)
            {
                string stat = forcast.stat;
                if (stat.Equals("비"))
                {
                    hour = forcast.hour;
                    break;
                }
            }

            return hour;
        }
    }
}
