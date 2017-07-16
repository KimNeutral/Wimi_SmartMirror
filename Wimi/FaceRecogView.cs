using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSFace;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Storage;
using System.IO;

namespace Wimi
{
    public sealed partial class MainPage : Page
    {
        WebcamHelper Webcam = new WebcamHelper();
        FaceRec face = new FaceRec();

        private async void btnCapture_Click(object sender, RoutedEventArgs e)
        {
            if (!Webcam.IsInitialized())
            {
                return;
            }

            StorageFile captured = await Webcam.CapturePhoto();
            using (Stream s = await captured.OpenStreamForReadAsync())
            {
                string[] faces = await face.GetIdentifiedNameAsync(s);
                if (faces.Count() > 0)
                {
                    tbFaceName.Text = faces.FirstOrDefault();
                }
            }
        }
    }
}
