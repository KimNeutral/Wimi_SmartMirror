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

        bool IsIdentified = false;
        string comment = "";

        string CurrentUser = "";
        int CntErr = 0;

        private async Task InitFaceRec()
        {
            faceTimer.Interval = new TimeSpan(0, 0, 1);
            faceTimer.Tick += FaceTimer_Tick;
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
                    if(await DetectFace(captured))
                    {
                        if (CurrentUser != tbFaceName.Text)
                        {
                            CntErr++;
                        }
                        else
                        {
                            CntErr = 0;
                        }
                    } else
                    {
                        CntErr++;
                    }
                    await DetectEmotion(captured);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
                if(CntErr == 5)
                {
                    faceTimer.Stop();
                    CurrentUser = "";
                    IsIdentified = false;
                }
            }
        }

        private async Task<bool> DetectFace(StorageFile captured)
        {
            using (Stream s = await captured.OpenStreamForReadAsync())
            {
                string[] faces = await face.GetIdentifiedNameAsync(s);
                if (faces.Count() > 0)
                {
                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        tbFaceName.Text = faces[0];
                        if (!IsIdentified)
                        {
                            if (tbFaceName.Text != "외부인")
                            {
                                comment = "안녕하세요 " + faces[0] + "님, ";
                                CurrentUser = faces[0];
                            }
                            else
                            {
                                comment = "안녕하세요, 손님이시군요.";
                            }
                        }
                    });
                    return true;
                }
                else
                {
                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        tbFaceName.Text = "인식되는 사람이 없음";
                    });
                    return false;
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
                        if (!IsIdentified)
                        {
                            string cmt = EmotionUtil.GetCommentByEmotion(emo.Value);
                            comment += cmt;
                            faceTimer.Start();
                        }
                    });
                    break;
                }
            }
        }

        private async Task<bool> DetectCalledByWimi()
        {
            IsIdentified = false;
            faceTimer.Stop();
            StorageFile captured = await Webcam.CapturePhoto();
            bool suc = await DetectFace(captured);
            await DetectEmotion(captured);
            if (!suc)
            {
                comment = "인식하지 못했어요. 다시 한번 해주세요.";
            }
            SetVoice(comment);
            
            return suc;
        }

        private async void btnCapture_Click(object sender, RoutedEventArgs e)
        {
            await DetectCalledByWimi();
        }
    }
}
