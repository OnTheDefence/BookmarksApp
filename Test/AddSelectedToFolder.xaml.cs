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
    public partial class AddSelectedToFolder : Window
    {
        List<string> folders = new List<string>();
        List<int> selectedBookmarks = new List<int>();

        public AddSelectedToFolder()
        {
            InitializeComponent();
        }

        public void LoadFolderDropdown()
        {
            folders.Add("---");
            List<string> allFolders = LoadAllFolders();
            allFolders.Sort();
            folders.AddRange(allFolders);
            FolderDropdown.ItemsSource = folders;
        }

        List<string> LoadAllFolders()
        {
            List<int> folderIDs = SqliteDataAccess.LoadFolderIDs();
            List<string> folders = new List<string>();

            for (int i = 0; i < folderIDs.Count; i++)
            {
                folders.Add(SqliteDataAccess.LoadFolderNameFromID(folderIDs[i]).Result);
            }

            return folders;
        }

        public void SetSelectedBookmarks(List<int> bookmarks){
            selectedBookmarks = bookmarks;
        }

        public void LoadBookmarkTitles() {
            BookmarksStack.Children.Clear();
            for (int i = 0; i < selectedBookmarks.Count; i++)
            {
                TextBlock bookmarkTitle = new TextBlock();
                bookmarkTitle.Text = "- " + SqliteDataAccess.LoadBookmarkTitleFromID(selectedBookmarks[i]).Result;
                bookmarkTitle.TextTrimming = TextTrimming.CharacterEllipsis;
                BookmarksStack.Children.Add(bookmarkTitle);
            }
        }

        private void ConfirmButtonBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            for (int i = 0; i < selectedBookmarks.Count; i++)
            {
                if (FolderDropdown.SelectedItem.ToString() == "---")
                {
                    SqliteDataAccess.AddBookmarkToFolder(selectedBookmarks[i], 0);
                }
                else {
                    SqliteDataAccess.AddBookmarkToFolder(selectedBookmarks[i], SqliteDataAccess.LoadFolderIDFromName(FolderDropdown.SelectedItem.ToString()).Result);
                }
            }

            if (FolderDropdown.SelectedItem.ToString() == "---")
            {
                ((MainWindow)Application.Current.MainWindow).LoadAllBookmarksOnPage();
            }
            else
            {
                ((MainWindow)Application.Current.MainWindow).LoadFolderBookmarksOnPage(SqliteDataAccess.LoadFolderIDFromName(FolderDropdown.SelectedItem.ToString()).Result);
            }

            Close();
        }

        private void CancelButtonBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Close();
        }
    }
}
