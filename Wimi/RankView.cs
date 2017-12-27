using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using DSSearchRank;
using Windows.UI.Xaml.Controls;

namespace Wimi
{
    public partial class MainPage : Page
    {
        DispatcherTimer SearchRankTimer;
        SearchRank searchRank = new SearchRank();
        private void InitSearchTimer()
        {
            asyncSetRankListViewAsync();
            SearchRankTimer = new DispatcherTimer();
            SearchRankTimer.Tick += SearchRankTimer_Tick;
            SearchRankTimer.Interval = new TimeSpan(0, 0, 30);
            SearchRankTimer.Start();
        }
        private void SearchRankTimer_Tick(object sender, object e)
        {
            asyncSetRankListViewAsync();
        }
        private async void asyncSetRankListViewAsync()
        {
            List<RankUnit> lstRankUnit = await searchRank.getSearchRank();
            lvSearchRank.ItemsSource = lstRankUnit;
        }
    }
}