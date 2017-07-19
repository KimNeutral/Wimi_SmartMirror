using System;
using System.Collections.Generic;
using System.Linq;
using DSFace;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Storage;
using System.IO;
using DSEmotion;
using System.Threading.Tasks;
using Windows.UI.Core;
using System.Diagnostics;

namespace Wimi
{
    public sealed partial class MainPage : Page
    {
        WebcamHelper Webcam = new WebcamHelper();
        FaceRec face = new FaceRec();

        DispatcherTimer faceTimer = new DispatcherTimer();

        private async Task InitFaceRec()
        {
            faceTimer.Interval = new TimeSpan(0, 0, 1);
            faceTimer.Tick += FaceTimer_Tick;

            faceTimer.Start();
        }

        private async void FaceTimer_Tick(object sender, object e)
        {
            if (!Webcam.IsInitialized())
            {
                return;
            }

            StorageFile captured = await Webcam.CapturePhoto();

            if(captured != null)
            {
                try
                {
                    //await DetectFace(captured);
                    //await DetectEmotion(captured);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
        }

        private async Task DetectFace(StorageFile captured)
        {
            using (Stream s = await captured.OpenStreamForReadAsync())
            {
                string[] faces = await face.GetIdentifiedNameAsync(s);
                if (faces.Count() > 0)
                {
                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        tbFaceName.Text = faces[0];
                    });
                }
                else
                {
                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        tbFaceName.Text = "";
                    });
                }
            }
        }

        private async Task DetectEmotion(StorageFile captured)
        {
            using (Stream s = await captured.OpenStreamForReadAsync())
            {
                Dictionary<Guid, Emotion> emotions = await face.GetEmotionByGuidAsync(s);
                foreach (var emo in emotions)
                {
                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        tbEmotion.Text = emo.Value.ToString();
                    });
                }
            }
        }

        private async void btnCapture_Click(object sender, RoutedEventArgs e)
        {
            StorageFile captured = await Webcam.CapturePhoto();
            await DetectFace(captured);

            //using (Stream s = await captured.OpenStreamForReadAsync())
            //{
            //    Dictionary<Guid, Emotion> emotions = await face.GetEmotionByGuidAsync(s);
            //    foreach (var emo in emotions)
            //    {
            //        tbEmotion.Text = emo.Value.ToString();
            //    }
            //}

            //await PlayMusicByEmotionAsync(EmotionUtil.GetEmotionByString(tbEmotion.Text));
        }
    }
}
