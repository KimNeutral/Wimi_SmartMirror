using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wimi
{
    class VoiceCommand
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
