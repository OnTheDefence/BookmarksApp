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
    public partial class EditFolder : Window
    {

        public int _folderID;
        public EditFolder()
        {
            InitializeComponent();
        }

        public void SetBookmarkID(int id)
        {
            _folderID = id;

            FolderNameText.Text = SqliteDataAccess.LoadFolderNameFromID(_folderID).Result;
        }

        public int GetBookmarkID()
        {
            return _folderID;
        }

        private void Confirm_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            if (FolderNameText.Text.Length > 0)
            {
                string folderName = FolderNameText.Text;

                SqliteDataAccess.UpdateFolder(_folderID, folderName);

                ((MainWindow)Application.Current.MainWindow).LoadAllParentFolders();

                Close();
            }
            else
            {
                MessageBox.Show("Please enter a folder name.");
            }
        }

        private void Delete_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            //SqliteDataAccess.DeleteFolder(_folderID);
            ((MainWindow)Application.Current.MainWindow).LoadAllParentFolders();
            Close();
        }

        private void Cancel_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Close();
        }
    }
}
