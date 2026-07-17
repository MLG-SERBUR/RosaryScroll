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
        private Visibility _uiVisibility = Visibility.Collapsed;
        private string _setName = "Joyful";

        public MainPage()
        {
            InitializeComponent();

            _mysteries = RosaryData.CreateMysteries();
            MysteryPivot.ItemsSource = _mysteries.FindAll(m => m.SetName == _setName);
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

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Focus(FocusState.Programmatic);
        }

        private void MysteryPivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateHeader();
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
        }

        private void MoveBy(int delta)
        {
            int selectedIndex = MysteryPivot.SelectedIndex;
            int itemCount = MysteryPivot.Items.Count;
            int nextIndex = selectedIndex + delta;

            if (nextIndex < 0)
            {
                nextIndex = 0;
            }
            else if (nextIndex >= itemCount)
            {
                nextIndex = itemCount - 1;
            }

            MysteryPivot.SelectedIndex = nextIndex;

            UpdateHeader();
        }

        private void UpdateHeader()
        {
            if (TitleText == null || ProgressText == null || InstructionText == null || _mysteries == null)
            {
                return;
            }

            int selected = MysteryPivot.SelectedIndex < 0 ? 0 : MysteryPivot.SelectedIndex;
            var currentSetMysteries = MysteryPivot.ItemsSource as List<MysteryGroup>;
            if (currentSetMysteries != null && selected >= 0 && selected < currentSetMysteries.Count)
            {
                MysteryGroup mystery = currentSetMysteries[selected];
                TitleText.Text = mystery.Name;
                ProgressText.Text = "Mystery " + (selected + 1) + " of " + currentSetMysteries.Count;
            }
        }
    }
}
