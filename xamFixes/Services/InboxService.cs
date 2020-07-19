﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using xamFixes.Interfaces;
using xamFixes.Models;
using xamFixes.Repository.ORM;
using xamFixes.Services.Utils;
using xamFixes.DBModel;
using Xamarin.Essentials;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace xamFixes.Services
{
    public class InboxService : IInboxService
    {
        private readonly HttpClient client = new HttpClient();

        private readonly IProfileService _profileService;

        public InboxService()
        {
            _profileService = new ProfileService();
        }
        async public Task<ConversationVM> ResumeOrStartConversation(int userId, string username, string profilePicturePath)
        {
            var conversation = await FindConversation(userId);

            if (conversation == null)
                conversation = new ConversationVM() { ConversationId = Guid.Empty };

            conversation.UserId = userId;
            conversation.RecipientUsername = username;
            conversation.RecipientProfilePicturePath = profilePicturePath; 

            return conversation;
        }

        async public Task<ObservableCollection<ConversationVM>> GetLastConversations(int userId)
        {
            try
            {
            
                var db = new InboxRepo();

                var conversations = await db.GetLastConversations();

                var conversationsPreview = new ObservableCollection<ConversationVM>();

                foreach (var c in conversations)
                {
                    try
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
                    catch(Exception e)
                    {
                        //broken convo
                        continue;
                    }

                }

                return conversationsPreview;
            }
            catch(Exception e)
            {
                return null;
            }
        }

        async public Task<ObservableCollection<MessageVM>> GetConversationLastMessages(Guid conversationId)
        {
            var db = new InboxRepo();

            var _messages = await db.GetLastMessagesOfConversation(conversationId);

            var messages = new ObservableCollection<MessageVM>();

            foreach(var m in _messages)
            {
                messages.Add(new MessageVM()
                {
                    Body = m.Body,
                    MessageId = m.MessageId,
                    Position = m.UserId == App.AuthenticatedUser.UserId ? LayoutOptions.End : LayoutOptions.Start,
                    CreatedAt = m.CreatedAt.ToString("t", CultureInfo.CreateSpecificCulture("en-US")),
                    IsSent = true
                });
            }

            return messages;

        }

        public MessageVM CreateMessage(string msg, int userId, Guid messageId)
        {
            var msgVM = new MessageVM()
            {
                MessageId = messageId,
                Position = App.AuthenticatedUser.UserId == userId ? LayoutOptions.End : LayoutOptions.Start,
                Body = msg,
                IsSent = true,
                UserId = userId,
                CreatedAt = DateTime.Now.ToString("yyyy")
            };

            return msgVM;
        }

        async public Task<ConversationVM> CreateConversation(ConversationVM c)
        {
            var db = new InboxRepo();

            var conversationPreview = new ConversationVM();
            conversationPreview.ConversationId = c.ConversationId;
            conversationPreview.LastActivity = DateTimeUtils.RelativeTime(DateTime.Parse(c.LastActivity));

            var usersInConv = await db.GetConversationParticipants(c.ConversationId, App.AuthenticatedUser.UserId);

            var recipient = await _profileService.GetUserProfile(usersInConv.FirstOrDefault().UserId);
            conversationPreview.RecipientUsername = recipient.Username;
            conversationPreview.RecipientProfilePicturePath = recipient.ProfilePicturePath;

            var lastMsg = await db.GetConversationLastMessage(c.ConversationId);
            conversationPreview.MessageBody = lastMsg.Body;
            conversationPreview.Unread = lastMsg.Unread;
            conversationPreview.Icon = lastMsg.UserId == App.AuthenticatedUser.UserId ? "→" : "←";
            conversationPreview.FontStyle = lastMsg.Unread ? FontAttributes.Bold : FontAttributes.None;

            return conversationPreview;
        }

        async public Task<Message> StoreMessage(MessageVM msg, Guid conversationId, int recipientUserId)
        {
            var db = new InboxRepo();

            var message = new Message()
            {
                MessageId = msg.MessageId,
                UserId = msg.UserId,
                Body = msg.Body,
                ConversationId = conversationId != Guid.Empty ? conversationId : Guid.Empty,
                CreatedAt = DateTime.Now,
                Unread = true
            };

            if (conversationId == Guid.Empty)
            {
                var conversation = await StoreConversation(recipientUserId);

                message.ConversationId = conversation.ConversationId;
            }

            _ = db.SaveMessage(message);

            return message;
        }

        async public Task<ConversationVM> StoreConversation(int userId)
        {
            var db = new InboxRepo();

            var conversation = new Conversation()
            {
                ConversationId = Guid.NewGuid(),
                LastActivity = DateTime.Now
            };

            var convId = await db.SaveConversationAsync(conversation);

            var uic = new UserInConversation()
            {
                ConversationId = convId,
                UserId = App.AuthenticatedUser.UserId
            };

            var uic2 = new UserInConversation()
            {
                ConversationId = convId,
                UserId = userId
            };

            _ = db.SaveUserInConversationAsync(uic).ConfigureAwait(false);
            _ = db.SaveUserInConversationAsync(uic2).ConfigureAwait(false);

            var user = await _profileService.GetUserProfile(userId);

            return new ConversationVM()
            {
                ConversationId = conversation.ConversationId,
                UserId = userId,
                RecipientProfilePicturePath = user.ProfilePicturePath,
                RecipientUsername = user.Username,
                LastActivity = DateTime.Now.ToString("yyyy-mm-dd HH:mm")
            };
        }

        async public Task<ConversationVM> FindConversation(int userId)
        {
            var db = new InboxRepo();
            var conversation = await db.FindConversation(userId);

            if(conversation != null)
            {
                var user = await _profileService.GetUserProfile(userId);

                conversation.RecipientUsername = user.Username;
                conversation.UserId = user.UserId;
                conversation.RecipientProfilePicturePath = user.ProfilePicturePath;
            }

            return conversation;
        }

        async public Task<bool> QueueMessage(string action, string who, string msgBody)
        {
            try
            {
                string _token = await SecureStorage.GetAsync("fixes_token");

                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);

                var oBody = new
                {
                    Action = action,
                    Who = who,
                    Body = msgBody
                };

                string requestBody = JsonConvert.SerializeObject(oBody);

                var stringTask = client.PostAsync(Base.baseURL + "/api/inbox/queuemessage", new StringContent(requestBody, Encoding.UTF8, "application/json"));

                var msg = await stringTask;

                var jsonString = msg.Content.ReadAsStringAsync().Result.Replace("\\", "").Trim('"');

                var response = JsonConvert.DeserializeObject<Base.Response>(jsonString);

                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

    }
}
