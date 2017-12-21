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
using Windows.UI.Xaml.Media;
using Windows.Media.FaceAnalysis;
using Windows.System.Threading;
using System.Threading;
using Windows.Graphics.Imaging;
using Windows.Media;
using Windows.UI.Xaml.Shapes;

namespace Wimi
{
    public sealed partial class MainPage : Page
    {
        private readonly SolidColorBrush lineBrush = new SolidColorBrush(Windows.UI.Colors.Yellow);
        private readonly double lineThickness = 2.0;
        private readonly SolidColorBrush fillBrush = new SolidColorBrush(Windows.UI.Colors.Transparent);

        private FaceTracker faceTracker;
        private ThreadPoolTimer frameProcessingTimer;
        private SemaphoreSlim frameProcessingSemaphore = new SemaphoreSlim(1);


        WebcamHelper Webcam = new WebcamHelper();
        FaceRec face = null;

        DispatcherTimer faceTimer = new DispatcherTimer();

        bool IsIdentified = false;
        string comment = "";

        public string CurrentUser = "Guest";
        int CntErr = 0;

        private async void InitFaceRecAsync()
        {
            if (face == null)
            {
                face = new FaceRec();
            }

            if (this.faceTracker == null)
            {
                this.faceTracker = await FaceTracker.CreateAsync();
            }

            // Use a 66 millisecond interval for our timer, i.e. 15 frames per second
            TimeSpan timerInterval = TimeSpan.FromMilliseconds(66);
            this.frameProcessingTimer = Windows.System.Threading.ThreadPoolTimer.CreatePeriodicTimer(new Windows.System.Threading.TimerElapsedHandler(ProcessCurrentVideoFrame), timerInterval);

            faceTimer.Interval = new TimeSpan(0, 0, 1);
            faceTimer.Tick += FaceTimer_Tick;
        }

        private async void ProcessCurrentVideoFrame(ThreadPoolTimer timer)
        {
            if (!Webcam.IsInitialized())
            {
                return;
            }

            if (!frameProcessingSemaphore.Wait(0))
            {
                return;
            }

            try
            {
                IList<DetectedFace> faces = null;

                const BitmapPixelFormat InputPixelFormat = BitmapPixelFormat.Nv12;
                using (VideoFrame previewFrame = new VideoFrame(InputPixelFormat, (int)this.Webcam.videoProperties.Width, (int)this.Webcam.videoProperties.Height))
                {
                    await this.Webcam.mediaCapture.GetPreviewFrameAsync(previewFrame);

                    if (FaceDetector.IsBitmapPixelFormatSupported(previewFrame.SoftwareBitmap.BitmapPixelFormat))
                    {
                        faces = await this.faceTracker.ProcessNextFrameAsync(previewFrame);
                    }
                    else
                    {
                        throw new System.NotSupportedException("PixelFormat '" + InputPixelFormat.ToString() + "' is not supported by FaceDetector");
                    }

                    var previewFrameSize = new Windows.Foundation.Size(previewFrame.SoftwareBitmap.PixelWidth, previewFrame.SoftwareBitmap.PixelHeight);
                    var ignored = this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        this.SetupVisualization(previewFrameSize, faces);
                        Debug.WriteLine(previewFrameSize.Width + ", " + previewFrameSize.Height + ", Count : " + faces.Count);
                    });
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ProcessCurrentVideoFrame - " + ex.Message);
            }
            finally
            {
                frameProcessingSemaphore.Release();
            }

        }

        private void SetupVisualization(Windows.Foundation.Size framePizelSize, IList<DetectedFace> foundFaces)
        {
            this.VisualizationCanvas.Children.Clear();

            double actualWidth = this.VisualizationCanvas.ActualWidth;
            double actualHeight = this.VisualizationCanvas.ActualHeight;

            if (Webcam.IsInitialized() && foundFaces != null && actualWidth != 0 && actualHeight != 0)
            {
                double widthScale = framePizelSize.Width / actualWidth;
                double heightScale = framePizelSize.Height / actualHeight;

                foreach (DetectedFace face in foundFaces)
                {
                    // Create a rectangle element for displaying the face box but since we're using a Canvas
                    // we must scale the rectangles according to the frames's actual size.
                    Rectangle box = new Rectangle();
                    box.Width = (uint)(face.FaceBox.Width / widthScale);
                    box.Height = (uint)(face.FaceBox.Height / heightScale);
                    box.Fill = this.fillBrush;
                    box.Stroke = this.lineBrush;
                    box.StrokeThickness = this.lineThickness;
                    box.Margin = new Thickness((uint)(face.FaceBox.X / widthScale), (uint)(face.FaceBox.Y / heightScale), 0, 0);

                    Debug.WriteLine(face.FaceBox.X + ", " + face.FaceBox.Y);
                    this.VisualizationCanvas.Children.Add(box);
                }
            }
        }

        private async void FaceTimer_Tick(object sender, object e)
        {
            if (!Webcam.IsInitialized() || !face.IsInit())
            {
                return;
            }

            if (face == null)
                return;

            StorageFile captured = await Webcam.CapturePhoto();

            if (captured != null)
            {
                try
                {
                    if (await DetectFace(captured))
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
                if (CntErr == 5)
                {
                    faceTimer.Stop();
                    CurrentUser = "";
                    tbFaceName.Text = "";
                    spUser.Visibility = Visibility.Collapsed;
                    IsIdentified = false;
                    HideSchedule();
                    ClearPanel();
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
                        string name = faces[0];
                        if (!IsIdentified)
                        {
                            if (name != "외부인")
                            {
                                comment = "안녕하세요 " + name + "님, ";
                                CurrentUser = name;
                            }
                            else
                            {
                                comment = "안녕하세요, 손님이시군요.";
                                CurrentUser = "손님";
                            }
                        }
                    });
                    return true;
                }
                else
                {
                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        //tbFaceName.Text = "인식되는 사람이 없음";
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
                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                    {
                        tbEmotion.Text = emo.Value.ToString();
                        if (!IsIdentified)
                        {
                            string cmt = EmotionUtil.GetCommentByEmotion(emo.Value);
                            HueAtrBool = await HueControl.HueLightWithEmotion(emo.Value);
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
            faceTimer.Stop();
            IsIdentified = false;

            if (!Webcam.IsInitialized() || !face.IsInit()) //chris - if the webcam isn't connected,
            {
                return false;
            }

            if(face == null)
            {
                return false;
            }

            StorageFile captured = await Webcam.CapturePhoto();
            bool suc = await DetectFace(captured);
            if (!suc)
            {
                comment = "인식하지 못했어요. 다시 한번 해주세요.";
            }
            else
            {
                await DetectEmotion(captured);
                string ment = "Hello\n";
                if (CurrentUser.Equals("손님"))
                {
                    ment += "Guest";
                }
                else
                {
                    ment += CurrentUser + "!";
                    ShowSchedule();
                    faceTimer.Start();
                }
                IsIdentified = true;
                tbFaceName.Text = CurrentUser;
                spUser.Visibility = Visibility.Visible;
                ShowTbHello(ment);
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
