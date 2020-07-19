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
        Task<ObservableCollection<MessageVM>> GetConversationLastMessages(Guid conversationId);
        MessageVM CreateMessage(string msg, int userId, Guid messageId);
        Task<Message> StoreMessage(MessageVM msg, Guid conversationId, int recipientUserId);
        Task<ConversationVM> FindConversation(int userId);
        Task<ConversationVM> CreateConversation(ConversationVM convo);
        Task<ConversationVM> ResumeOrStartConversation(int userId, string username, string profilePicture);
        Task<ConversationVM> StoreConversation(int userid);
        Task<bool> QueueMessage(string action, string who, string body);
    }
}
