using Bookmarks;
using Bookmarks.UserControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
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
        string sortBy = "abc";
        int selectedFolder = 0;
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
            GetBookmarkTitles(bookmarkIDs);
            bookmarkIDs = SortBookmarks(bookmarkIDs);
            LoadBookmarkDetails(bookmarkIDs);

            for (int i = 0; i < bookmarkIDs.Count; i++)
            {
                BookmarkItem _bookmark = new BookmarkItem();
                _bookmark.SetBookmarkID(bookmarkIDs[i]);
                bookmarksStack.Children.Add(_bookmark);
            }

            SelectedFolderText.Text = "All Unsorted Bookmarks";
            selectedFolder = 0;
        }

        public void GetBookmarkTitles(List<int> ids)
        {
            for (int i = 0; i < ids.Count; i++) {
                if (SqliteDataAccess.LoadBookmarkTitleFromID(ids[i]).Result == null)
                {
                    try
                    {
                        WebClient x = new WebClient();
                        string source = x.DownloadString(SqliteDataAccess.LoadBookmarkLinkFromID(ids[i]).Result);
                        string title = Regex.Match(source, @"\<title\b[^>]*\>\s*(?<Title>[\s\S]*?)\</title\>", RegexOptions.IgnoreCase).Groups["Title"].Value;
                        SqliteDataAccess.UpdateBookmark(ids[i], SqliteDataAccess.LoadBookmarkLinkFromID(ids[i]).Result, title);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Could not get title from link.\nPlease make sure the link contains 'https://'\n" + ex.Message);
                    }
                } 
            }
        }

        public void FlipSort() {
            if (sortBy == "abc")
            {
                sortBy = "date";
                Sort.Text = "Sort by: Date";
            }
            else
            {
                sortBy = "abc";
                Sort.Text = "Sort by: ABC";
            }

            LoadAllBookmarksOnPage();
        }

        public void LoadFolderBookmarksOnPage(int parent_id)
        {
            DeactivateSelectedToFolderButton();
            checkedItemIDs = new List<int>();
            bookmarksStack.Children.Clear();

            pageFolderIDs = SqliteDataAccess.LoadFoldersFromParentID(parent_id);
            bookmarkIDs = SqliteDataAccess.LoadBookmarksFromFolderID(parent_id);
            bookmarkIDs = SortBookmarks(bookmarkIDs);
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

            SelectedFolderText.Text = "Folder: " + SqliteDataAccess.LoadFolderNameFromID(parent_id).Result;
            selectedFolder = parent_id;
        }

        public void LoadAllParentFolders()
        {
            FolderStack.Children.Clear();

            folderIDs = SqliteDataAccess.LoadFolderIDs();
            for (int i = 0; i < folderIDs.Count; i++) {
                if (SqliteDataAccess.LoadFolderParentFromID(folderIDs[i]).Result == 0){
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

            IDs = SortBookmarks(IDs);

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

        List<int> SortBookmarks(List<int> all_ids) {
            List<string> titles = new List<string>();
            SortedDictionary<string, int> dates = new SortedDictionary<string, int>();
            List<int> sortedIDs = new List<int>();
            List<int> emptyIDs = new List<int>();

            if (sortBy == "abc")
            {
                for (int i = 0; i < all_ids.Count; i++)
                {
                    titles.Add(SqliteDataAccess.LoadBookmarkTitleFromID(all_ids[i]).Result);
                }

                titles.Sort();

                for (int i = 0; i < titles.Count; i++)
                {
                    sortedIDs.Add(SqliteDataAccess.LoadBookmarkIDFromTitle(titles[i]).Result);
                }
            }
            else if (sortBy == "date")
            {
                for (int i = 0; i < all_ids.Count; i++)
                {
                    string date = SqliteDataAccess.LoadBookmarkLastClickedFromID(all_ids[i]).Result;
                    if (date == "") {
                        emptyIDs.Add(all_ids[i]);
                    }
                    else
                    {
                        dates.Add(date, all_ids[i]);
                    }
                    
                }

                sortedIDs = emptyIDs;
                sortedIDs.AddRange(dates.Values.ToList());

                sortedIDs.Reverse();
            }

            return sortedIDs;
        
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

        public List<int> GetCheckedIDs()
        {
            return checkedItemIDs;
        }

        public void DeactivateSelectedToFolderButton()
        {
            SelectedToFolderButton.IsEnabled = false;
            SelectedToFolderButton.Visibility = Visibility.Collapsed;
            DeleteSelected.IsEnabled = false;
            DeleteSelected.Visibility = Visibility.Collapsed;
        }

        public void ActivateSelectedToFolderButton()
        {
            SelectedToFolderButton.IsEnabled = true;
            SelectedToFolderButton.Visibility = Visibility.Visible;
            DeleteSelected.IsEnabled = true;
            DeleteSelected.Visibility = Visibility.Visible;
        }

        private void BookmarkButtonBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            CreateBookmark createBookmark = new CreateBookmark();
            createBookmark.LoadFolderDropdown();
            createBookmark.SetDefaultFolder(selectedFolder);
            createBookmark.Show();
        }

        private void FolderButtonBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            CreateFolder createFolder = new CreateFolder();
            createFolder.LoadFolderDropdown();
            createFolder.Show();
        }

        private void HomeButtonBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            LoadAllBookmarksOnPage();
        }

        private void SelectedToFolderButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            AddSelectedToFolder addSelectedToFolder = new AddSelectedToFolder();
            addSelectedToFolder.LoadFolderDropdown();
            addSelectedToFolder.SetSelectedBookmarks(checkedItemIDs);
            addSelectedToFolder.LoadBookmarkTitles();
            addSelectedToFolder.Show();
        }

        private void DeleteSelected_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            string delete_string = "";
            for (int i = 0; i < checkedItemIDs.Count; i++)
            {
                delete_string += "\n- " + SqliteDataAccess.LoadBookmarkTitleFromID(checkedItemIDs[i]).Result;
            }
            var Result = MessageBox.Show("Are you sure you want to delete these bookmarks:" + delete_string + "?", "Delete?", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (Result == MessageBoxResult.Yes)
            {
                for (int i = 0; i < checkedItemIDs.Count; i++) {
                    SqliteDataAccess.DeleteBookmark(checkedItemIDs[i]);
                }
                MessageBox.Show("Deleted.");
                if (selectedFolder != 0)
                {
                    LoadFolderBookmarksOnPage(selectedFolder);
                }
                else 
                {
                    LoadAllBookmarksOnPage();
                }
            }
        }

        private void SortButtonBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            FlipSort();
            LoadAllBookmarksOnPage();
        }
    }


    static class CurrentMillis
    {
        private static readonly DateTime Jan1St1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        /// <summary>Get extra long current timestamp</summary>
        public static long Millis { get { return (long)((DateTime.UtcNow - Jan1St1970).TotalMilliseconds); } }
    }
}
