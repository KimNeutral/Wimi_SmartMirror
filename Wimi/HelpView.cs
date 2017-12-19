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
        public string Exemple { get; set; }

        private VoiceCommand() { }

        public static class Data
        {
            public static List<VoiceCommand> list = new List<VoiceCommand>() {
                new VoiceCommand { ThumbImg = "sample1", Feature = "sample2", Exemple = "Sample3" },

            };
        }
    }
}
