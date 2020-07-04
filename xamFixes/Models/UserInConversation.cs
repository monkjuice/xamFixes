using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace xamFixes.Models
{
    public class UserInConversation
    {
        [PrimaryKey, AutoIncrement]
        public int UserInConversationId { get; set; }

        public int ConversationId { get; set; }

        public int UserId { get; set; }
    }
}
