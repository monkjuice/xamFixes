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

        public Task<int> SaveConversationAsync(Conversation conversation)
        {
            if (conversation.ConversationId != 0)
            {
                return Database.UpdateAsync(conversation);
            }
            else
            {
                return Database.InsertAsync(conversation);
            }
        }

        public Task<int> SaveUserInConversationAsync(UserInConversation uic)
        {
            return Database.InsertAsync(uic);
        }

        public Task<int> SaveMessage(Message msg)
        {
            return Database.InsertAsync(msg);
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

    }
}
