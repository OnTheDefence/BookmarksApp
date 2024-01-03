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
        public CreateBookmark()
        {
            InitializeComponent();
        }

        private void Cancel_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Close();
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
                    MessageBox.Show("Could not get title from link.\n" + ex.Message);
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

                SqliteDataAccess.SaveBookmark(link, title);

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
