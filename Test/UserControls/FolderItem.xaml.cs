using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Bookmarks.UserControls
{
    /// <summary>
    /// Interaction logic for FolderItem.xaml
    /// </summary>
    public partial class FolderItem : UserControl
    {

        public int _folderID;

        private string _name;
        List<int> FolderIDs = new List<int>();

        public FolderItem()
        {
            InitializeComponent();
        }

        public void SetFolderID(int id)
        {
            _folderID = id;

            _name = SqliteDataAccess.LoadFolderNameFromID(_folderID).Result;
            FolderText.Text = _name;
        }

        public int GetFolderID()
        {
            return _folderID;
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            EditFolder editFolder = new EditFolder();
            editFolder.SetBookmarkID(_folderID);
            editFolder.Show();
            editFolder.LoadFolderDropdown();
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ((Test.MainWindow)Application.Current.MainWindow).LoadFolderBookmarksOnPage(_folderID);
        }

        private void DownButton_Click(object sender, RoutedEventArgs e)
        {
            if (DownImage.Source.ToString() == "pack://application:,,,/Bookmarks;component/Images/up_arrow.png")
            {
                DownImage.Source = new BitmapImage(new Uri(@"/Bookmarks;component/Images/down_arrow.png", UriKind.Relative));
            }
            else
            {
                DownImage.Source = new BitmapImage(new Uri(@"/Bookmarks;component/Images/up_arrow.png", UriKind.Relative));
            }

            if (FoldersStack.Children.Count == 0)
            {
                FolderIDs = SqliteDataAccess.LoadFoldersFromParentID(_folderID);
                for (int i = 0; i < FolderIDs.Count; i++)
                {
                    FolderItem _folder = new FolderItem();
                    _folder.SetFolderID(FolderIDs[i]);
                    FoldersStack.Children.Add(_folder);
                }
            }
            else
            {
                FoldersStack.Children.Clear();
            }
        }

        public void DisableArrow() {
            DownButton.Visibility = Visibility.Collapsed;
            DownButton.IsEnabled = false;
        }

        protected override void OnDrop(DragEventArgs e)
        {
            base.OnDrop(e);

            if (e.Data.GetDataPresent("Int")) {
                List<int> IDs = new List<int>();
                if (((Test.MainWindow)Application.Current.MainWindow).CheckedIDsLength() > 0)
                {
                    IDs.AddRange(((Test.MainWindow)Application.Current.MainWindow).GetCheckedIDs());
                    string s = "";
                    for (int i = 0; i < IDs.Count; i++)
                    {
                        SqliteDataAccess.AddBookmarkToFolder(IDs[i], _folderID);
                        s += SqliteDataAccess.LoadBookmarkTitleFromID(IDs[i]).Result + "\n";
                    }

                    MessageBox.Show("Added bookmarks with titles:\n" + s + "To folder: " + SqliteDataAccess.LoadFolderNameFromID(_folderID).Result);

                }
                else {
                    IDs.Add((int)e.Data.GetData("Int"));
                    SqliteDataAccess.AddBookmarkToFolder(IDs[0], _folderID);
                    MessageBox.Show("Added bookmarks with title:\n" + SqliteDataAccess.LoadBookmarkTitleFromID(IDs[0]).Result + "\nTo folder: " + SqliteDataAccess.LoadFolderNameFromID(_folderID).Result);
                }

                ((Test.MainWindow)Application.Current.MainWindow).LoadAllBookmarksOnPage();
            }
        }
    }
}
