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
            MysteryPivot.ItemsSource = _mysteries.FindAll(m => m.SetName == _setName);
            PrayerFlipView.Visibility = Visibility.Collapsed;
            MysteryPivot.Visibility = Visibility.Visible;
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
                MysteryPivot.ItemsSource = _mysteries.FindAll(m => m.SetName == _setName);
                MysteryPivot.SelectedIndex = 0;
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
            get { return ModeToggleButton == null || ModeToggleButton.IsChecked != true; }
        }

        private void ModeToggleButton_Click(object sender, RoutedEventArgs e)
        {
            ApplyMode(!IsPrayerMode);
        }

        private void ModeButton_Click(object sender, RoutedEventArgs e)
        {
            ApplyMode(!IsPrayerMode);
        }

        private void ApplyMode(bool prayerMode)
        {
            if (PrayerFlipView == null || MysteryPivot == null || ModeToggleButton == null)
            {
                return;
            }

            ModeToggleButton.IsChecked = !prayerMode;
            ModeToggleButton.Label = prayerMode ? "Mystery Browse" : "Rosary Progress";
            PrayerFlipView.Visibility = prayerMode ? Visibility.Visible : Visibility.Collapsed;
            MysteryPivot.Visibility = prayerMode ? Visibility.Collapsed : Visibility.Visible;

            if (prayerMode)
            {
                var currentSetMysteries = MysteryPivot.ItemsSource as List<MysteryGroup>;
                int mysteryIndex = MysteryPivot.SelectedIndex;
                if (currentSetMysteries != null && mysteryIndex >= 0 && mysteryIndex < currentSetMysteries.Count)
                {
                    int browseImageIndex = currentSetMysteries[mysteryIndex].ActiveImageIndex;
                    int beadIndex = System.Math.Min(browseImageIndex, 9);
                    int prayerIndex = mysteryIndex * 10 + beadIndex;
                    if (prayerIndex >= 0 && prayerIndex < _prayerSlides.Count)
                    {
                        PrayerFlipView.SelectedIndex = prayerIndex;
                    }
                }
            }
            else
            {
                int prayerIndex = PrayerFlipView.SelectedIndex;
                if (prayerIndex >= 0)
                {
                    int mysteryIndex = prayerIndex / 10;
                    int beadIndex = prayerIndex % 10;

                    var currentSetMysteries = MysteryPivot.ItemsSource as List<MysteryGroup>;
                    if (currentSetMysteries != null && mysteryIndex >= 0 && mysteryIndex < currentSetMysteries.Count)
                    {
                        int availableCount = currentSetMysteries[mysteryIndex].Images.Count;
                        if (availableCount > 0)
                        {
                            currentSetMysteries[mysteryIndex].ActiveImageIndex = beadIndex % availableCount;
                        }
                        MysteryPivot.SelectedIndex = mysteryIndex;
                    }
                }
            }
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

        private void MysteryPivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateHeader();
        }

        private void MysteryImageFlipView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var flipView = sender as FlipView;
            var mystery = flipView?.DataContext as MysteryGroup;
            if (flipView == null || mystery == null || mystery.Images == null || mystery.Images.Count == 0)
            {
                return;
            }

            if (flipView.SelectedIndex >= mystery.Images.Count)
            {
                mystery.ActiveImageIndex = 0;
                flipView.SelectedIndex = 0;
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
            }
        }

        private void PreviousMysteryButton_Click(object sender, RoutedEventArgs e)
        {
            MoveBy(-1);
        }

        private void NextMysteryButton_Click(object sender, RoutedEventArgs e)
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
            if (TopOverlayGrid != null)
            {
                TopOverlayGrid.Visibility = _uiVisibility;
            }
            if (ProgressText != null)
            {
                ProgressText.Visibility = _uiVisibility;
            }
            if (_mysteries != null)
            {
                foreach (var m in _mysteries)
                {
                    m.ControlsVisibility = _uiVisibility;
                    if (m.Images != null)
                    {
                        foreach (var img in m.Images)
                        {
                            img.ControlsVisibility = _uiVisibility;
                        }
                    }
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
            int selectedIndex = IsPrayerMode ? PrayerFlipView.SelectedIndex : MysteryPivot.SelectedIndex;
            int itemCount = IsPrayerMode ? PrayerFlipView.Items.Count : MysteryPivot.Items.Count;
            int nextIndex = selectedIndex + delta;

            if (nextIndex < 0)
            {
                nextIndex = 0;
            }
            else if (nextIndex >= itemCount)
            {
                nextIndex = itemCount - 1;
            }

            if (IsPrayerMode)
            {
                PrayerFlipView.SelectedIndex = nextIndex;
            }
            else
            {
                MysteryPivot.SelectedIndex = nextIndex;
            }

            UpdateHeader();
        }

        private void UpdateHeader()
        {
            if (TitleText == null || ProgressText == null || InstructionText == null || _mysteries == null || _prayerSlides == null || _prayerSlides.Count == 0)
            {
                return;
            }

            if (IsPrayerMode)
            {
                int selected = PrayerFlipView.SelectedIndex < 0 ? 0 : PrayerFlipView.SelectedIndex;
                RosaryImage slide = _prayerSlides[selected];
                TitleText.Text = slide.MysteryName;
                ProgressText.Text = _setName + " - Hail Mary " + slide.DecadePrayerNumber + " of 10 - bead " + (selected + 1) + " of " + _prayerSlides.Count;
                InstructionText.Text = "Swipe up for images.";
            }
            else
            {
                int selected = MysteryPivot.SelectedIndex < 0 ? 0 : MysteryPivot.SelectedIndex;
                var currentSetMysteries = MysteryPivot.ItemsSource as List<MysteryGroup>;
                if (currentSetMysteries != null && selected >= 0 && selected < currentSetMysteries.Count)
                {
                    MysteryGroup mystery = currentSetMysteries[selected];
                    TitleText.Text = mystery.Name;
                    ProgressText.Text = "Mystery " + (selected + 1) + " of " + currentSetMysteries.Count;
                }
                InstructionText.Text = "Swipe up for images.";
            }

            ProgressText.Visibility = _uiVisibility;
            InstructionText.Visibility = _uiVisibility;
        }
    }
}
