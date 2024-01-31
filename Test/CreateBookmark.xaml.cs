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
    public partial class CreateBookmark : Window
    {
        List<string> otherFolders = new List<string>();
        public CreateBookmark()
        {
            InitializeComponent();
        }

        public void SetDefaultFolder(int folder_id) 
        {
            if (folder_id == 0)
            {
                ParentFolderDropdown.SelectedIndex = 0;
            }
            else 
            {
                ParentFolderDropdown.SelectedItem = SqliteDataAccess.LoadFolderNameFromID(folder_id).Result;
            }
        }

        private void Cancel_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Close();
        }

        public void LoadFolderDropdown()
        {
            otherFolders.Add("---");
            List<int> allFolderIDs = SqliteDataAccess.LoadFolderIDs();
            List<string> allFolders = new List<string>();
            for (int i = 0; i < allFolderIDs.Count; i++) {
                allFolders.Add(SqliteDataAccess.LoadFolderNameFromID(allFolderIDs[i]).Result);
            }
            allFolders.Sort();
            otherFolders.AddRange(allFolders);
            ParentFolderDropdown.ItemsSource = otherFolders;
        }

        private void GetTitle_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            if (LinkText.Text.Length > 0)
            {
                try
                {
                    WebClient x = new WebClient();
                    string source = x.DownloadString(LinkText.Text);
                    string title = Regex.Match(source, @"\<title\b[^>]*\>\s*(?<Title>[\s\S]*?)\</title\>", RegexOptions.IgnoreCase).Groups["Title"].Value;
                    TitleText.Text = title;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Could not get title from link.\nPlease make sure the link contains 'https://'\n" + ex.Message);
                }
            }
            else {
                MessageBox.Show("Please enter a link.");
            }
        }

        private void Add_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            if (LinkText.Text.Length > 0 && TitleText.Text.Length > 0)
            {
                string title = TitleText.Text;
                string link = LinkText.Text;

                if (ParentFolderDropdown.SelectedItem.ToString() == "---")
                {
                    SqliteDataAccess.SaveBookmark(link, title, 0);
                }
                else {
                    SqliteDataAccess.SaveBookmark(link, title, SqliteDataAccess.LoadFolderIDFromName(ParentFolderDropdown.SelectedItem.ToString()).Result);
                }

                ((MainWindow)Application.Current.MainWindow).LoadAllBookmarksOnPage();

                Close();
            }
            else
            {
                MessageBox.Show("Please enter a link and title.");
            }
        }
    }
}
