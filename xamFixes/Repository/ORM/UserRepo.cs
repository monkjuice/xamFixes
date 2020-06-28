using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using xamFixes.Models;

namespace xamFixes.Repository.ORM
{
    public class UserRepo : FixesDatabase
    {

        public Task<List<User>> GetItemsAsync()
        {
            return Database.Table<User>().ToListAsync();
        }

        public Task<List<User>> GetItemsNotDoneAsync()
        {
            // SQL queries are also possible
            return Database.QueryAsync<User>("SELECT * FROM [TodoItem] WHERE [Done] = 0");
        }

        public Task<User> GetItemAsync(int id)
        {
            return Database.Table<User>().Where(i => i.UserId == id).FirstOrDefaultAsync();
        }

        public Task<int> SaveItemAsync(User user)
        {
            if (user.UserId != 0)
            {
                return Database.UpdateAsync(user);
            }
            else
            {
                return Database.InsertAsync(user);
            }
        }

        public Task<int> DeleteItemAsync(User user)
        {
            return Database.DeleteAsync(user);
        }

    }
}
