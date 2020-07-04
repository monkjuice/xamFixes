using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using xamFixes.Models;
using xamFixes.Repository.ViewModel;

namespace xamFixes.Interfaces
{
    public interface IInboxService
    {
        Task<List<ConversationVM>> GetLastConversations(int userId);

        Task<List<MessageVM>> GetConversationLastMessages(int userId);

    }
}
