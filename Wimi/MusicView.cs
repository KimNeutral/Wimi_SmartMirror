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
using Windows.Storage.Search;
using Windows.Storage;

namespace Wimi
{
    public partial class MainPage : Page
    {
        bool bIntroPlayed = false;

        Music music = new Music();
        List<StorageFile> lstMusic = new List<StorageFile>();
        List<StorageFile> lstMusicPlaying = new List<StorageFile>();

        private async void initMusicList()
        {
            QueryOptions queryOption = new QueryOptions(CommonFileQuery.OrderByTitle, new string[] { ".mp3", ".mp4", ".wma" });
            queryOption.FolderDepth = FolderDepth.Deep;
            Queue<IStorageFolder> folders = new Queue<IStorageFolder>();

            var files = await KnownFolders.MusicLibrary.CreateFileQueryWithOptions(queryOption).GetFilesAsync();

            StorageFolder musicLib = KnownFolders.MusicLibrary;
            foreach (var file in files)
            {
                lstMusic.Add(file);
            }

            lstMusicPlaying = lstMusic;

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

        public async Task PlayMusic()
        {
            //string musicName = RandomMusicName();
            if (mediaElement.CurrentState == MediaElementState.Paused)
            {
                mediaElement.Play();
                return;
            }

            Random r = new Random();
            int index = r.Next(lstMusic.Count);


            StorageFile storageFile = lstMusic[index];
            using (var stream = await storageFile.OpenAsync(FileAccessMode.Read))
            {
                gridMedia.Visibility = Windows.UI.Xaml.Visibility.Visible;
                string musicName = storageFile.DisplayName;
                Debug.WriteLine(musicName + "을 랜덤 재생합니다");
                tbMediaName.Text = "♬ " + musicName;

                mediaElement.SetSource(stream, storageFile.ContentType);
                mediaElement.AutoPlay = true;
                mediaElement.Play();
            }

            lstMusicPlaying.RemoveAt(index);
            if(lstMusicPlaying.Count == 0)
            {
                lstMusicPlaying = lstMusic;
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
            if (!bIntroPlayed)
            {
                //chris: 처음 media play시 바로 동작하지 않는 문제가 있어 intro음을 넣었으나 끝나고 나면 status가 stop이 되어야 하는데 pause로 유지되고 있어 체크.
                bIntroPlayed = true;
                mediaElement.Stop();
                return;
            }

            if (mediaElement.CurrentState == MediaElementState.Paused)
            {
                mediaElement.Play();
                return;
            }

        }
    }
}
