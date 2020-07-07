using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using xamFixes.Models;
using xamFixes.DBModel;

namespace xamFixes.Interfaces
{
    public interface IInboxService
    {
        Task<ObservableCollection<ConversationVM>> GetLastConversations(int userId);
        Task<ObservableCollection<MessageVM>> GetConversationLastMessages(int userId);
        MessageVM CreateMessage(string msg, int userId);
        Task<Message> StoreMessage(MessageVM msg, int conversationId, int recipientUserId);
        Task<ConversationVM> FindConversation(int userId);
        Task<ConversationVM> CreateConversation(ConversationVM convo);
    }
}
