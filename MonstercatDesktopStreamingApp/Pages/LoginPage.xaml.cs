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
            this.monstercatLogo.Source = new BitmapImage(new Uri("http://www.sclance.com/pngs/monstercat-logo-png/monstercat_logo_png_895099.png"));
        }

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
                        BuildLocalTrackAsync();
                        BuildLocalAlbumAsync();
                        MainPage.authentication = MainPage.Base64Encode(username.Text+":"+password.Text);
                        if (u.recovery == true)
                        {
                            MainPage.window.Navigate(typeof(ChangePage));
                        }
                        else
                        {
                            MainPage.window.Navigate(typeof(HomePage));
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

        private async void BuildLocalTrackAsync()
        {
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(@"http://www.monstercatstreaming.tk:8080");
                //httpClient.BaseAddress = new Uri(@"http://localhost:8080");
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("utf-8"));
                string endpoint = @"/api/song";
                //string endpoint = @"/song";
                string json = "";

                try
                {
                    HttpResponseMessage response = await httpClient.GetAsync(endpoint);
                    response.EnsureSuccessStatusCode();
                    json = await response.Content.ReadAsStringAsync();

                    JArray jArray = JArray.Parse(json);
                    foreach(JObject item in jArray)
                    {
                        JProperty songArt = (JProperty)item.First.Next.Next.Next.Next.Next.Next;
                        JObject alb = (JObject)item.Last.First;
                        JObject albArt = (JObject)alb.Last.First;

                        MainPage.tracks.Add(new Track
                        {
                            id = (string)item.GetValue("id"),
                            tracknumber = (int)item.GetValue("tracknumber"),
                            title = (string)item.GetValue("title"),
                            genreprimary = (string)item.GetValue("genreprimary"),
                            genresecondary = (string)item.GetValue("genresecondary"),
                            songURL = (string)item.GetValue("songURL"),
                            artist = new Artist()
                            {
                                name = (string)((JObject)songArt.First).GetValue("name")
                            },
                            album = new Album()
                            {
                                id = (string)alb.GetValue("id"),
                                name = (string)alb.GetValue("name"),
                                type = (string)alb.GetValue("type"),
                                releaseCode = (string)alb.GetValue("releaseCode"),
                                genreprimary = (string)alb.GetValue("genreprimary"),
                                genresecondary = (string)alb.GetValue("genresecondary"),
                                coverURL = (string)alb.GetValue("coverURL"),
                                artist = new Artist()
                                {
                                    name = (string)albArt.GetValue("name")
                                }
                            }
                        });
                    }
                }
                catch (Exception) { }
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
    }
}