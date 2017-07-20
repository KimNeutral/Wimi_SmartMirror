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
using System.Collections;
using System.Diagnostics;

namespace Wimi
{
    public partial class MainPage : Page
    {
        const string contentExtension = ".mp4";
        Music music = new Music();
        ArrayList lstMusic = new ArrayList();

        private void initMusicList()
        {
#if true
            lstMusic.Add("New Face");
            lstMusic.Add("SIGNAL");
            lstMusic.Add("Rookie");
            lstMusic.Add("야생화");
            lstMusic.Add("남이 될 수 있을까");
            lstMusic.Add("LA SONG");
            lstMusic.Add("Beautiful");
            lstMusic.Add("비도 오고 그래서");
            lstMusic.Add("미치게 만들어");
#else
            lstMusic.Add("김필");
            lstMusic.Add("내 눈에만 보여");
            lstMusic.Add("너무너무너무");
            lstMusic.Add("뉴페이스");
            lstMusic.Add("라디");
            lstMusic.Add("반달");
            lstMusic.Add("버즈");
            lstMusic.Add("불타오르네");
            lstMusic.Add("삐딱하게");
            lstMusic.Add("사랑에 빠졌을 때");
            lstMusic.Add("소란했던 시절에");
            lstMusic.Add("소유");
            lstMusic.Add("쓰담쓰담");
            lstMusic.Add("아웃사이드");
            lstMusic.Add("오이오");
            lstMusic.Add("응급실");
            lstMusic.Add("쩔어");
            lstMusic.Add("호구");
            lstMusic.Add("아는사람 얘기");
#endif
        }

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

        public async Task PlayRandomMusic()
        {
            //string musicName = RandomMusicName();
            Random r = new Random();
            int index = r.Next(0, lstMusic.Count - 1);
            string musicName = lstMusic[index].ToString() + contentExtension;
            Debug.WriteLine(musicName + "을 랜덤 재생합니다");
            using (IRandomAccessStream s = await music.GetMusicStream(musicName))
            {
                mediaElement.Visibility = Windows.UI.Xaml.Visibility.Visible;
                string type = await music.GetMusicMIME(musicName);
                mediaElement.SetSource(s, type);
                mediaElement.Play();
                //mediaElement.IsFullWindow = true;
            }
        }

        public void PauseMusic()
        {
            Debug.WriteLine("음악 일시중지");
            mediaElement.Pause();
        }

        public void StopMusic()
        {
            Debug.WriteLine("음악 중지");
            mediaElement.Stop();
            mediaElement.IsFullWindow = false;
            mediaElement.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }
        public void PlayMusic()
        {
            //Debug.WriteLine("음악 resume");
            //mediaElement.IsFullWindow = true;
            mediaElement.Play();
            mediaElement.Visibility = Windows.UI.Xaml.Visibility.Visible;
        }

        public void SetFullScreen()
        {
            if(mediaElement.Visibility == Windows.UI.Xaml.Visibility.Visible)
            {
                bool isFull = mediaElement.IsFullWindow;
                mediaElement.IsFullWindow = !isFull;
            }
        }
    }
}
