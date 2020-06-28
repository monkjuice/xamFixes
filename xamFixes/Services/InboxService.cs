using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using xamFixes.Interfaces;
using xamFixes.Models;
using xamFixes.Repository.ORM;

namespace xamFixes.Services
{
    public class InboxService : IInboxService
    {
        async public Task<List<Message>> GetLastConversations()
        {
            var db = new InboxRepo();

            return await db.GetLastConversations(App.UserId);
        }
    }
}
