using System;
using System.Collections.Generic;
using System.Text;

namespace xamFixes.Models
{
    public class MessageVM
    {
        public Guid MessageId { get; set; }

        // Conversation view position (L,R)
        public Xamarin.Forms.LayoutOptions Position { get; set; }

        public string Body { get; set; }

        public byte[] EncryptedBody { get; set; }

        public string Source { get; set; }

        public int UserId { get; set; }

        public string CreatedAt { get; set; }

        public string PartialPublicKey { get; set; }

        public bool IsSent { get; set; }

        public bool IsRead { get; set; }

        public bool IsBlob { get; set; }
    }
}
