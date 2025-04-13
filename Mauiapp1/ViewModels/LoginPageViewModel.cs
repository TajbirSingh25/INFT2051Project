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

        [ObservableProperty]
        private bool _rememberMe;

        public LoginViewModel(IAuthService authService, INavigationService navigationService)
        {
            Title = "Login";
            _authService = authService;
            _navigationService = navigationService;
            LoadSavedUsername();
        }

        private async void LoadSavedUsername()
        {
            try
            {
                string savedUsername = await SecureStorage.Default.GetAsync("username");
                if (!string.IsNullOrEmpty(savedUsername))
                {
                    Username = savedUsername;
                    RememberMe = true;
                }
            }
            catch (Exception)
            {
                // Handle any issues with SecureStorage
            }
        }

        [RelayCommand]
        private async Task Login()
        {
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Please enter both username and password";
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
                    // Save username if Remember Me is checked
                    if (RememberMe)
                    {
                        await SecureStorage.Default.SetAsync("username", Username);
                    }
                    else
                    {
                        SecureStorage.Default.Remove("username");
                    }

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
        private async Task Register()
        {
            try
            {
                Console.WriteLine("Register command invoked");

                // Try using Application.Current.MainPage directly for navigation
                var registerPage = new RegisterPage(new RegisterViewModel(_authService, _navigationService));
                await Application.Current.MainPage.Navigation.PushAsync(registerPage);

                Console.WriteLine("Navigation to register page completed");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Navigation exception: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }
    }
}