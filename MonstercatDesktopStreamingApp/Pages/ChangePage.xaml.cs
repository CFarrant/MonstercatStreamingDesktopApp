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
using System.Threading.Tasks;
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
    public sealed partial class ChangePage : Page
    {
        public ChangePage()
        {
            this.InitializeComponent();
        }

        private async void SaveButton_ClickedAsync(object sender, RoutedEventArgs e)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(@"http://www.monstercatstreaming.tk:8080");
                //httpClient.BaseAddress = new Uri(@"http://localhost:8080");
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("utf-8"));
                httpClient.DefaultRequestHeaders.Add("Authorization", MainPage.Base64Encode(username.Text + ":" + password.Text));
                string endpoint = @"/api/user/change";
                //string endpoint = @"/user/change";

                if (newPassword.Text.Equals(confirmNewPassword.Text))
                {
                    if (email.Text.Count() == 0)
                    {
                        DisplayMissingEmailDialog();
                    }
                    else
                    {
                        PUTUser u = new PUTUser
                        {
                            email = email.Text,
                            password = newPassword.Text
                        };

                        try
                        {
                            HttpContent content = new StringContent(JsonConvert.SerializeObject(u), Encoding.UTF8, "application/json");
                            HttpResponseMessage response = await httpClient.PutAsync(endpoint, content);

                            if (response.IsSuccessStatusCode)
                            {
                                if (MainPage.authentication.Equals(""))
                                {
                                    MainPage.window.Navigate(typeof(LoginPage));
                                }
                                else
                                {
                                    MainPage.authentication = MainPage.Base64Encode(username.Text + ":" + newPassword.Text);
                                    MainPage.window.Navigate(typeof(LibraryView));
                                }
                            }
                            else
                            {
                                httpClient.CancelPendingRequests();
                                DisplayInvalidLoginDialog();
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
