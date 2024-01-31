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
                    MessageBox.Show("Could not get title from link.\nPlease make sure the link contains 'https://'\n" + ex.Message);
                }
                
            }
            LinkTitle.Text = _linkTitle;
            int folderID = SqliteDataAccess.LoadFolderIDFromBookmarkID(_bookmarkID).Result;
            if (folderID == 0) 
            {
                FolderText.Text = "---";
            }
            else
            {
                FolderText.Text = SqliteDataAccess.LoadFolderNameFromID(folderID).Result;
            }
        }

        public int GetBookmarkID() {
            return _bookmarkID;
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            EditBookmark editBookmark = new EditBookmark();
            editBookmark.SetBookmarkID(_bookmarkID);
            editBookmark.Show();
        }

        private void Border_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            SqliteDataAccess.BookmarkClick(_bookmarkID, CurrentMillis.Millis.ToString());
            ((Test.MainWindow)Application.Current.MainWindow).LoadAllBookmarksOnPage();
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

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DataObject data = new DataObject();
                data.SetData("Int", _bookmarkID);

                DragDrop.DoDragDrop(this, data, DragDropEffects.Copy);
            }
        }
    }
}
