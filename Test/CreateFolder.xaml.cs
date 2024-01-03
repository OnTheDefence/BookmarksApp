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
        public CreateFolder()
        {
            InitializeComponent();
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

                SqliteDataAccess.SaveFolder(name);

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
