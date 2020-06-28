using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xamFixes.Models;

namespace xamFixes.Repository
{
    public class FixesDatabase
    {
        static readonly Lazy<SQLiteAsyncConnection> lazyInitializer = new Lazy<SQLiteAsyncConnection>(() =>
        {
            return new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
        });

        public static SQLiteAsyncConnection Database => lazyInitializer.Value;
        static bool initialized = false;

        public FixesDatabase()
        {
            InitializeAsync().SafeFireAndForget(false);
        }

        async Task InitializeAsync()
        {
            if (!initialized)
            {
                if (!Database.TableMappings.Any(m => m.MappedType.Name == typeof(User).Name))
                {
                    await Database.CreateTablesAsync(CreateFlags.None, typeof(User)).ConfigureAwait(false);
                    await Database.CreateTablesAsync(CreateFlags.None, typeof(Conversation)).ConfigureAwait(false);
                    await Database.CreateTablesAsync(CreateFlags.None, typeof(UsersInConversation)).ConfigureAwait(false);
                    await Database.CreateTablesAsync(CreateFlags.None, typeof(Message)).ConfigureAwait(false);

                    initialized = true;
                }
            }
        }
    }
}
