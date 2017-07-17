using DSBus;
using DSEmotion;
using DSMusic;
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
        Bus bus = new Bus();

        Music mu = new Music();

        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void BusText_Loaded(object sender, RoutedEventArgs e)
        {
            bool yn = await bus.LoadBusInfo();

            if (yn)
            {
                foreach(var t in bus.BusListInfo)
                {
                    Debug.WriteLine(t.note);
                    Debug.WriteLine(t.number);
                    Debug.WriteLine(t.position);
                    Debug.WriteLine(t.state);
                }
            }
        }

        private async void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            initSynthesizer();
            InitializeRecognizer();

            await Webcam.InitializeCameraAsync();
            captureElement.Source = Webcam.mediaCapture;
            await Webcam.StartCameraPreview();

            await face.InitListAsync();
        }


    }
}
