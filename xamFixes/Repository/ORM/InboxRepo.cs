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
                var t = await Database.InsertAsync(conversation);

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
                var conversations = await Database.QueryAsync<Conversation>("Select * from Conversation order by LastActivity desc;");

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
                    where m.ConversationId = '{conversationId}' order by m.CreatedAt desc limit 1;");

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

        async public Task<Conversation> GetConversation(Guid conversationId)
        {
            return await Database.Table<Conversation>().Where(x => x.ConversationId == conversationId).FirstOrDefaultAsync();
        }

        async public Task<Message> GetMessage(Guid messageId)
        {
            return await Database.Table<Message>().Where(x => x.MessageId == messageId).FirstOrDefaultAsync();
        }

        async public Task<int> UpdateConversation(Conversation conversation)
        {
            try
            {
                conversation.LastActivity = DateTime.Now;
                return await Database.UpdateAsync(conversation);
            }
            catch (Exception e)
            {
                return -1;
            }
        }

        async public Task<int> UpdateMessage(Message message)
        {
            try 
            { 
                return await Database.UpdateAsync(message);
            }
            catch(Exception e)
            {
                return -1;
            }
        }

        async public Task<List<Message>> GetUnsentMessagesTo(int userId)
        {
            var t = await Database.QueryAsync<Message>($@"select * from message m
                                                            join UserInConversation uc on uc.ConversationId = m.ConversationId
                                                            where uc.UserId = {userId}
                                                            and m.IsSent = false");

            return t;
        }

    }
}
