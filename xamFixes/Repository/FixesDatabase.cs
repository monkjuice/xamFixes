using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using xamFixes.Models;

namespace xamFixes.Repository
{
    public class FixesDatabase
    {
        static readonly Lazy<SQLiteAsyncConnection> lazyInitializer = new Lazy<SQLiteAsyncConnection>(() =>
        {
            //File.Delete("/data/user/0/com.companyname.xamfixes/files/.local/share/1.db3");
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
                    await Database.CreateTablesAsync(CreateFlags.None, typeof(User));
                    await Database.CreateTablesAsync(CreateFlags.None, typeof(Conversation));
                    await Database.CreateTablesAsync(CreateFlags.None, typeof(UserInConversation));
                    await Database.CreateTablesAsync(CreateFlags.None, typeof(Message));

                    initialized = true;
                }
            }
        }
    }
}
