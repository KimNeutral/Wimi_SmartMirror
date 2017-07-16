using System;
using System.IO;
using Windows.Storage;
using DSEmotion;
using System.Threading.Tasks;
using System.Diagnostics;

namespace DSMusic
{
    public class Music
    {
        private Emotion emo;

        public Emotion Emo { get => emo; set => emo = value; }

        public async Task<Stream> MusicByEmotion(Emotion e)
        {
            StorageFile storageFile;
            Stream stream;
            string name = e.ToString() + ".mp3";
            Debug.WriteLine(name);
            storageFile = await KnownFolders.MusicLibrary.GetFileAsync(name);
            stream = await storageFile.OpenStreamForReadAsync();

            return stream;
        }
    }
}
