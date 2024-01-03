using Bookmarks;
using Bookmarks.UserControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Test.UserControls;

namespace Test
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        List<int> bookmarkIDs = new List<int>();
        List<string> bookmarkTitles = new List<string>();
        List<string> bookmarkLinks = new List<string>();
        List<int> parentFolderIDs = new List<int>();
        List<int> folderIDs = new List<int>();
        List<int> pageFolderIDs = new List<int>();
        List<string> parentFolderNames = new List<string>();
        List<int> checkedItemIDs = new List<int>();
        public MainWindow()
        {
            InitializeComponent();

            LoadAllBookmarksOnPage();
            LoadAllParentFolders();
        }

        public void LoadAllBookmarksOnPage()
        {
            DeactivateSelectedToFolderButton();
            checkedItemIDs = new List<int>();
            bookmarksStack.Children.Clear();

            bookmarkIDs = SqliteDataAccess.LoadBookmarkIDs();
            LoadBookmarkDetails(bookmarkIDs);

            for (int i = 0; i < bookmarkIDs.Count; i++)
            {
                BookmarkItem _bookmark = new BookmarkItem();
                _bookmark.SetBookmarkID(bookmarkIDs[i]);
                bookmarksStack.Children.Add(_bookmark);
            }
        }

        public void LoadFolderBookmarksOnPage(int parent_id)
        {
            DeactivateSelectedToFolderButton();
            checkedItemIDs = new List<int>();
            bookmarksStack.Children.Clear();

            pageFolderIDs = SqliteDataAccess.LoadFoldersFromParentID(parent_id);
            bookmarkIDs = SqliteDataAccess.LoadBookmarksFromFolderID(parent_id);
            LoadBookmarkDetails(bookmarkIDs);

            for (int i = 0; i < pageFolderIDs.Count; i++) {
                FolderItem _folder = new FolderItem();
                _folder.SetFolderID(pageFolderIDs[i]);
                _folder.DisableArrow();
                bookmarksStack.Children.Add(_folder);
            }

            for (int i = 0; i < bookmarkIDs.Count; i++)
            {
                BookmarkItem _bookmark = new BookmarkItem();
                _bookmark.SetBookmarkID(bookmarkIDs[i]);
                bookmarksStack.Children.Add(_bookmark);
            }
        }

        public void LoadAllParentFolders()
        {
            FolderStack.Children.Clear();

            folderIDs = SqliteDataAccess.LoadFolderIDs();
            for (int i = 0; i < folderIDs.Count; i++) {
                if (SqliteDataAccess.LoadFolderParentFromID(folderIDs[i]).Result == -1){
                    parentFolderIDs.Add(folderIDs[i]);
                    parentFolderNames.Add(SqliteDataAccess.LoadFolderNameFromID(folderIDs[i]).Result);
                    FolderItem _folder = new FolderItem();
                    _folder.SetFolderID(folderIDs[i]);
                    FolderStack.Children.Add(_folder);
                }
            }
        }

        public void LoadBookmarksByID(List<int> IDs) {
            DeactivateSelectedToFolderButton();
            bookmarksStack.Children.Clear();

            for (int i = 0; i < IDs.Count; i++)
            {
                BookmarkItem _bookmark = new BookmarkItem();
                _bookmark.SetBookmarkID(IDs[i]);
                bookmarksStack.Children.Add(_bookmark);
            }
        }

        public void LoadBookmarkDetails(List<int> ids) {

            bookmarkLinks = new List<string>();
            bookmarkTitles = new List<string>();

            for (int i = 0; i < ids.Count; i++) {
                bookmarkLinks.Add(SqliteDataAccess.LoadBookmarkLinkFromID(ids[i]).Result);
                bookmarkTitles.Add(SqliteDataAccess.LoadBookmarkTitleFromID(ids[i]).Result);
            }
        }

        public void AddBookmarkIDToList(int ID) {
            bookmarkIDs.Add(ID);
        }

        public List<int> GetBookmarkIDList() {
            return bookmarkIDs;
        }

        private void Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            List<int> IDs = new List<int>();
            List<int> currentIDs = SqliteDataAccess.LoadBookmarkIDs();

            LoadBookmarkDetails(currentIDs);

            for (int i = 0; i < currentIDs.Count; i++) {
                if (bookmarkTitles[i].ToLower().Contains(Search.Text.ToLower()) | bookmarkLinks[i].ToLower().Contains(Search.Text.ToLower())){
                    IDs.Add(currentIDs[i]);
                }
            }

            LoadBookmarksByID(IDs);
        }

        public void AddCheckedID(int id) {
            checkedItemIDs.Add(id);
        }

        public void RemoveCheckedID(int id) {
            checkedItemIDs.Remove(id);
        }

        public int CheckedIDsLength() {
            return checkedItemIDs.Count();
        }

        public void DeactivateSelectedToFolderButton()
        {
            SelectedToFolderButton.IsEnabled = false;
            SelectedToFolderButton.Visibility = Visibility.Collapsed;
        }

        public void ActivateSelectedToFolderButton()
        {
            SelectedToFolderButton.IsEnabled = true;
            SelectedToFolderButton.Visibility = Visibility.Visible;
        }

        private void BookmarkButtonBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            CreateBookmark createBookmark = new CreateBookmark();
            createBookmark.Show();
        }

        private void FolderButtonBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            CreateFolder createFolder = new CreateFolder();
            createFolder.Show();
        }

        private void HomeButtonBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            LoadAllBookmarksOnPage();
        }

        private void SelectedToFolderButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
