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
        public string Exemple { get; set; }

        private VoiceCommand() { }

        public static class Data
        {
            public static List<VoiceCommand> list = new List<VoiceCommand>() {
                //new VoiceCommand { ThumbImg = "sample1", Feature = "sample2", Exemple = "Sample3" },
                new VoiceCommand { ThumbImg = "sample1", Feature = "안녕!", Exemple = "거울앞의 사용자를 인식합니다" },
                new VoiceCommand { ThumbImg = "sample1", Feature = "날씨", Exemple = "구지면의 날씨를 보여줍니다." },
                new VoiceCommand { ThumbImg = "sample1", Feature = "뉴스", Exemple = "가장 최근의 뉴스 몇가지를 보여드립니다." },
                new VoiceCommand { ThumbImg = "sample1", Feature = "버스", Exemple = "근처 버스정류장의 상태를 알려드립니다." },
                new VoiceCommand { ThumbImg = "sample1", Feature = "노래", Exemple = "노래를 키거나, 끄거나, 잠시 멈춥니다. 또, 볼륨조절이 가능합니다." },
                new VoiceCommand { ThumbImg = "sample1", Feature = "안녕!", Exemple = "거울앞의 사용자를 인식합니다" },
                new VoiceCommand { ThumbImg = "sample1", Feature = "조명(또는 불)", Exemple = "조명을 키거나 끌 수 있습니다. 다양한 색을 제공합니다." },
            };
        }
    }
}
