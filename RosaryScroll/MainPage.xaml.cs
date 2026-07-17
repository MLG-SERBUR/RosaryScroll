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
        private List<MysteryGroup> _currentSetMysteries;
        private int _currentMysteryIndex;

        public MainPage()
        {
            InitializeComponent();

            _mysteries = RosaryData.CreateMysteries();
            SetMysterySet(_setName);
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
                SetMysterySet(_setName);
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
            int itemCount = _currentSetMysteries == null ? 0 : _currentSetMysteries.Count;
            int nextIndex = _currentMysteryIndex + delta;

            if (itemCount == 0)
            {
                return;
            }

            if (nextIndex < 0)
            {
                nextIndex = 0;
            }
            else if (nextIndex >= itemCount)
            {
                nextIndex = itemCount - 1;
            }

            ShowMystery(nextIndex);
        }

        private void UpdateHeader()
        {
            if (TitleText == null || ProgressText == null || InstructionText == null || _mysteries == null)
            {
                return;
            }

            if (_currentSetMysteries != null && _currentMysteryIndex >= 0 && _currentMysteryIndex < _currentSetMysteries.Count)
            {
                MysteryGroup mystery = _currentSetMysteries[_currentMysteryIndex];
                TitleText.Text = mystery.Name;
                ProgressText.Text = "Mystery " + (_currentMysteryIndex + 1) + " of " + _currentSetMysteries.Count;
            }
        }

        private void SetMysterySet(string setName)
        {
            _currentSetMysteries = _mysteries.FindAll(m => m.SetName == setName);
            ShowMystery(0);
        }

        private void ShowMystery(int index)
        {
            if (_currentSetMysteries == null || index < 0 || index >= _currentSetMysteries.Count)
            {
                return;
            }

            _currentMysteryIndex = index;
            MysteryContent.Content = _currentSetMysteries[index];
            UpdateHeader();
        }
    }
}
