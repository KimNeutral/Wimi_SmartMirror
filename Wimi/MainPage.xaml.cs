using System;
using System.Threading.Tasks;
using DSMusic;
using Microsoft.Toolkit.Uwp.UI.Animations;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;
using System.Threading;
using System.Collections.Generic;
using Windows.UI.ViewManagement;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409
// ref. http://uwpcommunitytoolkit.readthedocs.io/en/master/

namespace Wimi
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public partial class MainPage : Page
    {
        const bool USE_FACERECOG = true; //chris: for test

        DispatcherTimer ClockTimer = new DispatcherTimer();
        Music mu = new Music();

        public MainPage()
        {
            this.InitializeComponent();
            BackgroundBrush.ImageSource = null; //chris: 이미지 액자로 활용시 BackgroundBrush에 이미지를 설정하면 된다.
            //GradientAnimation.Begin(); //chris: 음성인식되는중 배경을 쓸지도 몰라서 코드는 남겨둠.

            Window.Current.CoreWindow.KeyDown += CoreWindow_KeyDown;

            ClockTimer.Tick += ClockTimer_Tick;
            ClockTimer.Interval = new TimeSpan(0, 0, 2);
            ClockTimer.Start();
        }

        private void CoreWindow_KeyDown(CoreWindow sender, KeyEventArgs e)
        {
            ApplicationView view = ApplicationView.GetForCurrentView();
            if (e.VirtualKey == Windows.System.VirtualKey.Escape)
            {
                view.ExitFullScreenMode();
            }
            else if (e.VirtualKey == Windows.System.VirtualKey.Enter)
            {
                view.TryEnterFullScreenMode();
            }
        }

        private void ClearPanel()
        {
            int count = VisualTreeHelper.GetChildrenCount(gridCommand);
            List<string> whiteList = new List<string>()
            {
                "gridVolumeStat",
                "gridVoiceHelper"
            };
            
            for (int i = 0; i < count; i++)
            {
                UIElement child = (UIElement)VisualTreeHelper.GetChild(gridCommand, i);

                if (child is Grid && string.IsNullOrEmpty(whiteList.Find(x => x.Equals(((Grid)child).Name))))
                {
                    child.Visibility = Visibility.Collapsed;
                }
            }

            //gridBus.Visibility = Visibility.Collapsed;
            //gridWeather.Visibility = Visibility.Collapsed;
            //gridNews.Visibility = Visibility.Collapsed;
            tbHello.Visibility = Visibility.Collapsed;
        }

        private void ShowTbHello(string ment)
        {
            //ClearPanel();
            tbHello.Text = ment;
            tbHello.Visibility = Visibility.Visible;
        }

        private void ClockTimer_Tick(object sender, object e)
        {
            SetTime(); // tbClock.Text = DateTime.Now.ToString("HH:mm");
        }

        public void SetTime()
        {
            DateTime dt = DateTime.Now;
            tbDateTime.Text = dt.ToString("dddd, MMM d").ToUpper() + "th";
            tbTime.Text = dt.ToString("h:mm");
            tbAmPm.Text = dt.ToString("tt");
        }

        private async void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            ApplicationView view = ApplicationView.GetForCurrentView();
            view.TryEnterFullScreenMode();

            await gridVoiceHelper.Offset(0, -400, 0, 0, EasingType.Linear).StartAsync();

            InitExpireCommand();
            InitVoiceCommand();
            initMusicList();
            initSynthesizer();
            await InitializeRecognizer();

            HueInit();

            bool isInit = await Webcam.InitializeCameraAsync();

            if (isInit)
            {
                captureElement.Source = Webcam.mediaCapture;
                await Webcam.StartCameraPreview();

                if (USE_FACERECOG)
                {
                    InitFaceRecAsync();
                    bool result = await face.InitListAsync();
                    if (!result)
                    {
                        tbCameraStat.Text = "현재 얼굴 인식을 사용할 수 없습니다.";
                    }
                }
            }
            else
            {
                tbCameraStat.Text = "연결되어있는 웹캠이 존재하지 않습니다!";
            }
            
            GetBusInfo();
#if PC_MODE
            Getschedule();
#endif
            await GetForecastInfo();

            await GetNewsInfo();
        }

        protected async override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            await Webcam.StopCameraPreview();
            Webcam.mediaCapture.Dispose();

            CleanSpeechRecognizer();
            RemoveConstraints();
        }
    }
}
