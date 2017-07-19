using DSMusic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using DSEmotion;
using System.IO;
using Windows.Storage.Streams;

namespace Wimi
{
    public partial class MainPage : Page
    {
        Music music = new Music();
        String[] MusicNames = { "김필", "내 눈에만 보여", "너무너무너무", "뉴페이스",
                                "라디", "반달", "버즈", "불타오르네", "삐딱하게", "사랑에 빠졌을 때",
                                "소란했던 시절에", "소유", "쓰담쓰담", "아웃사이드", "오이오",
                                "응급실", "쩔어", "톰보이", "호구", "아는사람 얘기" };
        bool[] random = new bool[20];

        public async Task PlayMusicByEmotionAsync(Emotion emotion)
        {
            string musicName = music.MusicByEmotion(emotion);
            using (IRandomAccessStream s = await music.GetMusicStream(musicName))
            {
                string type = await music.GetMusicMIME(musicName);
                mediaElement.SetSource(s, type);
                mediaElement.Play();
            }
        }

        public async Task PlayMusic()
        {
            string musicName = RandomMusicName();
            using (IRandomAccessStream s = await music.GetMusicStream(musicName))
            {
                string type = await music.GetMusicMIME(musicName);
                mediaElement.SetSource(s, type);
                mediaElement.Play();
            }
        }

        public string RandomMusicName()
        {
            Random ran = new Random();
            bool flag = false;
            foreach(bool v in random)
            {
                if (!v)
                {
                    flag = true;
                }
            }
            if (!flag)
            {
                random = new bool[20];
            }

            int num = 1;
            do
            {
                num = ran.Next();
                num = (num % 20);
            } while (random[num] == true);

            return MusicNames[num] + ".mp3";
        }

        public void PauseMusic()
        {
            mediaElement.Pause();
        }

        public void StopMusic()
        {
            mediaElement.Stop();
        }
    }
}
