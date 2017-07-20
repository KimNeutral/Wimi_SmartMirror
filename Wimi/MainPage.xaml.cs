using DSMusic;
using System;
using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

//test
// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Wimi
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public partial class MainPage : Page
    {
        DispatcherTimer ClockTimer = new DispatcherTimer();
        Music mu = new Music();

        public MainPage()
        {
            this.InitializeComponent();

            ClockTimer.Tick += ClockTimer_Tick;
            ClockTimer.Interval = new TimeSpan(0, 0, 2);
            ClockTimer.Start();
        }

        private void ClearLeftPanel()
        {
            lbBusInfo.Visibility = Visibility.Collapsed;
            lbForcastInfo.Visibility = Visibility.Collapsed;
            lbNewsInfo.Visibility = Visibility.Collapsed;
        }

        private void ClockTimer_Tick(object sender, object e)
        {
            SetTime(); // tbClock.Text = DateTime.Now.ToString("HH:mm");
        }

        public void SetTime()
        {
            DateTime dt = DateTime.Now;
            tbDateTime.Text = dt.ToString("ddd, MMM d") + "th";
            tbTime.Text = dt.ToString("h:mmtt");
        }

        private async void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            initMusicList();
            initSynthesizer();
            InitializeRecognizer();

            HueInit();

            await Webcam.InitializeCameraAsync();
            captureElement.Source = Webcam.mediaCapture;
            await Webcam.StartCameraPreview();

            await face.InitListAsync();
            await InitFaceRec();

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
