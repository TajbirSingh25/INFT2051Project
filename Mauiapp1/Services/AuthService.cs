using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Mauiapp1.Models;

namespace Mauiapp1.Services
{
    public class AuthService : IAuthService
    {
        private readonly ILogger<AuthService> _logger;
        private readonly IDatabaseService _databaseService;
        private readonly IPasswordHasher _passwordHasher;
        
        public AuthService(
            ILogger<AuthService> logger,
            IDatabaseService databaseService,
            IPasswordHasher passwordHasher)
        {
            _logger = logger;
            _databaseService = databaseService;
            _passwordHasher = passwordHasher;
        }
        
        public bool IsAuthenticated { get; private set; }
        public string CurrentUsername { get; private set; }
        
        public async Task<bool> LoginAsync(string username, string password)
        {
            _logger.LogInformation("Attempting to log in with username: {Username}", username);
            
            try
            {
                var user = await _databaseService.GetUserAsync(username);
                
                if (user == null)
                {
                    _logger.LogInformation("User not found: {Username}", username);
                    return false;
                }
                
                bool isPasswordValid = _passwordHasher.VerifyPassword(password, user.PasswordHash);
                
                if (isPasswordValid)
                {
                    IsAuthenticated = true;
                    CurrentUsername = username;
                    
                    // Update last login time
                    user.LastLogin = DateTime.UtcNow;
                    await _databaseService.UpdateUserAsync(user);
                    
                    _logger.LogInformation("Login successful for username: {Username}", username);
                    return true;
                }
                
                _logger.LogInformation("Invalid password for username: {Username}", username);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for username: {Username}", username);
                return false;
            }
        }
        
        public async Task LogoutAsync()
        {
            _logger.LogInformation("Logging out user: {Username}", CurrentUsername);
            IsAuthenticated = false;
            CurrentUsername = null;
        }
        
        public async Task<bool> RegisterUserAsync(string username, string password, string email, string firstName, string lastName)
        {
            try
            {
                if (await _databaseService.UserExistsAsync(username))
                {
                    _logger.LogInformation("Registration failed: Username already exists: {Username}", username);
                    return false;
                }
                
                var user = new User
                {
                    Username = username,
                    PasswordHash = _passwordHasher.HashPassword(password),
                    Email = email,
                    FirstName = firstName,
                    LastName = lastName
                };
                
                var result = await _databaseService.AddUserAsync(user);
                
                if (result)
                {
                    _logger.LogInformation("User registered successfully: {Username}", username);
                    return true;
                }
                
                _logger.LogInformation("Failed to register user: {Username}", username);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during user registration: {Username}", username);
                return false;
            }
        }
    }
}