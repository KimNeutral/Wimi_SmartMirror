using DSLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Wimi
{
    public partial class MainPage : Page
    {
        LocationHelper LocationHelper = new LocationHelper();
        DispatcherTimer LocationTimer = null;

        public void ShowLocationAsync()
        {
            if(LocationTimer == null)
            {
                LocationTimer = new DispatcherTimer();
                LocationTimer.Interval = new TimeSpan(0, 0, 1);
                LocationTimer.Tick += LocationTimer_TickAsync;
            }
            gridLocation.Visibility = Visibility.Visible;
            LocationTimer.Start();
        }

        private async void LocationTimer_TickAsync(object sender, object e)
        {
            if(gridLocation.Visibility == Visibility.Collapsed)
            {
                LocationTimer.Stop();
            }

            List<User> users = await LocationHelper.getLocationInfosAsync();

            if(users != null)
            {
                var groups = from c in users
                             group c by c.location.pname;
                cvs.Source = groups;

                lvLocation.SelectedIndex = -1;
            }
        }
    }
}