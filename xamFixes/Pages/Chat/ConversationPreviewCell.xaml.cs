﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace xamFixes.Pages.Chat
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ConversationPreviewCell : ContentView
    {
        public ConversationPreviewCell()
        {
            InitializeComponent();
        }
    }
}