using SQLite;
using Mauiapp1.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mauiapp1.Services
{
    public class DatabaseService : IDatabaseService
    {
        private SQLiteAsyncConnection _database;
        private readonly string _dbPath;
        private bool _isInitialized = false;

        public DatabaseService(string dbPath)
        {
            _dbPath = dbPath;
        }

        private async Task InitializeAsync()
        {
            if (!_isInitialized)
            {
                _database = new SQLiteAsyncConnection(_dbPath);
                await _database.CreateTableAsync<User>();
                _isInitialized = true;
            }
        }

        public async Task<List<User>> GetUsersAsync()
        {
            await InitializeAsync();
            return await _database.Table<User>().ToListAsync();
        }

        public async Task<User> GetUserAsync(string username)
        {
            await InitializeAsync();
            return await _database.Table<User>()
                .Where(u => u.Username == username)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> AddUserAsync(User user)
        {
            await InitializeAsync();
            user.CreatedAt = DateTime.UtcNow;
            var result = await _database.InsertAsync(user);
            return result > 0;
        }

        public async Task<bool> UpdateUserAsync(User user)
        {
            await InitializeAsync();
            var result = await _database.UpdateAsync(user);
            return result > 0;
        }

        public async Task<bool> DeleteUserAsync(User user)
        {
            await InitializeAsync();
            var result = await _database.DeleteAsync(user);
            return result > 0;
        }

        public async Task<bool> UserExistsAsync(string username)
        {
            await InitializeAsync();
            var user = await GetUserAsync(username);
            return user != null;
        }
        public async Task CreateProductsTableAsync()
        {
            await InitializeAsync();
            await _database.CreateTableAsync<Product>();
        }

        public async Task<int> InsertProductAsync(Product product)
        {
            await InitializeAsync();
            product.CreatedAt = DateTime.UtcNow;
            return await _database.InsertAsync(product);
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {
            await InitializeAsync();
            return await _database.Table<Product>().ToListAsync();
        }

        public async Task<Product> GetProductByIdAsync(int id)
        {
            await InitializeAsync();
            return await _database.Table<Product>()
                .Where(p => p.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task<int> UpdateProductAsync(Product product)
        {
            await InitializeAsync();
            return await _database.UpdateAsync(product);
        }

        public async Task<int> DeleteProductAsync(Product product)
        {
            await InitializeAsync();
            return await _database.DeleteAsync(product);
        }
        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _database.Table<User>().ToListAsync();
        }
    }
}