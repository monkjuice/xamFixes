using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace xamFixes.Models
{
    public class User
    {
        [PrimaryKey, AutoIncrement]
        public int UserId { get; set; }

        public string Username { get; set; }

        public string ProfilePicturePath { get; set; }

    }
}
