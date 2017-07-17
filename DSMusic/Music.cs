using System;
using System.IO;
using Windows.Storage;
using DSEmotion;
using System.Threading.Tasks;
using System.Diagnostics;
using Windows.Storage.Streams;

namespace DSMusic
{
    public class Music
    {
        private Emotion emo;

        public Emotion Emo { get => emo; set => emo = value; }

        public string MusicByEmotion(Emotion e)
        {

            string name = e.ToString() + ".mp3";
            return name;
        }

        public async Task<IRandomAccessStream> GetMusicStream(string name)
        {
            StorageFile storageFile;

            storageFile = await KnownFolders.MusicLibrary.GetFileAsync(name);
            var stream = await storageFile.OpenAsync(FileAccessMode.Read);

            return stream;
        }

        public async Task<string> GetMusicMIME(string name)
        {
            StorageFile storageFile;
            storageFile = await KnownFolders.MusicLibrary.GetFileAsync(name);
            return storageFile.ContentType;
        }
    }
}
