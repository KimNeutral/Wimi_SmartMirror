using DSBus;
using System.Collections.Generic;
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
#if false // for test
            lBusInfo.Clear();
            lBusInfo.Add(new BusInfo()
            {
                number = "급행5",
                state = "전",
                position = "대곡방면"
            });

            lBusInfo.Add(new BusInfo()
            {
                number = "600",
                state = "전전",
                position = "대곡방면"
            });

            lBusInfo.Add(new BusInfo()
            {
                number = "급행4",
                state = "3분전",
                position = "대곡방면"
            });
#endif
            if (lBusInfo != null)
            {
                lbBusInfo.ItemsSource = lBusInfo;
            }
        }

        private void ShowBus()
        {
            ClearPanel();
            gridBus.Visibility = Windows.UI.Xaml.Visibility.Visible;
        }
    }
}
