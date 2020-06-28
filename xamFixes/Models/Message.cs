using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace xamFixes.Models
{
    public class Message
    {
        [PrimaryKey, AutoIncrement]
        public int MessageId { get; set; }

        public int ConversationId { get; set; }

        public string MessageBody { get; set; }

        public bool Unread { get; set; }

        public int UserId { get; set; }

    }
}
