using System;
using System.Collections.Generic;
using System.Text;
using xamFixes.Models;
using xamFixes.Repository.ORM;

namespace xamFixes.Tests
{
    class SeedDummyData
    {

        async public void Execute()
        {
            var users = new List<User>()
            {
                new User(){ UserId = 1, ProfilePicturePath="profile.png", Username= "Starlight"},
                new User(){ UserId = 2, ProfilePicturePath="profile.png", Username="Bobby"}
            };

            var conversation = new Conversation() { LastActivity = DateTime.Now };

            var userInConv = new List<UsersInConversation>()
            {
                new UsersInConversation() { ConversationId = 1, UserId = 1 },
                new UsersInConversation() { ConversationId = 1, UserId = 2 }
            };

            var msgs = new List<Message>()
            {
                new Message() { UserId = 1, ConversationId = 1, MessageBody = "Hello", MessageId = 1, Unread = true },
                new Message() { UserId = 2, ConversationId = 1, MessageBody = "Hello, how are you?", MessageId = 2, Unread = true },
                new Message() { UserId = 1, ConversationId = 1, MessageBody = "All good bro", MessageId = 3, Unread = true },
            };


            // insert users
            try 
            { 
                var userRepo = new UserRepo();
                foreach (User user in users)
                    await userRepo.SaveItemAsync(user);

                // insert convo with msgs
                var inboxRepo = new InboxRepo();
                await inboxRepo.SaveConversationAsync(conversation);

                foreach (var uic in userInConv)
                    await inboxRepo.SaveUsersInConversationAsync(uic);

                foreach (var msg in msgs)
                    await inboxRepo.SaveMessage(msg);

                var tt = await inboxRepo.GetLastConversations(App.UserId);

                return;
            }
            catch(Exception e)
            {
                throw e;
            }


        }

    }
}
