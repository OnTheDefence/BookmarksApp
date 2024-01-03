using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using Test;

namespace Bookmarks
{
    /// <summary>
    /// Interaction logic for EditBookmark.xaml
    /// </summary>
    public partial class EditBookmark : Window
    {

        public int _bookmarkID;
        public EditBookmark()
        {
            InitializeComponent();
        }

        public void SetBookmarkID(int id)
        {
            _bookmarkID = id;

            LinkText.Text = SqliteDataAccess.LoadBookmarkLinkFromID(_bookmarkID).Result;
            TitleText.Text = SqliteDataAccess.LoadBookmarkTitleFromID(_bookmarkID).Result;
        }

        public int GetBookmarkID()
        {
            return _bookmarkID;
        }

        private void Confirm_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            if (LinkText.Text.Length > 0 && TitleText.Text.Length > 0)
            {
                string title = TitleText.Text;
                string link = LinkText.Text;

                SqliteDataAccess.UpdateBookmark(_bookmarkID, link, title);

                ((MainWindow)Application.Current.MainWindow).LoadAllBookmarksOnPage();

                Close();
            }
            else
            {
                MessageBox.Show("Please enter a link and title.");
            }
        }

        private void Delete_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            SqliteDataAccess.DeleteBookmark(_bookmarkID);
            ((MainWindow)Application.Current.MainWindow).LoadAllBookmarksOnPage();
            Close();
        }

        private void Cancel_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Close();
        }

        private void GetTitle_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            if (LinkText.Text.Length > 0)
            {
                WebClient x = new WebClient();
                string source = x.DownloadString(LinkText.Text);
                string title = Regex.Match(source, @"\<title\b[^>]*\>\s*(?<Title>[\s\S]*?)\</title\>", RegexOptions.IgnoreCase).Groups["Title"].Value;
                TitleText.Text = title;
            }
            else
            {
                MessageBox.Show("Please enter a link.");
            }
        }
    }
}
