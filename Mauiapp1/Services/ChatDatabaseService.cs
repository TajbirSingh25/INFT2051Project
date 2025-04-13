using System.Collections.ObjectModel;
using Mauiapp1.Models;
using SQLite;

namespace Mauiapp1.Services
{
    public class ChatDatabaseService
    {
        private SQLiteAsyncConnection _database;
        private static ChatDatabaseService _instance;
        private TaskCompletionSource _initializationCompletionSource = new TaskCompletionSource();

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
            InitializeAsync();
        }

        private async void InitializeAsync()
        {
            try
            {
                if (_database != null)
                {
                    _initializationCompletionSource.TrySetResult();
                    return;
                }

                string dbPath = Path.Combine(FileSystem.AppDataDirectory, "chat_messages.db");
                _database = new SQLiteAsyncConnection(dbPath);
                await _database.CreateTableAsync<ChatMessage>();
                _initializationCompletionSource.TrySetResult();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database initialization error: {ex.Message}");
                _initializationCompletionSource.TrySetException(ex);
            }
        }

        public async Task<ObservableCollection<ChatMessage>> GetMessagesForListingAsync(int listingId, string userId)
        {
            await WaitForDatabaseInitialization();

            try
            {
                var chatSessionId = $"{userId}_{listingId}";
                var messages = await _database.Table<ChatMessage>()
                    .Where(m => m.ChatSessionId == chatSessionId)
                    .OrderBy(m => m.Timestamp)
                    .ToListAsync();

                return new ObservableCollection<ChatMessage>(messages);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting messages: {ex.Message}");
                return new ObservableCollection<ChatMessage>();
            }
        }

        public async Task SaveMessageAsync(ChatMessage message)
        {
            await WaitForDatabaseInitialization();

            try
            {
                if (message.Id == 0)
                {
                    await _database.InsertAsync(message);
                }
                else
                {
                    await _database.UpdateAsync(message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving message: {ex.Message}");
                throw;
            }
        }

        public async Task ClearChatHistoryAsync(int listingId, string userId)
        {
            await WaitForDatabaseInitialization();

            try
            {
                var chatSessionId = $"{userId}_{listingId}";
                var messagesToDelete = await _database.Table<ChatMessage>()
                    .Where(m => m.ChatSessionId == chatSessionId)
                    .ToListAsync();

                foreach (var message in messagesToDelete)
                {
                    await _database.DeleteAsync(message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error clearing chat history: {ex.Message}");
                throw;
            }
        }

        private async Task WaitForDatabaseInitialization()
        {
            await _initializationCompletionSource.Task;
        }
    }
}