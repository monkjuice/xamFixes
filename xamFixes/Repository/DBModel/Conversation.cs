using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace xamFixes.DBModel
{
    public class Conversation
    {
        [PrimaryKey]
        public Guid ConversationId { get; set; }

        public DateTime LastActivity { get; set; }

    }
}
