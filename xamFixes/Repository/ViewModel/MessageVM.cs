﻿using System;
using System.Collections.Generic;
using System.Text;

namespace xamFixes.Repository.ViewModel
{
    public class MessageVM
    {
        public int MessageId { get; set; }

        public Xamarin.Forms.LayoutOptions Position { get; set; }

        public string Body { get; set; }

        public bool Sent { get; set; }

        public bool IsBlob { get; set; }

        public string Source { get; set; }

        public int UserId { get; set; }

        public string CreatedAt { get; set; }
    }
}
