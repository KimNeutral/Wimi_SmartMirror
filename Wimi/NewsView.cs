using DSNews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace Wimi
{
    public partial class MainPage : Page
    {
        NewsHeadline news = new NewsHeadline();
        List<News> lNewsList = new List<News>();

        async Task GetNewsInfo()
        {
            lNewsList = await news.GetDaumNewsAsync();
            lbNewsInfo.ItemsSource = lNewsList;
        }

        private void ShowNews()
        {
            ClearPanel();
            gridNews.Visibility = Windows.UI.Xaml.Visibility.Visible;
        }
    }
}
