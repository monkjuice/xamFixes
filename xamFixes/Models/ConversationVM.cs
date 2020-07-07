using System;
using System.Collections.Generic;
using System.Text;

namespace xamFixes.Models
{
    public class ConversationVM
    {
        public int MessageId { get; set; }

        public int ConversationId { get; set; }

        public string MessageBody { get; set; }

        public bool Unread { get; set; }

        public string Icon { get; set; }

        public Xamarin.Forms.FontAttributes FontStyle { get; set; }

        public int UserId { get; set; }

        public string Username { get; set; }

        public string ProfilePicturePath { get; set; }

        public string RecipientUsername { get; set; }

        public string RecipientProfilePicturePath { get; set; }

        public string LastActivity { get; set; }
    }
}
