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

        private bool IsIdentified = false;
        private bool IsCallFaceRecog = false;

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

            // Use a 66 millisecond interval for our timer, i.e. 10 frames per second
            TimeSpan timerInterval = TimeSpan.FromMilliseconds(100);
            this.frameProcessingTimer = Windows.System.Threading.ThreadPoolTimer.CreatePeriodicTimer(new Windows.System.Threading.TimerElapsedHandler(ProcessCurrentVideoFrame), timerInterval);

            faceTimer.Interval = new TimeSpan(0, 0, 1);
            faceTimer.Tick += FaceTimer_Tick;
        }

        //얼굴 감지
        private async void ProcessCurrentVideoFrame(ThreadPoolTimer timer)
        {
            if (Webcam == null)
            {
                return;
            }

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
                        if (!IsIdentified && faces.Count > 0)
                        {
                            await this.dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () => { await DetectCalledByFaceTrackerAsync(); });
                        }
                    }
                    else
                    {
                        throw new System.NotSupportedException("PixelFormat '" + InputPixelFormat.ToString() + "' is not supported by FaceDetector");
                    }

                    var previewFrameSize = new Windows.Foundation.Size(previewFrame.SoftwareBitmap.PixelWidth, previewFrame.SoftwareBitmap.PixelHeight);
                    var ignored = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        this.SetupVisualizationAsync(previewFrameSize, faces);
                        //Debug.WriteLine(previewFrameSize.Width + ", " + previewFrameSize.Height + ", Count : " + faces.Count);
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

        //private async Task<StorageFile> ConvertSoftwareBitmapToStorageFileAsync(SoftwareBitmap softwareBitmap)
        //{
        //    string fileName = DateTime.UtcNow.ToString("yyyy.MMM.dd HH-mm-ss") + " Wimi Face" + ".jpg";
        //    CreationCollisionOption collisionOption = CreationCollisionOption.GenerateUniqueName;
        //    StorageFile file = await ApplicationData.Current.TemporaryFolder.CreateFileAsync(fileName, collisionOption);

        //    using(Stream stream = await file.OpenStreamForWriteAsync())
        //    {
        //        BitmapEncoder bitmapEncoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, stream.AsRandomAccessStream());
        //        try
        //        {
        //            await bitmapEncoder.FlushAsync();
        //        }
        //        catch (Exception e)
        //        {
        //            Debug.WriteLine("ConvertSoftwareBitmapToStorageFileAsync - " + e.Message);
        //        }
        //    }

        //    return file;
        //}

        //Canvas에 감지된 얼굴 그리기
        private void SetupVisualizationAsync(Windows.Foundation.Size framePizelSize, IList<DetectedFace> foundFaces)
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

                    //Debug.WriteLine(face.FaceBox.X + ", " + face.FaceBox.Y);
                    this.VisualizationCanvas.Children.Add(box);
                }
            }
        }

        private async void FaceTimer_Tick(object sender, object e)
        {
            if (face == null || Webcam == null)
            {
                return;
            }

            if (!Webcam.IsInitialized() || !face.IsInit())
            {
                return;
            }

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
                            await DetectEmotion(captured);
                        }
                    } else
                    {
                        CntErr++;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
                if (CntErr >= 3)
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
            if(captured == null)
            {
                return false;
            }
            using (Stream s = await captured.OpenStreamForReadAsync())
            {
                if (s == null)
                {
                    return false;
                }
                try
                {
                    string[] faces = await face.GetIdentifiedNameAsync(s);
                    if (faces.Count() > 0)
                    {
                        await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {
                            if (IsIdentified)
                            {
                                var users = faces.Where(x => x.Equals(CurrentUser));
                                if(users.Count() != 0)
                                {
                                    if (!string.IsNullOrEmpty(users.First()))
                                    {
                                        return;
                                    }
                                }
                            }
                            string name = faces[0];
                            if (name != "외부인")
                            {
                                comment = "안녕하세요 " + name + "님, ";
                                CurrentUser = name;
                                Debug.WriteLine(CurrentUser);
                            }
                            else
                            {
                                comment = "안녕하세요, 손님이시군요.";
                                CurrentUser = "손님";
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
                catch(Exception e)
                {
                    Debug.WriteLine("DetectFace - " + e.Message);
                    return false;
                }
            }
        }

        private async Task DetectEmotion(StorageFile captured)
        {
            if (captured == null)
            {
                return;
            }
            using (Stream s = await captured.OpenStreamForReadAsync())
            {
                if(s == null)
                {
                    return;
                }
                try
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
                                //HueAtrBool = await HueControl.HueLightWithEmotion(emo.Value);
                                comment += cmt;
                                faceTimer.Start();
                            }
                        });
                        break;
                    }
                }
                catch(Exception e)
                {
                    Debug.WriteLine("DetectEmotion - " + e.Message);    
                }
            }
        }

        private async Task<bool> DetectCalledByFaceTrackerAsync()
        {
            if (IsCallFaceRecog)
            {
                return false;
            }

            if (face == null || Webcam == null)
            {
                return false;
            }

            if (!Webcam.IsInitialized() || !face.IsInit() || IsIdentified) //chris - if the webcam isn't connected,
            {
                return false;
            }
            IsCallFaceRecog = true;
            faceTimer.Stop();

            StorageFile captured = await Webcam.CapturePhoto();
            bool suc = await DetectFace(captured);
            comment = "";
            if (!suc || string.IsNullOrEmpty(CurrentUser))
            {
                //comment = "인식하지 못했어요. 다시 한번 해주세요.";
            }
            else
            {
                await DetectEmotion(captured);
                string ment = "Hello\n";
                if (CurrentUser.Equals("손님"))
                {
                    ment += "Guest";
                    IsCallFaceRecog = false;
                    return true;
                }
                else
                {
                    ment += CurrentUser + "!";
                    ShowSchedule();
                    IsIdentified = true;
                    faceTimer.Start();
                }
                tbFaceName.Text = CurrentUser;
                spUser.Visibility = Visibility.Visible;
                ShowTbHello(ment);
                SetVoice(comment);
            }
            IsCallFaceRecog = false;
            return suc;
        }

        private async void btnCapture_Click(object sender, RoutedEventArgs e)
        {
            await DetectCalledByFaceTrackerAsync();
        }
    }
}
