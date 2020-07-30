using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace xamFixes.DBModel
{
    public class Message
    {
        [PrimaryKey]
        public Guid MessageId { get; set; }

        public Guid ConversationId { get; set; }

        public string Body { get; set; }

        public int UserId { get; set; }

        public DateTime CreatedAt { get; set; }

        public bool IsRead { get; set; }

        public bool IsRecieved { get; set; }

        public bool IsSent { get; set; }

    }
}
