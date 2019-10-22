using MonstercatDesktopStreamingApp.Objects;
using MonstercatDesktopStreamingApp.Pages;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace MonstercatDesktopStreamingApp.Pages
{
    public sealed partial class LoginPage : Page
    {
        public LoginPage()
        {
            this.InitializeComponent();
            this.monstercatLogo.Source = new BitmapImage(new Uri("https://tr.rbxcdn.com/64807643d96804ad1d51f0446ca59c8a/420/420/Decal/Png"));
            BuildLocalAlbumAsync();
        }

        #region Navigation
        private void ForgotButton_Clicked(object sender, RoutedEventArgs e)
        {
            if (MainPage.window.CurrentSourcePageType != typeof(ForgotPage))
            {
                MainPage.window.Navigate(typeof(ForgotPage));
            }
        }

        private void RegisterButton_Clicked(object sender, RoutedEventArgs e)
        {
            if (MainPage.window.CurrentSourcePageType != typeof(RegisterPage))
            {
                MainPage.window.Navigate(typeof(RegisterPage));
            }
        }
        #endregion

        #region Dialog Windows
        private async void DisplayInvalidLoginDialog()
        {
            ContentDialog invalidLogin = new ContentDialog
            {
                RequestedTheme = ElementTheme.Dark,
                Title = "User Login was Invalid",
                Content = "Please try again!",
                CloseButtonText = "Ok"
            };

            ContentDialogResult result = await invalidLogin.ShowAsync();
        }
        #endregion

        #region API Calls
        private async void LoginButton_ClickedAsync(object sender, RoutedEventArgs e)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(@"http://www.monstercatstreaming.tk:8080");
                //httpClient.BaseAddress = new Uri(@"http://localhost:8080");
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("utf-8"));
                httpClient.DefaultRequestHeaders.Add("Authorization", MainPage.Base64Encode(username.Text + ":" + password.Text));
                string endpoint = @"/api/user/login";
                //string endpoint = @"/user/login";

                try
                {
                    HttpResponseMessage response = await httpClient.GetAsync(endpoint);
                    string json = await response.Content.ReadAsStringAsync();
                    GETUser u = JsonConvert.DeserializeObject<GETUser>(json);

                    if (response.IsSuccessStatusCode)
                    {
                        MainPage.authentication = MainPage.Base64Encode(username.Text+":"+password.Text);
                        if (u.recovery == true)
                        {
                            MainPage.window.Navigate(typeof(ChangePage));
                        }
                        else
                        {
                            MainPage.window.Navigate(typeof(LibraryView));
                        }
                    }
                    else
                    {
                        httpClient.CancelPendingRequests();
                        DisplayInvalidLoginDialog();
                    }
                }
                catch (Exception) {}
            }
        }

        private async void BuildLocalAlbumAsync()
        {
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(@"http://www.monstercatstreaming.tk:8080");
                //httpClient.BaseAddress = new Uri(@"http://localhost:8080");
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("utf-8"));
                string endpoint = @"/api/album";
                //string endpoint = @"/album";
                string json = "";

                try
                {
                    HttpResponseMessage response = await httpClient.GetAsync(endpoint);
                    response.EnsureSuccessStatusCode();
                    json = await response.Content.ReadAsStringAsync();

                    JArray jArray = JArray.Parse(json);
                    foreach (JObject item in jArray)
                    {
                        JObject albArt = (JObject)item.Last.First;

                        MainPage.albums.Add(new Album()
                        {
                            id = (string)item.GetValue("id"),
                            name = (string)item.GetValue("name"),
                            type = (string)item.GetValue("type"),
                            releaseCode = (string)item.GetValue("releaseCode"),
                            genreprimary = (string)item.GetValue("genreprimary"),
                            genresecondary = (string)item.GetValue("genresecondary"),
                            coverURL = (string)item.GetValue("coverURL"),
                            artist = new Artist()
                            {
                                name = (string)albArt.GetValue("name")
                            }
                        });
                    }
                }
                catch (Exception) { }
            }
        }
        #endregion
    }
}