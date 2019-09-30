using MonstercatDesktopStreamingApp.Objects;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
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
    public sealed partial class ForgotPage : Page
    {
        public ForgotPage()
        {
            this.InitializeComponent();
            this.monstercatLogo.Source = new BitmapImage(new Uri("http://www.sclance.com/pngs/monstercat-logo-png/monstercat_logo_png_895099.png"));
        }

        public async void Submit_ClickedAsync(object sender, RoutedEventArgs e)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(@"http://www.monstercatstreaming.tk:8080");
                //httpClient.BaseAddress = new Uri(@"http://localhost:8080");
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("utf-8"));
                string endpoint = @"/api/user/forgot";
                //string endpoint = @"/user/forgot";

                if (!username.Text.Equals("") && !email.Text.Equals(""))
                {
                    POSTUser user = new POSTUser
                    {
                        username = username.Text,
                        email = email.Text
                    };

                    HttpContent content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");

                    try
                    {
                        HttpResponseMessage response = await httpClient.PostAsync(endpoint, content);

                        if (response.IsSuccessStatusCode)
                        {
                            DisplayCheckEmailDialog();
                            MainPage.window.Navigate(typeof(LoginPage));
                        }
                    }
                    catch (Exception) { }
                }
                else 
                {
                    DisplayMissingInfoDialog();
                }
            } 
        }

        private async void DisplayCheckEmailDialog()
        {
            ContentDialog invalidLogin = new ContentDialog
            {
                RequestedTheme = ElementTheme.Dark,
                Title = "A Temporary Password has been Sent to Your Email!",
                Content = "Please attempt Login",
                CloseButtonText = "Ok"
            };

            ContentDialogResult result = await invalidLogin.ShowAsync();
        }

        private async void DisplayMissingInfoDialog()
        {
            ContentDialog invalidLogin = new ContentDialog
            {
                RequestedTheme = ElementTheme.Dark,
                Title = "Both a Username and Email are required to Reset Password",
                Content = "Please try again!",
                CloseButtonText = "Ok"
            };

            ContentDialogResult result = await invalidLogin.ShowAsync();
        }
    }
}
