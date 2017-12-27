using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using DSSearchRank;
using Windows.UI.Xaml.Controls;

namespace Wimi
{
    public partial class MainPage : Page
    {
        DispatcherTimer SearchRankTimer;
        SearchRank searchRank = new SearchRank();

        DispatcherTimer RankExploreTimer;

        private void InitSearchTimer()
        {
            asyncSetRankListViewAsync();
            SearchRankTimer = new DispatcherTimer();
            SearchRankTimer.Tick += SearchRankTimer_Tick;
            SearchRankTimer.Interval = new TimeSpan(0, 0, 60);
            SearchRankTimer.Start();

            RankExploreTimer = new DispatcherTimer();
            RankExploreTimer.Tick += RankExploreTimer_Tick;
            RankExploreTimer.Interval = new TimeSpan(0, 0, 3);
            RankExploreTimer.Start();
        }

        private void RankExploreTimer_Tick(object sender, object e)
        {
            int p = (CarouselSearchRank.SelectedIndex + 1) % 20;
            CarouselSearchRank.SelectedIndex = p;
        }

        private void SearchRankTimer_Tick(object sender, object e)
        {
            asyncSetRankListViewAsync();
        }
        private async void asyncSetRankListViewAsync()
        {
            List<RankUnit> lstRankUnit = await searchRank.getSearchRank();
            if (lstRankUnit.Count != 0)
            {
                await this.dispatcher.TryRunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    lvSearchRank.ItemsSource = lstRankUnit;
                    CarouselSearchRank.Items.Clear();
                    foreach(var e in lstRankUnit)
                    {
                        CarouselSearchRank.Items.Add(e);
                    }
                });
            }
        }
    }
}