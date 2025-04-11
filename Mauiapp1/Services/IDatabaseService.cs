using Mauiapp1.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mauiapp1.Services
{
    public interface IDatabaseService
    {
        Task<List<User>> GetUsersAsync();
        Task<User> GetUserAsync(string username);
        Task<bool> AddUserAsync(User user);
        Task<bool> UpdateUserAsync(User user);
        Task<bool> DeleteUserAsync(User user);
        Task<bool> UserExistsAsync(string username);

        // Product methods
        Task CreateProductsTableAsync();
        Task<int> InsertProductAsync(Product product);
        Task<List<Product>> GetAllProductsAsync();
        Task<Product> GetProductByIdAsync(int id);
        Task<int> UpdateProductAsync(Product product);
        Task<int> DeleteProductAsync(Product product);
    }
}