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
using Windows.UI.Xaml.Navigation;

namespace MonstercatDesktopStreamingApp.Pages
{
    public sealed partial class RegisterPage : Page
    {
        public RegisterPage()
        {
            this.InitializeComponent();
        }

        public async void Register_ClickedAsync(object sender, RoutedEventArgs e)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(@"http://www.monstercatstreaming.tk:8080");
                //httpClient.BaseAddress = new Uri(@"http://localhost:8080");
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("utf-8"));
                string endpoint = @"/api/user";
                //string endpoint = @"/user";

                if (password.Text.Equals(confirmPassword.Text))
                {
                    httpClient.DefaultRequestHeaders.Add("Authorization", MainPage.Base64Encode(username.Text + ":" + password.Text));
                    if (email.Text.Count() == 0)
                    {
                        DisplayMissingEmailDialog();
                    }
                    else
                    {
                        POSTUser u = new POSTUser
                        {
                            email = email.Text,
                            username = username.Text
                        };

                        try
                        {
                            HttpContent content = new StringContent(JsonConvert.SerializeObject(u), Encoding.UTF8, "application/json");
                            HttpResponseMessage response = await httpClient.PostAsync(endpoint, content);

                            if (response.IsSuccessStatusCode)
                            {
                                MainPage.authentication = MainPage.Base64Encode(username.Text + ":" + password.Text);
                                MainPage.window.Navigate(typeof(LoginPage));
                            }
                            else
                            {
                                httpClient.CancelPendingRequests();
                                DisplayInvalidRegistrationDialog();
                            }
                        }
                        catch (Exception) { }
                    }
                }
                else
                {
                    DisplayMismatchingPasswordDialog();
                }
            }
        }

        private async void DisplayInvalidRegistrationDialog()
        {
            ContentDialog invalidLogin = new ContentDialog
            {
                RequestedTheme = ElementTheme.Dark,
                Title = "Email/Username already in Use",
                Content = "Please try again!",
                CloseButtonText = "Ok"
            };

            ContentDialogResult result = await invalidLogin.ShowAsync();
        }

        private async void DisplayMismatchingPasswordDialog()
        {
            ContentDialog invalidLogin = new ContentDialog
            {
                RequestedTheme = ElementTheme.Dark,
                Title = "The New Passowrd and the Confirm New Password values do not Math",
                Content = "Please try again!",
                CloseButtonText = "Ok"
            };

            ContentDialogResult result = await invalidLogin.ShowAsync();
        }

        private async void DisplayMissingEmailDialog()
        {
            ContentDialog invalidLogin = new ContentDialog
            {
                RequestedTheme = ElementTheme.Dark,
                Title = "The Email Value is Required",
                Content = "Please try again!",
                CloseButtonText = "Ok"
            };

            ContentDialogResult result = await invalidLogin.ShowAsync();
        }
    }
}
