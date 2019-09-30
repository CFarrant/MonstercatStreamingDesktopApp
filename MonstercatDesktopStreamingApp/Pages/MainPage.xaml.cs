using MonstercatDesktopStreamingApp.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace MonstercatDesktopStreamingApp.Pages
{
    public sealed partial class MainPage : Page
    {
        public static string authentication = "";
        public static TextBlock nowPlaying;
        public static Frame window;
        public static WebView audioStreaming;
        public static List<Album> albums;
        public static List<Track> tracks;

        public MainPage()
        {
            this.InitializeComponent();
            window = windowView;
            albums = new List<Album>();
            tracks = new List<Track>();
            nowPlaying = this.nowPlayingTitle;
            audioStreaming = this.audioStreamUI;
            windowView.Navigate(typeof(LoginPage));
        }

        private void Library_Clicked(object sender, RoutedEventArgs e)
        {
            if (windowView.CurrentSourcePageType != typeof(LibraryView) && windowView.CurrentSourcePageType != typeof(LoginPage) 
                && windowView.CurrentSourcePageType != typeof(ForgotPage) && windowView.CurrentSourcePageType != typeof(RegisterPage))
            {
                windowView.Navigate(typeof(LibraryView));
            }
            else if (authentication.Equals(""))
            {
                DisplayNotLoggedInDialog();
            }
        }

        private void Change_Clicked(object sender, RoutedEventArgs e)
        {
            if (windowView.CurrentSourcePageType != typeof(ChangePage) && windowView.CurrentSourcePageType != typeof(LoginPage)
               && windowView.CurrentSourcePageType != typeof(ForgotPage) && windowView.CurrentSourcePageType != typeof(RegisterPage))
            {
                windowView.Navigate(typeof(ChangePage));
            }
            else if (authentication.Equals(""))
            {
                DisplayNotLoggedInDialog();
            }
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return "Basic " + System.Convert.ToBase64String(plainTextBytes);
        }

        private async void DisplayNotLoggedInDialog()
        {
            ContentDialog notLoggedIn = new ContentDialog
            {
                RequestedTheme = ElementTheme.Dark,
                Title = "User Login is Required",
                Content = "Please log in and try again!",
                CloseButtonText = "Ok"
            };

            ContentDialogResult result = await notLoggedIn.ShowAsync();
        }
    }
}
