using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xamFixes.Models;
using xamFixes.DBModel;
using System.Diagnostics;

namespace xamFixes.Repository.ORM
{
    public class InboxRepo : FixesDatabase
    {

        async public Task<Guid> SaveConversationAsync(Conversation conversation)
        {

            try
            { 
                if (await GetConversation(conversation.ConversationId) != null)
                {
                    _ = await Database.UpdateAsync(conversation);
                }
                else
                {
                    var t = await Database.InsertAsync(conversation);
                }

                return conversation.ConversationId;
            }
            catch(Exception e)
            {
                return Guid.Empty;
            }

        }

        public Task<int> SaveUserInConversationAsync(UserInConversation uic)
        {
            return Database.InsertAsync(uic);
        }

        public Task<int> SaveMessage(Message msg)
        {
            try 
            { 
                return Database.InsertAsync(msg);
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        async public Task<List<Conversation>> GetLastConversations()
        {
            try
            {
                var conversations = await Database.QueryAsync<Conversation>("Select * from Conversation;");

                return conversations;
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Returns conversations in a conversation except for logged in user
        /// </summary>
        /// <param name="conversationId"></param>
        /// <param name="userId">Logged in user id</param>
        /// <returns>List of UserInConversation</returns>
        async public Task<List<UserInConversation>> GetConversationParticipants(Guid conversationId, int userId)
        {
            try
            {

                Database.Tracer = new Action<string>(q => Debug.WriteLine(q));
                Database.Trace = true;

                var usersInConv = await Database.QueryAsync<UserInConversation>($@"Select * from UserInConversation uc
                    where uc.ConversationId = '" + conversationId.ToString() + $"' and uc.UserId is not {userId};");

                return usersInConv;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        async public Task<Message> GetConversationLastMessage(Guid conversationId)
        {
            try
            {
                var msg = await Database.QueryAsync<Message>($@"Select * from Message m
                    where m.ConversationId = '{conversationId}' order by m.MessageId desc limit 1;");

                return msg.FirstOrDefault();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        async public Task<List<Message>> GetLastMessagesOfConversation(Guid conversationId)
        {
            try 
            {
                var t = await Database.QueryAsync<Message>($@"Select * from Message
                                                                        where ConversationId = '{conversationId}'
                                                                        order by MessageId asc limit 30;");
                return t;
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        async Task<ConversationVM> GetConversation(Guid conversationId)
        {
            var t = await Database.QueryAsync<ConversationVM>("Select * from Conversation where ConversationId = '" + conversationId.ToString() + "';");

            return t.FirstOrDefault();
        }

        async public Task<ConversationVM> FindConversation(int userId)
        {
            var t = await Database.QueryAsync<ConversationVM>($@"Select c.ConversationId, c.LastActivity 
                                                                        from UserInConversation uc
                                                                        join Conversation c on c.ConversationId = uc.ConversationId
                                                                        where uc.UserId = {userId} limit 1;");
            if (t.Count() > 0)
                return t.First();

            return null;
        }

    }
}
