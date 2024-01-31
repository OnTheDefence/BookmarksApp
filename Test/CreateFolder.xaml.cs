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
    public partial class CreateFolder : Window
    {
        List<string> otherFolders = new List<string>();

        public CreateFolder()
        {
            InitializeComponent();
        }

        public void LoadFolderDropdown()
        {
            otherFolders.Add("---");
            List<string> allFolders = LoadAllOtherFolders();
            allFolders.Sort();
            otherFolders.AddRange(allFolders);
            ParentFolderDropdown.ItemsSource = otherFolders;
        }

        List<string> LoadAllOtherFolders()
        {
            List<int> folderIDs = SqliteDataAccess.LoadFolderIDs();
            List<string> folders = new List<string>();
            List<int> childFolderIDs = new List<int>();

            for (int i = 0; i < folderIDs.Count; i++)
            {
                folders.Add(SqliteDataAccess.LoadFolderNameFromID(folderIDs[i]).Result);
            }

            return folders;
        }

        private void Cancel_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Close();
        }

        private void Add_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            if (FolderTitleText.Text.Length > 0)
            {
                string name = FolderTitleText.Text;
                int parent = 0;

                if (ParentFolderDropdown.SelectedItem.ToString() != "---") {
                    parent = SqliteDataAccess.LoadFolderIDFromName(ParentFolderDropdown.SelectedItem.ToString()).Result;
                }

                SqliteDataAccess.SaveFolder(name, parent);

                ((MainWindow)Application.Current.MainWindow).LoadAllParentFolders();

                Close();
            }
            else
            {
                MessageBox.Show("Please enter a folder name.");
            }
        }
    }
}
