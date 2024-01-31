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
        int _parentFolderID;
        List<string> otherFolders = new List<string>();
        public EditFolder()
        {
            InitializeComponent();
        }

        public void LoadFolderDropdown() {
            otherFolders.Add("---");
            List<string> allFolders = LoadAllOtherFolders();
            allFolders.Sort();
            otherFolders.AddRange(allFolders);
            ParentFolderDropdown.ItemsSource = otherFolders;

            _parentFolderID = SqliteDataAccess.LoadFolderParentFromID(_folderID).Result;
            

            if (_parentFolderID != 0)
            {
                ParentFolderDropdown.SelectedItem = SqliteDataAccess.LoadFolderNameFromID(_parentFolderID);
            }
        }

        List<string> LoadAllOtherFolders() {
            List<int> folderIDs = SqliteDataAccess.LoadFolderIDs();
            List<string> folders = new List<string>();
            List<int> childFolderIDs = new List<int>();
            folderIDs.Remove(_folderID);

            for (int i = 0; i < folderIDs.Count; i++){
                int firstParent = SqliteDataAccess.LoadFolderParentFromID(folderIDs[i]).Result;
                int currentParent = firstParent;
                int parent = currentParent;

                while (currentParent != 0) {
                    if (parent == _folderID) {
                        childFolderIDs.Add(folderIDs[i]);
                    }
                    parent = SqliteDataAccess.LoadFolderParentFromID(currentParent).Result;
                    currentParent = parent;
                }
            }

            for (int i = 0; i < childFolderIDs.Count; i++){
                folderIDs.Remove(childFolderIDs[i]);
            }

            for (int i = 0; i < folderIDs.Count; i++) {
                folders.Add(SqliteDataAccess.LoadFolderNameFromID(folderIDs[i]).Result);
            }

            return folders;
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

                if (ParentFolderDropdown.SelectedItem.ToString() == "---")
                {
                    SqliteDataAccess.UpdateFolder(_folderID, folderName, 0);
                }
                else {
                    int parentFolderID = SqliteDataAccess.LoadFolderIDFromName(ParentFolderDropdown.SelectedItem.ToString()).Result;
                    SqliteDataAccess.UpdateFolder(_folderID, folderName, parentFolderID);
                }

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
            List<int> folderIDs = SqliteDataAccess.LoadFolderIDs();
            List<int> childFolderIDs = new List<int>();
            List<int> foldersToDelete = new List<int>();
            folderIDs.Remove(_folderID);
            foldersToDelete.Add(_folderID);

            for (int i = 0; i < folderIDs.Count; i++)
            {
                if (SqliteDataAccess.LoadFolderParentFromID(folderIDs[i]).Result == _folderID) {
                    childFolderIDs.Add(folderIDs[i]);
                }
            }


            while (childFolderIDs.Count > 0) 
            {
                for (int i = 0; i < childFolderIDs.Count; i++) 
                {
                    for (int j = 0; j < folderIDs.Count; j++)
                    {
                        if (SqliteDataAccess.LoadFolderParentFromID(folderIDs[j]).Result == childFolderIDs[i])
                        {
                            childFolderIDs.Add(folderIDs[j]);
                        }
                    }
                    foldersToDelete.Add(childFolderIDs[i]);
                    childFolderIDs.Remove(childFolderIDs[i]);
                }
            }

            for (int i = 0; i < foldersToDelete.Count; i++)
            {
                SqliteDataAccess.DeleteFolder(foldersToDelete[i]);
            }


            ((MainWindow)Application.Current.MainWindow).LoadAllParentFolders();
            Close();
        }

        private void Cancel_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Close();
        }
    }
}
