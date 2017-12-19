using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace Wimi
{
    public partial class MainPage : Page
    {
        public void ShowHelp()
        {
            ClearPanel();
            gridHelp.Visibility = Windows.UI.Xaml.Visibility.Visible;
            lbHelp.ItemsSource = VoiceCommand.Data.list;
        }
    }

    public class VoiceCommand
    {
        /// <summary>
        /// 샘플이미지
        /// </summary>
        public string ThumbImg { get; set; }
        /// <summary>
        /// 명령어
        /// </summary>
        public string Feature { get; set; }
        /// <summary>
        /// 해당 명령어 수행내용
        /// </summary>
        public string Example { get; set; }

        private VoiceCommand() { }

        public static class Data
        {
            public static List<VoiceCommand> list = new List<VoiceCommand>() {
                //new VoiceCommand { ThumbImg = "sample1", Feature = "sample2", Exemple = "Sample3" },
                new VoiceCommand { ThumbImg = @"Assets\Icons\Siri.png", Feature = "안녕!", Example = "거울앞의 사용자를 인식합니다" },
                new VoiceCommand { ThumbImg = @"Assets\Icons\Weather.png", Feature = "날씨", Example = "구지면의 날씨를 보여줍니다." },
                new VoiceCommand { ThumbImg = @"Assets\Icons\News.png", Feature = "뉴스", Example = "가장 최근의 뉴스 몇가지를 보여드립니다." },
                new VoiceCommand { ThumbImg = @"Assets\Icons\Weather.png", Feature = "버스", Example = "근처 버스정류장의 상태를 알려드립니다." },
                new VoiceCommand { ThumbImg = @"Assets\Icons\Music.png", Feature = "노래", Example = "노래를 키거나, 끄거나, 잠시 멈춥니다. 또, 볼륨조절이 가능합니다." },
                new VoiceCommand { ThumbImg = @"Assets\Icons\Hue.png", Feature = "조명(또는 불)", Example = "조명을 키거나 끌 수 있습니다. 다양한 색을 제공합니다." },
            };
        }
    }
}
