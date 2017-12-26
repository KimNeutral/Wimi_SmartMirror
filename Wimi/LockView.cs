using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using BingHelper;
using Windows.UI.Xaml.Media.Imaging;
using Microsoft.Toolkit.Uwp.UI.Animations;
using Windows.UI.Xaml;

namespace Wimi
{
    public partial class MainPage : Page
    {
        public async Task InitLockScreenAsync()
        {
            BingImage bingImage = new BingImage();
            var bingUnit = await bingImage.GetTodayBingUnit(BingImageResolution.LARGE);
            imageLockScreen.Source = new BitmapImage(new Uri(bingUnit.Path));
        }

        public async Task LockScreenAsync()
        {
            await gridLockScreen.Offset(offsetX: 0, offsetY: -(float)Window.Current.Bounds.Height, duration: 0, delay: 0, easingType: EasingType.Linear).StartAsync();
            //gridLockScreen.Visibility = Windows.UI.Xaml.Visibility.Visible;
            await InitLockScreenAsync();
            SetVoice("lock.mp3", true);
            await gridLockScreen.Offset(offsetX: 0, offsetY: 0, duration: 2000, delay: 50, easingType: EasingType.Quintic).StartAsync();
        }

        public async Task UnlockScreenAsync()
        {
            SetVoice("unlock.mp3", true);
            await gridLockScreen.Offset(offsetX: 0, offsetY: -(float)Window.Current.Bounds.Height, duration: 2000, delay: 50, easingType: EasingType.Quintic).StartAsync();
        }
    }
}
