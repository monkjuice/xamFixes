﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace xamFixes.Repository
{
    public class Constants
    {
        public string DatabaseFilename = string.Format("{0}.db3", App.AuthenticatedUser != null ? App.AuthenticatedUser.UserId : 0);

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

        public string DatabasePath
        {
            get
            {
                var basePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                return Path.Combine(basePath, DatabaseFilename);
            }
        }
    }
}
