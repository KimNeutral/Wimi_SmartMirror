using DSBus;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Wimi
{
    public partial class MainPage : Page
    {
        Bus bus = new Bus();
        List<BusInfo> lBusInfo = new List<BusInfo>();

        async void GetBusInfo()
        {
            lBusInfo = await bus.LoadBusInfo();
            if(lBusInfo != null)
            {
                lbBusInfo.ItemsSource = lBusInfo;
            }
        }
    }
}
