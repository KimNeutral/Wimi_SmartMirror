using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using DSHue;

namespace Wimi
{
    public partial class MainPage : Page
    {
        Hue HueControl = new Hue(); //Hue제어 하기 위해 생성
        bool HueAtrBool = false; //Hue제어 할때 await를
        
        public async void HueInit()
        {
            HueAtrBool = await HueControl.Init();
        }
    }
}
