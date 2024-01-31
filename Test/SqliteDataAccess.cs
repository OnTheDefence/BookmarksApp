using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Bookmarks
{
    public class SqliteDataAccess
    {
        public static List<int> LoadBookmarkIDs()
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString())) {
                var output = cnn.Query<int>("select ID from Bookmarks", new DynamicParameters());
                return output.ToList();
            }
        }

        public async static Task<string> LoadBookmarkLinkFromID(int ID)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                return await cnn.QueryFirstAsync<string>(@"select Link from Bookmarks where ID = @_ID", new { @_ID = ID });
            }
        }

        public async static Task<string> LoadBookmarkTitleFromID(int ID)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                return await cnn.QueryFirstAsync<string>(@"select Title from Bookmarks where ID = @_ID", new { @_ID = ID });
            }
        }

        public async static Task<string> LoadBookmarkLastClickedFromID(int ID)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                return await cnn.QueryFirstAsync<string>(@"select LastClicked from Bookmarks where ID = @_ID", new { @_ID = ID });
            }
        }

        public async static Task<int> LoadBookmarkIDFromLastClicked(string date)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                return await cnn.QueryFirstAsync<int>(@"select ID from Bookmarks where LastClicked = @_date", new { @_date = date });
            }
        }

        public async static Task<int> LoadBookmarkIDFromLink(string link)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                return await cnn.QueryFirstAsync<int>(@"select ID from Bookmarks where link = @_link", new { @_link = link });
            }
        }

        public async static Task<int> LoadBookmarkIDFromTitle(string title)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                return await cnn.QueryFirstAsync<int>(@"select ID from Bookmarks where Title = @_title", new { @_title = title });
            }
        }

        public async static Task<int> LoadFolderIDFromBookmarkID(int ID)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                return await cnn.QueryFirstAsync<int>(@"select Folder from Bookmarks where ID = @_ID", new { @_ID = ID });
            }
        }

        public async static void SaveBookmark(string link, string title, int folder_id)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                WebClient x = new WebClient();
                await cnn.ExecuteAsync("insert into Bookmarks (Link, Title, Folder) values (@link, @title, @folderID)", new { @link = link, @title = title, @folderID = folder_id });
            }
        }

        public static void UpdateBookmark(int ID, string link, string title)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute("update Bookmarks set link = @link where ID = @_ID", new { @link = link, @_ID = ID });
                cnn.Execute("update Bookmarks set title = @title where ID = @_ID", new { @title = title, @_ID = ID });
            }
        }

        public static void BookmarkClick(int ID, string date)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute("update Bookmarks set LastClicked = @_date where ID = @_ID", new { @_date = date, @_ID = ID });
            }
        }

        public static void AddBookmarkToFolder(int ID, int folderID)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute("update Bookmarks set Folder = @_folderID where ID = @_ID", new { @_folderID = folderID, @_ID = ID });
            }
        }

        public static void DeleteBookmark(int ID) {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute("delete from Bookmarks where ID = @_ID", new { _ID = ID });
            }
        }

        public static List<int> LoadFolderIDs()
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<int>("select ID from Folders", new DynamicParameters());
                return output.ToList();
            }
        }

        public static List<int> LoadFoldersFromParentID(int parent_id)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<int>("select ID from Folders where ParentID = @parent_id", new { @parent_id = parent_id });
                return output.ToList();
            }
        }

        public static List<int> LoadBookmarksFromFolderID(int folder_id)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<int>("select ID from Bookmarks where Folder = @folder_id", new { @folder_id = folder_id });
                return output.ToList();
            }
        }

        public async static Task<string> LoadFolderNameFromID(int ID)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                return await cnn.QueryFirstAsync<string>(@"select Name from Folders where ID = @_ID", new { @_ID = ID });
            }
        }

        public async static Task<int> LoadFolderIDFromName(string name)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                return await cnn.QueryFirstAsync<int>(@"select ID from Folders where Name = @_name", new { @_name = name });
            }
        }

        public async static Task<int> LoadFolderParentFromID(int ID)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.QueryFirstAsync<int>(@"select ParentID from Folders where ID = @_ID", new { @_ID = ID });
                return await output;
            }
        }

        public static void UpdateFolder(int ID, string name, int parentID)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute("update Folders set Name = @name where ID = @_ID", new { @name = name, @_ID = ID });
                cnn.Execute("update Folders set ParentID = @parentID where ID = @_ID", new { @parentID = parentID, @_ID = ID });
            }
        }

        public static void DeleteFolder(int ID)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute("delete from Folders where ID = @_ID", new { _ID = ID });
            }
        }

        public async static void SaveFolder(string name, int parentID)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                WebClient x = new WebClient();
                await cnn.ExecuteAsync("insert into Folders (Name, ParentID) values (@name, @parent_id)", new { @name = name, @parent_id = parentID });
            }
        }

        private static string LoadConnectionString(string id = "Default")
        {
            return ConfigurationManager.ConnectionStrings[id].ConnectionString;
        }
    }
}
