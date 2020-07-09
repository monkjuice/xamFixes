using System;
using System.Collections.Generic;
using System.Text;

namespace xamFixes.Repository.Utils
{
    public class ResetPool
    {
        public static void ResetDBPool()
        {
            SQLite.SQLiteAsyncConnection.ResetPool();
        }
    }
}
