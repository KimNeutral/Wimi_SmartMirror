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
            string temp = await GetCurTempertureAsync();
            tbWeather.Text = temp;
        }

        async void GetForecastInfo()
        {
            lForcastInfo = await weather.GetForecastInfoByCountAsync();
            lbForcastInfo.ItemsSource = lForcastInfo;
        }
    }
}
