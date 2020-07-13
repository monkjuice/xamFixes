using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace xamFixes.DBModel
{
    public class UserInConversation
    {
        [PrimaryKey, AutoIncrement]
        public int UserInConversationId { get; set; }

        public Guid ConversationId { get; set; }

        public int UserId { get; set; }
    }
}
