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
    }
}
