using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using Xamarin.Forms;
using SLB.iTrackr.Utils;
using SLB.iTrackr.Droid;
using SQLite;

[assembly: Dependency(typeof(SQLiteService))]
namespace SLB.iTrackr.Droid
{
    public class SQLiteService : ISQLiteService
    {
        public SQLiteConnection GetConnection()
        {
            var sqliteFileName = "SLB.iTrackr.db";
            string documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            var path = System.IO.Path.Combine(documentsPath, sqliteFileName);
            //Create Connection
            var conn = new SQLiteConnection(path);
            return conn;            
        }
    }
}