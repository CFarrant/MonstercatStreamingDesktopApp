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
            List<Album> albums = MainPage.albums;
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
    }
}
