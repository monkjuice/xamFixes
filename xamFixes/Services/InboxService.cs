using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using xamFixes.Interfaces;
using xamFixes.Models;
using xamFixes.Repository.ORM;
using xamFixes.Repository.ViewModel;
using xamFixes.Services.Utils;

namespace xamFixes.Services
{
    public class InboxService : IInboxService
    {
        private readonly IProfileService _profileService;

        public InboxService()
        {
            _profileService = new ProfileService();
        }

        async public Task<List<ConversationVM>> GetLastConversations(int userId)
        {
            var db = new InboxRepo();

            var conversations = await db.GetLastConversations(userId);

            var conversationsPreview = new List<ConversationVM>();

            foreach (var c in conversations)
            {
                var conversationPreview = new ConversationVM();
                conversationPreview.ConversationId = c.ConversationId;
                conversationPreview.LastActivity = DateTimeUtils.RelativeTime(c.LastActivity);

                var usersInConv = await db.GetConversationParticipants(c.ConversationId, userId);

                var recipient = await _profileService.GetUserProfile(usersInConv.FirstOrDefault().UserId);
                conversationPreview.RecipientUsername = recipient.Username;
                conversationPreview.RecipientProfilePicturePath = recipient.ProfilePicturePath;

                var lastMsg = await db.GetConversationLastMessage(c.ConversationId);
                conversationPreview.MessageBody = lastMsg.Body;
                conversationPreview.Unread = lastMsg.Unread;
                conversationPreview.Icon = lastMsg.UserId == App.AuthenticatedUser.UserId ? "→" : "←";
                conversationPreview.FontStyle = lastMsg.Unread ? FontAttributes.Bold : FontAttributes.None;

                conversationsPreview.Add(conversationPreview);

            }

            return conversationsPreview;
        }

        async public Task<List<MessageVM>> GetConversationLastMessages(int conversationId)
        {
            var db = new InboxRepo();

            var _messages = await db.GetLastMessagesOfConversation(conversationId);

            var messages = new List<MessageVM>();

            foreach(var m in _messages)
            {
                messages.Add(new MessageVM()
                {
                    Body = m.Body,
                    MessageId = m.MessageId,
                    Position = m.UserId == App.AuthenticatedUser.UserId ? LayoutOptions.Start : LayoutOptions.End,
                    CreatedAt = m.CreatedAt.ToString("t", CultureInfo.CreateSpecificCulture("en-US")),
                    Sent = true
                });
            }

            return messages;

        }
    }
}
