using System;
using System.Collections.Generic;
using System.Text;

namespace xamFixes.Models
{
    public class SecureMessage
    {
        public Guid MessageId { get; set; }

        public byte[] EncryptedBody { get; set; }

        public string Source { get; set; }

        public int UserId { get; set; }

        public string CreatedAt { get; set; }

        public string PartialPublicKey { get; set; }
    }
}
