using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xamFixes.Models;
using xamFixes.Repository.ViewModel;

namespace xamFixes.Repository.ORM
{
    public class InboxRepo : FixesDatabase
    {

        async public Task<int> SaveConversationAsync(Conversation conversation)
        {
            if (conversation.ConversationId != 0)
            {
                return await Database.UpdateAsync(conversation);
            }
            else
            {
                var t = await Database.InsertAsync(conversation);
                return conversation.ConversationId;
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

        async public Task<List<Conversation>> GetLastConversations(int userId)
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
        async public Task<List<UserInConversation>> GetConversationParticipants(int conversationId, int userId)
        {
            try
            {
                var usersInConv = await Database.QueryAsync<UserInConversation>($@"Select * from UserInConversation uc
                    where uc.ConversationId = {conversationId} and uc.UserId is not {userId};");

                return usersInConv;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        async public Task<Message> GetConversationLastMessage(int conversationId)
        {
            try
            {
                var msg = await Database.QueryAsync<Message>($@"Select * from Message m
                    where m.ConversationId = {conversationId} order by m.MessageId desc limit 1;");

                return msg.FirstOrDefault();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        async public Task<List<Message>> GetLastMessagesOfConversation(int conversationId)
        {
            try 
            { 
                var t = await Database.QueryAsync<Message>($@"Select * from Message
                                                                        where ConversationId = {conversationId}
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

    }
}
