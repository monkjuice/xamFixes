using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace xamFixes.Models
{
    public class UsersInConversation
    {
        [PrimaryKey, AutoIncrement]
        public int UsersInConversationId { get; set; }

        public int ConversationId { get; set; }

        public int UserId { get; set; }
    }
}
