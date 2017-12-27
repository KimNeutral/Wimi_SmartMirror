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
                var wt = await weather.GetForecastInfoByCountAsync(1);
                if(wt.Count > 0)
                {
                    tbSky.Text = wt[0].statSymbol;
                    return wt[0].temp + "";
                }
                else
                {
                    return null;
                }
            }
            tbSky.Text = WeatherIcon(hour);
            string temp = hour.temperature.tc;
            temp = temp.Substring(0, temp.Length - 1);
            return temp;
        }

        private async Task ShowForecastAsync()
        {
            ClearPanel();
            gridWeather.Visibility = Visibility.Visible;
            var wt = await weather.GetForecastInfoByCountAsync(5);
            if(wt.Count > 0)
            {
                wt[0].tmx = wt[4].tmx;
                wt[0].tmn = wt[4].tmn;
                gridCurWeather.DataContext = wt[0];
            }
            else
            {
                gridWeather.Visibility = Visibility.Collapsed;
            }
        }

        private async void tbWeather_Loaded(object sender, RoutedEventArgs e)
        {
            string temperature = await GetCurTempertureAsync();
            if(temperature != null)
            {
                tbTc.Text = temperature;
            }
        }

        private string WeatherIcon(SKWeatherHourly.Hourly hour)
        {
            switch (hour.sky.code)
            {
                case "SKY_A01":
                case "SKY_O01":
                    return "\uE284";
                    //WeatherResult += "맑은 날씨입니다,";
                case "SKY_A02":
                case "SKY_O02":
                    return "\uE286";
                    //WeatherResult += "구름 조금 낀 날씨입니다,";
                case "SKY_A03":
                case "SKY_O03":
                    return "\uE285";
                    //WeatherResult += "구름많은 날씨입니다,";
                case "SKY_A04":
                case "SKY_O04":
                    return "\uE288";
                    //WeatherResult += "구름이 많고 비가 내립니다,";
                case "SKY_A05":
                case "SKY_O05":
                    return "\uE28A";
                    //WeatherResult += "구름이 많고 눈이 내립니다,";
                case "SKY_A06":
                case "SKY_O06":
                    return "\uE288";
                    //WeatherResult += "구름이 많고 비나 눈이 내립니다,";
                case "SKY_A07":
                case "SKY_O07":
                    return "\uE285";
                    //WeatherResult += "흐린 날씨입니다,";
                case "SKY_A08":
                case "SKY_O08":
                    return "\uE288";
                    //WeatherResult += "흐리고 비가 내립니다,";
                case "SKY_A09":
                case "SKY_O09":
                    return "\uE28A";
                    //WeatherResult += "흐리고 눈이 내립니다,";
                case "SKY_A10":
                case "SKY_O10":
                    return "\uE288";
                    //WeatherResult += "흐리고 비나 눈이 내립니다,";
                case "SKY_A11":
                case "SKY_O11":
                    return "\uE289";
                    //WeatherResult += "흐리고 낙뢰가 칠수 있습니다,";
                case "SKY_A12":
                case "SKY_O12":
                    return "\uE289";
                    //WeatherResult += "뇌우를 동반한 비가 내립니다,"; 
                case "SKY_A13":
                case "SKY_O13":
                    return "\uE289";
                    //WeatherResult += "뇌우를 동반한 눈이 내립니다,"; 
                case "SKY_A14":
                case "SKY_O14":
                    return "\uE289";
                    //WeatherResult += "뇌우를 동반한 비나 눈이 내립니다,"; 
            }
            return "";

        }

        async Task GetForecastInfo()
        {
            lForcastInfo = await weather.GetForecastInfoByCountAsync(5);
            if(lForcastInfo.Count > 0)
            {
                lbForcastInfo.ItemsSource = lForcastInfo;
                //tbMaxTemp.Text = weather.strMaxTemperature;
                //tbMinTemp.Text = weather.strMinTemperature;
            }
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
