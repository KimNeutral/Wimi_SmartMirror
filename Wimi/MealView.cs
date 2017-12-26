using SchoolMeal;
using SchoolMeal.Exception;
using Windows.UI.Xaml.Controls;
using System.Collections.Generic;
using System;

namespace Wimi
{
    public partial class MainPage : Page
    {
        List<MealMenu> MealList = new List<MealMenu>();
        private int[] mealTime = new int[] { 8, 13, 20 };

        public void ShowMeal()
        {
            Meal meal = new Meal(Regions.Daegu, SchoolType.High, "D100000282");
            gridMeal.Visibility = Windows.UI.Xaml.Visibility.Visible;
            MealList = meal.GetMealMenu();
            var menu = MealList.Find(x => x.Date.Equals(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day)));
            var nextMenu = MealList.Find(x => x.Date.Equals(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day).AddDays(1)));
            lvMealBreakfast.ItemsSource = menu.Breakfast;
            lvMealLunch.ItemsSource = menu.Lunch;
            lvMealDinner.ItemsSource = menu.Dinner;
            lvMealNextBreakFast.ItemsSource = nextMenu.Breakfast;
        }
    }
}
