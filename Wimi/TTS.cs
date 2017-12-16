using DSWeather;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources.Core;
using Windows.Media.SpeechSynthesis;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Wimi
{
    public partial class MainPage : Page
    {
        DispatcherTimer TTSDispatcherTimer;

        private SpeechSynthesizer synthesizer;
        private ResourceContext speechContext;
        private ResourceMap speechResourceMap;
        private MediaElement feedbackMediaElement;
        public Boolean TTSflag;


        private async void TellmeWeatherAsync()
        {
            Weather weather = new Weather();
            String WeatherResult="";
            SKWeatherHourly.Hourly nowWeather = new SKWeatherHourly.Hourly();
            nowWeather = await weather.GetCurrentWeatherHourlyAsync();

            string tc = nowWeather.temperature.tc.Split(new char[] { '.' })[0];
            string hu = nowWeather.humidity.Split(new char[] { '.' })[0];

            WeatherResult += nowWeather.grid.village +"의 오늘 날씨는 현재 "
                + tc + "도, 습도 " + hu + "퍼센트, ";
            //Debug.WriteLine(nowWeather.humidity);
            
            switch (nowWeather.sky.code)
            {
                case "SKY_A01":
                case "SKY_O01":
                    WeatherResult += "맑은 날씨입니다,"; break;
                case "SKY_A02":
                case "SKY_O02":
                    WeatherResult += "구름 조금 낀 날씨입니다,"; break;
                case "SKY_A03":
                case "SKY_O03":
                    WeatherResult += "구름많은 날씨입니다,"; break;
                case "SKY_A04":
                case "SKY_O04":
                    WeatherResult += "구름이 많고 비가 내립니다,"; break;
                case "SKY_A05":
                case "SKY_O05":
                    WeatherResult += "구름이 많고 눈이 내립니다,"; break;
                case "SKY_A06":
                case "SKY_O06":
                    WeatherResult += "구름이 많고 비나 눈이 내립니다,"; break;
                case "SKY_A07":
                case "SKY_O07":
                    WeatherResult += "흐린 날씨입니다,"; break;
                case "SKY_A08":
                case "SKY_O08":
                    WeatherResult += "흐리고 비가 내립니다,"; break;
                case "SKY_A09":
                case "SKY_O09":
                    WeatherResult += "흐리고 눈이 내립니다,"; break;
                case "SKY_A10":
                case "SKY_O10":
                    WeatherResult += "흐리고 비나 눈이 내립니다,"; break;
                case "SKY_A11":
                case "SKY_O11":
                    WeatherResult += "흐리고 낙뢰가 칠수 있습니다,"; break;
                case "SKY_A12":
                case "SKY_O12":
                    WeatherResult += "뇌우를 동반한 비가 내립니다,"; break;
                case "SKY_A13":
                case "SKY_O13":
                    WeatherResult += "뇌우를 동반한 눈이 내립니다,"; break;
                case "SKY_A14":
                case "SKY_O14":
                    WeatherResult += "뇌우를 동반한 비나 눈이 내립니다,"; break;
            }
            int hour = await CheckIfRainyAsync();
            if (hour != -1)
            {
                WeatherResult += hour + "시에 비가 내릴 예정이니 우산 꼭 가져가세요!";
            }
            SetVoice(WeatherResult);


        }
        private void TTSDispatcherTimer_Tick(object sender, object e)
        {
            TTSflag = false;
            TTSDispatcherTimer.Stop();
        }

        
        
        private void initSynthesizer()
        {
            TTSDispatcherTimer = new DispatcherTimer();
            TTSDispatcherTimer.Tick += TTSDispatcherTimer_Tick;
            TTSDispatcherTimer.Interval = new TimeSpan(0, 0, 3);

            feedbackMediaElement = new MediaElement();
            feedbackMediaElement.AutoPlay = true;
            feedbackMediaElement.Volume = 1;
            feedbackMediaElement.MediaOpened += FeedbackMediaElement_MediaOpened;
            feedbackMediaElement.MediaFailed += FeedbackMediaElement_MediaFailed;
            synthesizer = new SpeechSynthesizer();
            
            speechContext = ResourceContext.GetForCurrentView();
            speechContext.Languages = new string[] { SpeechSynthesizer.DefaultVoice.Language };

            speechResourceMap = ResourceManager.Current.MainResourceMap.GetSubtree("LocalizationTTSResources");

            var voices = SpeechSynthesizer.AllVoices;

            // Get the currently selected voice.
            VoiceInformation currentVoice = synthesizer.Voice;
            
            foreach (VoiceInformation voice in voices.OrderBy(p => p.Language))
            {
                synthesizer.Voice = voice;
            }
        }

        public async Task SetVoice(string str, bool isFilePath = false)
        {
            //return; //잡음 문제로 일단 아래코드를 처리하지 않는다.

            if (feedbackMediaElement.CurrentState.Equals(MediaElementState.Playing))
            {
                feedbackMediaElement.Stop();
            }
            else
            {
                string text = str;
                if (!String.IsNullOrEmpty(text))
                {
                    try
                    {
                        feedbackMediaElement.AutoPlay = true;
                        TTSDispatcherTimer.Start();
                        TTSflag = true;

                        if (isFilePath)
                        {
                            Windows.Storage.StorageFolder folder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync("Assets");
                            Windows.Storage.StorageFile file = await folder.GetFileAsync(str);
                            var stream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read);
                            feedbackMediaElement.SetSource(stream, file.ContentType);
                        }
                        else
                        {
                            // Create a stream from the text. This will be played using a media element.
                            SpeechSynthesisStream synthesisStream = await synthesizer.SynthesizeTextToStreamAsync(text);

                            // Set the source and start playing the synthesized audio stream.
                            feedbackMediaElement.SetSource(synthesisStream, synthesisStream.ContentType);
                            feedbackMediaElement.Play();
                        }
                    }
                    catch (System.IO.FileNotFoundException)
                    {
                        Debug.WriteLine("SetVoice - System.IO.FileNotFoundException");
                    }
                    catch (Exception ex)
                    {
                        feedbackMediaElement.AutoPlay = false;
                        Debug.WriteLine(ex.Message);
                    }
                }
            }
        }

        private void FeedbackMediaElement_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            Debug.WriteLine(e.ErrorMessage);
        }

        private void FeedbackMediaElement_MediaOpened(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("FeedbackMediaElement_MediaOpened");
        }
    }
}
