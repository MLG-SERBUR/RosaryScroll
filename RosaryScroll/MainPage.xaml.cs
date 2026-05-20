using System.Collections.Generic;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

namespace RosaryScroll
{
    public sealed partial class MainPage : Page
    {
        private readonly List<MysteryGroup> _mysteries;
        private List<RosaryImage> _prayerSlides;
        private Visibility _uiVisibility = Visibility.Collapsed;
        private string _setName = "Joyful";

        public MainPage()
        {
            InitializeComponent();

            _mysteries = RosaryData.CreateMysteries();
            _prayerSlides = RosaryData.CreatePrayerSlides(_mysteries, _setName);

            PrayerFlipView.ItemsSource = _prayerSlides;
            MysteryFlipView.ItemsSource = _mysteries;
            UpdateHeader();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // Show back button in title bar on PC
            var navManager = SystemNavigationManager.GetForCurrentView();
            navManager.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            navManager.BackRequested += OnBackRequested;

            if (e.Parameter is string setName && !string.IsNullOrEmpty(setName))
            {
                _setName = setName;
                _prayerSlides = RosaryData.CreatePrayerSlides(_mysteries, _setName);
                PrayerFlipView.ItemsSource = _prayerSlides;
                PrayerFlipView.SelectedIndex = 0;
                UpdateHeader();
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            var navManager = SystemNavigationManager.GetForCurrentView();
            navManager.BackRequested -= OnBackRequested;
            navManager.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
        }

        private void OnBackRequested(object sender, BackRequestedEventArgs e)
        {
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
                e.Handled = true;
            }
        }

        private bool IsPrayerMode
        {
            get { return ModeComboBox.SelectedIndex == 0; }
        }

        private void ModeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PrayerFlipView == null || MysteryFlipView == null)
            {
                return;
            }

            PrayerFlipView.Visibility = IsPrayerMode ? Visibility.Visible : Visibility.Collapsed;
            MysteryFlipView.Visibility = IsPrayerMode ? Visibility.Collapsed : Visibility.Visible;
            UpdateHeader();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Focus(FocusState.Programmatic);
        }

        private void PrayerFlipView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateHeader();
        }

        private void MysteryFlipView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateHeader();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
            }
        }

        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            MoveBy(-1);
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            MoveBy(1);
        }

        private void Page_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.GamepadDPadRight || e.Key == VirtualKey.GamepadLeftThumbstickRight || e.Key == VirtualKey.Right)
            {
                MoveBy(1);
                e.Handled = true;
            }
            else if (e.Key == VirtualKey.GamepadDPadLeft || e.Key == VirtualKey.GamepadLeftThumbstickLeft || e.Key == VirtualKey.Left)
            {
                MoveBy(-1);
                e.Handled = true;
            }
            else if (e.Key == VirtualKey.GamepadB || e.Key == VirtualKey.Escape)
            {
                if (Frame.CanGoBack)
                {
                    Frame.GoBack();
                    e.Handled = true;
                }
            }
        }

        private void FlipView_Tapped(object sender, TappedRoutedEventArgs e)
        {
            _uiVisibility = _uiVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
            if (TopControlsGrid != null)
            {
                TopControlsGrid.Visibility = _uiVisibility;
            }

            if (_mysteries != null)
            {
                foreach (var m in _mysteries)
                {
                    m.ControlsVisibility = _uiVisibility;
                }
            }

            if (_prayerSlides != null)
            {
                foreach (var s in _prayerSlides)
                {
                    s.ControlsVisibility = _uiVisibility;
                }
            }
        }

        private void MoveBy(int delta)
        {
            FlipView active = IsPrayerMode ? PrayerFlipView : MysteryFlipView;
            int nextIndex = active.SelectedIndex + delta;

            if (nextIndex < 0)
            {
                nextIndex = 0;
            }
            else if (nextIndex >= active.Items.Count)
            {
                nextIndex = active.Items.Count - 1;
            }

            active.SelectedIndex = nextIndex;
            UpdateHeader();
        }

        private void UpdateHeader()
        {
            if (TitleText == null || ProgressText == null || _mysteries == null || _prayerSlides == null || _prayerSlides.Count == 0)
            {
                return;
            }

            if (IsPrayerMode)
            {
                int selected = PrayerFlipView.SelectedIndex < 0 ? 0 : PrayerFlipView.SelectedIndex;
                RosaryImage slide = _prayerSlides[selected];
                TitleText.Text = slide.MysteryName;
                ProgressText.Text = _setName + " - Hail Mary " + slide.DecadePrayerNumber + " of 10 - bead " + (selected + 1) + " of " + _prayerSlides.Count;
            }
            else
            {
                int selected = MysteryFlipView.SelectedIndex < 0 ? 0 : MysteryFlipView.SelectedIndex;
                MysteryGroup mystery = _mysteries[selected];
                TitleText.Text = mystery.Name;
                ProgressText.Text = "Mystery " + (selected + 1) + " of " + _mysteries.Count + " - browse images";
            }
        }
    }
}
