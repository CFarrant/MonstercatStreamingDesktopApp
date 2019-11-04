using MonstercatDesktopStreamingApp.Objects;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace MonstercatDesktopStreamingApp.Pages
{
    public sealed partial class LibraryView : Page
    {
        public LibraryView()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            List<Album> albums;

            if (e.Parameter == null)
            {
                albums = MainPage.albums;
            }
            else
            {
                albums = (List<Album>)e.Parameter;
            }

            foreach(Album a in albums)
            {
                albumsViewer.Items.Add(new LibraryObject(a));
            }
        }

        #region Navigation
        private void AlbumsViewer_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (MainPage.window.CurrentSourcePageType != typeof(AlbumView))
            {
                MainPage.window.Navigate(typeof(AlbumView), e.ClickedItem);
            }
        }
        #endregion

        private void SearchButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            List<Album> results = new List<Album>();

            switch (queryType.SelectedIndex)
            {
                //Album
                case 0:
                    foreach (Album a in MainPage.albums)
                    {
                        if (a.name.ToLower().Contains(queryContent.Text.ToLower()))
                        {
                            results.Add(a);
                        }
                    }
                    break;
                //Artist
                case 1:
                    foreach (Album a in MainPage.albums)
                    {
                        if (a.artist.name.ToLower().Contains(queryContent.Text.ToLower()))
                        {
                            results.Add(a);
                        }
                    }
                    break;
                //Song
                case 2:
                    break;
                //Genre
                case 3:
                    foreach (Album a in MainPage.albums)
                    {
                        if (a.genreprimary.ToLower().Contains(queryContent.Text.ToLower()) || a.genresecondary.ToLower().Contains(queryContent.Text.ToLower()))
                        {
                            results.Add(a);
                        }
                    }
                    break;
                default:
                    break;
            }

            MainPage.searchedLibrary = true;
            MainPage.window.Navigate(typeof(LibraryView), results);
        }
    }
}
