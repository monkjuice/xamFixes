using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace xamFixes.Repository
{
    public static class Constants
    {
        public const string DatabaseFilename = "Fixes5.db3";

        public const SQLite.SQLiteOpenFlags Flags =
            // open the database in read/write mode
            SQLite.SQLiteOpenFlags.ReadWrite |
            // create the database if it doesn't exist
            SQLite.SQLiteOpenFlags.Create |
            // enable multi-threaded database access
            SQLite.SQLiteOpenFlags.SharedCache |
            // enable SQLite encryption 
            SQLite.SQLiteOpenFlags.ProtectionCompleteUnlessOpen
            ;

        public static string DatabasePath
        {
            get
            {
                var basePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                return Path.Combine(basePath, DatabaseFilename);
            }
        }
    }
}
