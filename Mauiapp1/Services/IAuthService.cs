using System.Threading.Tasks;

namespace Mauiapp1.Services
{
    public interface IAuthService
    {
        Task<bool> LoginAsync(string username, string password);
        Task LogoutAsync();
        Task<bool> RegisterUserAsync(string username, string password, string email, string firstName, string lastName);
        bool IsAuthenticated { get; }
        string CurrentUsername { get; }
    }
}
