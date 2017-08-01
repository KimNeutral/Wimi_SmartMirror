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
            lstMusic.Add("I LUV IT");
            lstMusic.Add("Uptown Funk");
            lstMusic.Add("나로 말할 것 같으면");
            lstMusic.Add("남이 될 수 있을까");
#else

            lstMusic.Add("New Face");
            lstMusic.Add("SIGNAL");
            lstMusic.Add("Rookie");
            lstMusic.Add("야생화");
            lstMusic.Add("남이 될 수 있을까");
            lstMusic.Add("LA SONG");
            lstMusic.Add("Beautiful");
            lstMusic.Add("비도 오고 그래서");
            lstMusic.Add("미치게 만들어");
#endif
            mediaElement.Volume = 1;
            mediaElement.Stop();
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
                gridMedia.Visibility = Windows.UI.Xaml.Visibility.Visible;
                string type = await music.GetMusicMIME(musicName);
                mediaElement.SetSource(s, type);
                mediaElement.AutoPlay = true;
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
            gridMedia.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }
        public void PlayMusic()
        {
            //Debug.WriteLine("음악 resume");
            //mediaElement.IsFullWindow = true;
            mediaElement.Play();
            gridMedia.Visibility = Windows.UI.Xaml.Visibility.Visible;
        }

        public void SetFullScreen()
        {
            if(gridMedia.Visibility == Windows.UI.Xaml.Visibility.Visible)
            {
                bool isFull = mediaElement.IsFullWindow;
                mediaElement.IsFullWindow = !isFull;
            }
        }
    }
}
