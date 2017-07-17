using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using DSWeather;
using Windows.UI.Xaml;

namespace Wimi
{
    public partial class MainPage : Page
    {
        Weather weather = new Weather();

        private async void tbWeather_Loaded(object sender, RoutedEventArgs e)
        {
            SKWeather.Minutely min = await weather.GetCurrentWeatherAsync();
            string temp = min.temperature.tc;
            temp = temp.Substring(0, temp.Length - 1);
            tbWeather.Text = temp;
        }
    }
}
