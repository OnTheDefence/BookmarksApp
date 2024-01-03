using Bookmarks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Test.UserControls
{
    /// <summary>
    /// Interaction logic for BookmarkItem.xaml
    /// </summary>
    public partial class BookmarkItem : UserControl
    {
        public int _bookmarkID;

        private string _link;

        public BookmarkItem()
        {
            InitializeComponent();
        }

        public void SetBookmarkID(int id) {
            _bookmarkID = id;

            _link = SqliteDataAccess.LoadBookmarkLinkFromID(_bookmarkID).Result;

            LinkText.Text = _link;
            string _linkTitle = SqliteDataAccess.LoadBookmarkTitleFromID(_bookmarkID).Result;
            if (_linkTitle == null) {
                try
                {
                    WebClient x = new WebClient();
                    string source = x.DownloadString(LinkText.Text);
                    string title = Regex.Match(source, @"\<title\b[^>]*\>\s*(?<Title>[\s\S]*?)\</title\>", RegexOptions.IgnoreCase).Groups["Title"].Value;
                    _linkTitle = title;

                    SqliteDataAccess.UpdateBookmark(id, _link, _linkTitle);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Could not get title from link.\n" + ex.Message);
                }
                
            }
            LinkTitle.Text = _linkTitle;
        }

        public int GetBookmarkID() {
            return _bookmarkID;
        }


        private void Link_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo { FileName = e.Uri.AbsoluteUri, UseShellExecute = true });
            e.Handled = true;
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            EditBookmark editBookmark = new EditBookmark();
            editBookmark.SetBookmarkID(_bookmarkID);
            editBookmark.Show();
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Process.Start(new ProcessStartInfo { FileName = _link, UseShellExecute = true });
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (((Test.MainWindow)Application.Current.MainWindow).CheckedIDsLength() == 0) {
                ((Test.MainWindow)Application.Current.MainWindow).ActivateSelectedToFolderButton();
            }
            ((Test.MainWindow)Application.Current.MainWindow).AddCheckedID(_bookmarkID);
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            ((Test.MainWindow)Application.Current.MainWindow).RemoveCheckedID(_bookmarkID);
            if (((Test.MainWindow)Application.Current.MainWindow).CheckedIDsLength() == 0)
            {
                ((Test.MainWindow)Application.Current.MainWindow).DeactivateSelectedToFolderButton();
            }
        }
    }
}
