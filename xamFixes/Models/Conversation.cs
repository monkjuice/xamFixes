using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace xamFixes.Models
{
    public class Conversation
    {
        [PrimaryKey, AutoIncrement]
        public int ConversationId { get; set; }

        public DateTime LastActivity { get; set; }

    }
}
