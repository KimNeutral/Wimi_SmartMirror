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
            lvMealBreakfast.ItemsSource = MealList[DateTime.Now.Day-1].Breakfast;
            lvMealLunch.ItemsSource = MealList[DateTime.Now.Day-1].Lunch;
            lvMealDinner.ItemsSource = MealList[DateTime.Now.Day-1].Dinner;
        }
    }
}
