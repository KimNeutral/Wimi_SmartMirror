using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Playback;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Media.Core;
using Windows.Storage.Pickers;
using Windows.Storage;
using DSEmotion;
using Windows.Storage.Search;
using System.Threading.Tasks;

namespace DSMusic
{
    class Music
    {
        private Emotion emo;

        public Emotion Emo { get => emo; set => emo = value; }

        public async Task<Stream> MusicByEmotion(Emotion e)
        {
            StorageFile storageFile;
            Stream stream;
            string name = e.ToString() + ".mp3";
            storageFile = await KnownFolders.MusicLibrary.GetFileAsync(name);
            stream = await storageFile.OpenStreamForReadAsync();

            return stream;
        }
    }
}
