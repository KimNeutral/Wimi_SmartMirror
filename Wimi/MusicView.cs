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
using Microsoft.Toolkit.Uwp.UI.Animations;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml;

namespace Wimi
{
    public partial class MainPage : Page
    {
        bool bIntroPlayed = false;

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
            if (mediaElement.CurrentState == MediaElementState.Paused)
            {
                mediaElement.Play();
                return;
            }

            Random r = new Random();
            int index = r.Next(0, lstMusic.Count - 1);
            string musicName = lstMusic[index].ToString() + contentExtension;
            Debug.WriteLine(musicName + "을 랜덤 재생합니다");
            tbMediaName.Text = "♬ " + musicName;
            using (IRandomAccessStream s = await music.GetMusicStream(musicName))
            {
                gridMedia.Visibility = Windows.UI.Xaml.Visibility.Visible;
                string type = await music.GetMusicMIME(musicName);
                mediaElement.SetSource(s, type);
                mediaElement.AutoPlay = true;
                mediaElement.Play();
                //await imageSpeaker1.Scale(1.5f, 1.5f, 0, 0, 500, 0, EasingType.Linear).StartAsync();
                //await imageSpeaker2.Scale(1.5f, 1.5f, 0, 0, 500, 0, EasingType.Linear).StartAsync();
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
            tbMediaName.Text = string.Empty;
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

        private void mediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            if(!bIntroPlayed)
            {
                //chris: 처음 media play시 바로 동작하지 않는 문제가 있어 intro음을 넣었으나 끝나고 나면 status가 stop이 되어야 하는데 pause로 유지되고 있어 체크.
                bIntroPlayed = true;
                mediaElement.Stop();
            }
        }
    }
}
