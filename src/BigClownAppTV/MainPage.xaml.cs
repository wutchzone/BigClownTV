using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using BigClownAppTV.View;
using BigClownAppTV.ViewModel;
using Syncfusion.UI.Xaml.Charts;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace BigClownAppTV
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private UIElement _graph;
        private Frame _settings;

        public MainPage()
        {
            this.InitializeComponent();

            _graph = MySplitView.Content;
            _settings = new Frame();
            _settings.Navigate(typeof(SettingsPage));
        }

        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            MySplitView.IsPaneOpen = !MySplitView.IsPaneOpen;
        }


        private void SettingsButton_OnClick(object sender, RoutedEventArgs e)
        {
            MySplitView.Content = _settings;

        }

        private void HomeButton_OnClick(object sender, RoutedEventArgs e)
        {
            MySplitView.Content = _graph;
        }
    }
}
