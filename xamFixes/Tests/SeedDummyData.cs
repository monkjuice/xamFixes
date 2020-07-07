using System;
using System.Collections.Generic;
using System.Text;
using xamFixes.Models;
using xamFixes.Repository.ORM;
using xamFixes.DBModel;

namespace xamFixes.Tests
{
    class SeedDummyData
    {

        async public void Execute()
        {
            var users = new List<User>()
            {
                new User(){ ProfilePicturePath="profile.png", Username= "Starlight"},
                new User(){ ProfilePicturePath="profile.png", Username="Bobby"},
                new User(){ ProfilePicturePath="profile.png", Username="Samuel"}
            };

            var conversations = new List<Conversation>()
            {
                new Conversation() { LastActivity = DateTime.Now },
                new Conversation() { LastActivity = DateTime.Now }
            };

            var userInConv = new List<UserInConversation>()
            {
                // Conversation A
                new UserInConversation() { ConversationId = 1, UserId = 1 },
                new UserInConversation() { ConversationId = 1, UserId = 2 },

                // Conversation B
                new UserInConversation() { ConversationId = 2, UserId = 1 },
                new UserInConversation() { ConversationId = 2, UserId = 3 },
            };

            var msgs = new List<Message>()
            {
                // Conversation A
                new Message() { UserId = 2, ConversationId = 1, Body = "Hello", Unread = false },
                new Message() { UserId = 1, ConversationId = 1, Body = "Hello, how are you?", Unread = false },
                new Message() { UserId = 2, ConversationId = 1, Body = "All good bro", Unread = true },

                // Conversation B
                new Message() { UserId = 1, ConversationId = 2, Body = "Hello", Unread = false },
                new Message() { UserId = 1, ConversationId = 2, Body = "Nice flipflops", Unread = false },
                new Message() { UserId = 3, ConversationId = 2, Body = "Thanks bro", Unread = false },
                new Message() { UserId = 3, ConversationId = 2, Body = "Did you see that cat?", Unread = false },
                new Message() { UserId = 1, ConversationId = 2, Body = "Indeed", Unread = false },
            };

            try
            {
                // insert Users
                var userRepo = new UserRepo();
                foreach (User user in users)
                    await userRepo.SaveItemAsync(user);

                // insert convos with msgs
                var inboxRepo = new InboxRepo();

                foreach (var c in conversations)
                    await inboxRepo.SaveConversationAsync(c);

                foreach (var uic in userInConv)
                    await inboxRepo.SaveUserInConversationAsync(uic);

                foreach (var msg in msgs)
                    await inboxRepo.SaveMessage(msg);

                return;
            }
            catch (Exception e)
            {
                throw e;
            }


        }

    }
}
