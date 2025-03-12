using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mauiapp1.Services;

namespace Mauiapp1.ViewModels
{
    public partial class LoginViewModel : BaseViewModel
    {
        private readonly IAuthService _authService;
        private readonly INavigationService _navigationService;

        [ObservableProperty]
        private string _username;

        [ObservableProperty]
        private string _password;

        [ObservableProperty]
        private string _errorMessage;

        [ObservableProperty]
        private bool _isErrorVisible;

        public LoginViewModel(IAuthService authService, INavigationService navigationService)
        {
            Title = "Login";
            _authService = authService;
            _navigationService = navigationService;
        }

        [RelayCommand]
        private async Task Login()
        {
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Please enter username and password";
                IsErrorVisible = true;
                return;
            }

            await ExecuteAsync(LoginAsync());
        }

        private async Task LoginAsync()
        {
            try
            {
                IsErrorVisible = false;

                // Call the authentication service
                bool isAuthenticated = await _authService.LoginAsync(Username, Password);

                if (isAuthenticated)
                {
                    // Navigate to the main page on successful login
                    await _navigationService.NavigateToAsync("///MainPage");

                    // Clear the password
                    Password = string.Empty;
                }
                else
                {
                    ErrorMessage = "Invalid username or password";
                    IsErrorVisible = true;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Login failed: {ex.Message}";
                IsErrorVisible = true;
            }
        }

        [RelayCommand]
        private async Task ForgotPassword()
        {
            await _navigationService.NavigateToAsync("ForgotPasswordPage");
        }
    }
}