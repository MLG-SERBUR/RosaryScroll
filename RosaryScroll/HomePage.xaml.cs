using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace RosaryScroll
{
    public sealed partial class HomePage : Page
    {
        private readonly string _todaySetName;

        public HomePage()
        {
            InitializeComponent();
            _todaySetName = GetTodayMysterySet();
            SetupTiles();
        }

        private void SetupTiles()
        {
            TodaySetName.Text = _todaySetName + " Mysteries";
            TodayDayLabel.Text = DateTime.Now.ToString("dddd");
            TodayImage.Source = GetSetImage(_todaySetName);
        }

        private static string GetTodayMysterySet()
        {
            switch (DateTime.Now.DayOfWeek)
            {
                case DayOfWeek.Monday:
                case DayOfWeek.Saturday:
                    return "Joyful";
                case DayOfWeek.Tuesday:
                case DayOfWeek.Friday:
                    return "Sorrowful";
                case DayOfWeek.Wednesday:
                case DayOfWeek.Sunday:
                    return "Glorious";
                case DayOfWeek.Thursday:
                    return "Luminous";
                default:
                    return "Joyful";
            }
        }

        private static BitmapImage GetSetImage(string setName)
        {
            string prefix;
            switch (setName)
            {
                case "Joyful": prefix = "joyful-annunciation"; break;
                case "Luminous": prefix = "luminous-baptism"; break;
                case "Sorrowful": prefix = "sorrowful-agony"; break;
                case "Glorious": prefix = "glorious-resurrection"; break;
                default: prefix = "joyful-annunciation"; break;
            }

            return new BitmapImage(new Uri("ms-appx:///Assets/Mysteries/" + prefix + "-01.jpg"));
        }

        private void TodayTile_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage), _todaySetName);
        }

        private void MysteryTile_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button?.Tag != null)
            {
                Frame.Navigate(typeof(MainPage), button.Tag.ToString());
            }
        }
    }
}
