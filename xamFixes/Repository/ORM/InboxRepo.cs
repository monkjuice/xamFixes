using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using xamFixes.Models;

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

        public Task<int> SaveUsersInConversationAsync(UsersInConversation uic)
        {
            return Database.InsertAsync(uic);
        }

        public Task<int> SaveMessage(Message msg)
        {
            return Database.InsertAsync(msg);
        }


        public Task<List<Message>> GetLastConversations(int userId)
        {
            return Database.QueryAsync<Message>(string.Format(@"Select * from Conversation c
                                                join UsersInConversation uc on uc.ConversationId = c.ConversationId
                                                join Message m on m.ConversationId = uc.ConversationId
                                                where uc.UserId = {0}
                                                group by c.ConversationId
                                                order by LastActivity desc limit 5", userId));
        }

        public Task<List<Message>> GetLastMessagesOfConversation(int conversationId)
        {
            return Database.QueryAsync<Message>(string.Format(@"Select * from Messages
                                                where ConversationId = {0}
                                                order by MessageId desc limit 5", conversationId));
        }

    }
}
