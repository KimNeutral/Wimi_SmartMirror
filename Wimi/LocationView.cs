using DSLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Windows.UI.Xaml.Controls;

namespace Wimi
{
    public partial class MainPage : Page
    {
        LocationHelper LocationHelper = new LocationHelper();

        public async void ShowLocationAsync()
        {
            List<User> users = await LocationHelper.getLocationInfosAsync();
            lbLocation.ItemsSource = users;
        }
    }
}