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
            SKWeatherHourly.Hourly hour = await weather.GetCurrentWeatherHourlyAsync();
            if(hour == null)
            {
                return null;
            }
            WeatherIcon(hour);
            string temp = hour.temperature.tc;
            temp = temp.Substring(0, temp.Length - 1);
            return temp;
        }

        private void ShowForecast()
        {
            ClearPanel();
            lbForcastInfo.Visibility = Visibility.Visible;
        }

        private async void tbWeather_Loaded(object sender, RoutedEventArgs e)
        {
            string temperature = await GetCurTempertureAsync();
            if(temperature != null)
            {
                tbTc.Text = temperature;
            }
        }

        private void WeatherIcon(SKWeatherHourly.Hourly hour)
        {
            switch (hour.sky.code)
            {
                case "SKY_A01":
                case "SKY_O01":
                    tbSky.Text = "\uE284";
                    //WeatherResult += "맑은 날씨입니다,";
                    break;
                case "SKY_A02":
                case "SKY_O02":
                    tbSky.Text = "\uE286";
                    //WeatherResult += "구름 조금 낀 날씨입니다,";
                    break;
                case "SKY_A03":
                case "SKY_O03":
                    tbSky.Text = "\uE285";
                    //WeatherResult += "구름많은 날씨입니다,";
                    break;
                case "SKY_A04":
                case "SKY_O04":
                    tbSky.Text = "\uE288";
                    //WeatherResult += "구름이 많고 비가 내립니다,";
                    break;
                case "SKY_A05":
                case "SKY_O05":
                    tbSky.Text = "\uE28A";
                    //WeatherResult += "구름이 많고 눈이 내립니다,";
                    break;
                case "SKY_A06":
                case "SKY_O06":
                    tbSky.Text = "\uE288";
                    //WeatherResult += "구름이 많고 비나 눈이 내립니다,";
                    break;
                case "SKY_A07":
                case "SKY_O07":
                    tbSky.Text = "\uE285";
                    //WeatherResult += "흐린 날씨입니다,";
                    break;
                case "SKY_A08":
                case "SKY_O08":
                    tbSky.Text = "\uE288";
                    //WeatherResult += "흐리고 비가 내립니다,";
                    break;
                case "SKY_A09":
                case "SKY_O09":
                    tbSky.Text = "\uE28A";
                    //WeatherResult += "흐리고 눈이 내립니다,";
                    break;
                case "SKY_A10":
                case "SKY_O10":
                    tbSky.Text = "\uE288";
                    //WeatherResult += "흐리고 비나 눈이 내립니다,";
                    break;
                case "SKY_A11":
                case "SKY_O11":
                    tbSky.Text = "\uE289";
                    //WeatherResult += "흐리고 낙뢰가 칠수 있습니다,";
                    break;
                case "SKY_A12":
                case "SKY_O12":
                    tbSky.Text = "\uE289";
                    //WeatherResult += "뇌우를 동반한 비가 내립니다,"; 
                    break;
                case "SKY_A13":
                case "SKY_O13":
                    tbSky.Text = "\uE289";
                    //WeatherResult += "뇌우를 동반한 눈이 내립니다,"; 
                    break;
                case "SKY_A14":
                case "SKY_O14":
                    tbSky.Text = "\uE289";
                    //WeatherResult += "뇌우를 동반한 비나 눈이 내립니다,"; 
                    break;
            }
        }

        async Task GetForecastInfo()
        {
            lForcastInfo = await weather.GetForecastInfoByCountAsync(7);
            lbForcastInfo.ItemsSource = lForcastInfo;
        }

        async Task<int> CheckIfRainyAsync()
        {
            if(lForcastInfo.Count <= 0)
            {
                await GetForecastInfo();
            }
            int hour = -1;

            foreach(var forcast in lForcastInfo)
            {
                string stat = forcast.wfKor;
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
