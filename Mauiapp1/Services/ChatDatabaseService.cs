using System.Collections.ObjectModel;
using Mauiapp1.Models;
using SQLite;

namespace Mauiapp1.Services
{
    public class ChatDatabaseService
    {
        private SQLiteAsyncConnection _database;
        private static ChatDatabaseService _instance;

        public static ChatDatabaseService Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ChatDatabaseService();
                }
                return _instance;
            }
        }

        private ChatDatabaseService()
        {
            Initialize();
        }

        private async void Initialize()
        {
            if (_database != null)
                return;

            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "chat_messages.db");
            _database = new SQLiteAsyncConnection(dbPath);

            await _database.CreateTableAsync<ChatMessage>();
        }

        public async Task<ObservableCollection<ChatMessage>> GetMessagesForListingAsync(int listingId, string userId)
        {
            await WaitForDatabaseInitialization();

            var chatSessionId = $"{userId}_{listingId}";
            var messages = await _database.Table<ChatMessage>()
                .Where(m => m.ChatSessionId == chatSessionId)
                .OrderBy(m => m.Timestamp)
                .ToListAsync();

            return new ObservableCollection<ChatMessage>(messages);
        }

        public async Task SaveMessageAsync(ChatMessage message)
        {
            await WaitForDatabaseInitialization();

            if (message.Id == 0)
            {
                await _database.InsertAsync(message);
            }
            else
            {
                await _database.UpdateAsync(message);
            }
        }

        public async Task ClearChatHistoryAsync(int listingId, string userId)
        {
            await WaitForDatabaseInitialization();

            var chatSessionId = $"{userId}_{listingId}";
            // Fix the lambda expression issue by using Table and Where
            var messagesToDelete = await _database.Table<ChatMessage>()
                .Where(m => m.ChatSessionId == chatSessionId)
                .ToListAsync();

            foreach (var message in messagesToDelete)
            {
                await _database.DeleteAsync(message);
            }
        }

        private async Task WaitForDatabaseInitialization()
        {
            if (_database == null)
            {
                Initialize();

                // Give the database a moment to initialize
                int attempts = 0;
                while (_database == null && attempts < 5)
                {
                    await Task.Delay(100);
                    attempts++;
                }

                if (_database == null)
                {
                    throw new Exception("Database failed to initialize");
                }
            }
        }
    }
}
