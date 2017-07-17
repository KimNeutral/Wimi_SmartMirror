using DSBus;
using DSEmotion;
using DSMusic;
using System;
using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

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

            ClockTimer.Tick += Timer_Tick;
            ClockTimer.Interval = new TimeSpan(0, 0, 1);
            ClockTimer.Start();
        }

        private void ClockTimer_Tick(object sender, object e)
        {
            tbClock.Text = DateTime.Now.ToString("HH:mm");
        }

        private async void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            initSynthesizer();
            InitializeRecognizer();

            await Webcam.InitializeCameraAsync();
            captureElement.Source = Webcam.mediaCapture;
            await Webcam.StartCameraPreview();

            await face.InitListAsync();

            GetBusInfo();
#if PC_MODE
            Getschedule();
#endif
        }


    }
}
