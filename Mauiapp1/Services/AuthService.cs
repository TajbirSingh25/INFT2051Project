using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Mauiapp1.Services
{
    public class AuthService : IAuthService
    {
        private readonly ILogger<AuthService> _logger;

        public AuthService(ILogger<AuthService> logger)
        {
            _logger = logger;
        }

        public bool IsAuthenticated { get; private set; }

        public async Task<bool> LoginAsync(string username, string password)
        {
            _logger.LogInformation("Attempting to log in with username: {Username}", username);

            // Simulate an authentication process
            await Task.Delay(1000); // Simulate network delay

            // Dummy authentication logic
            if (username == "user" && password == "password")
            {
                IsAuthenticated = true;
                _logger.LogInformation("Login successful for username: {Username}", username);
                return true;
            }

            IsAuthenticated = false;
            _logger.LogInformation("Login failed for username: {Username}", username);
            return false;
        }

        public async Task LogoutAsync()
        {
            _logger.LogInformation("Logging out...");
            await Task.Delay(500); // Simulate network delay
            IsAuthenticated = false;
        }
    }
}
