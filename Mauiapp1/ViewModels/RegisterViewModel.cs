using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mauiapp1.Services;
using System;
using System.Threading.Tasks;

namespace Mauiapp1.ViewModels
{
    public partial class RegisterViewModel : BaseViewModel
    {
        private readonly IAuthService _authService;
        private readonly INavigationService _navigationService;

        [ObservableProperty]
        private string _username;

        [ObservableProperty]
        private string _password;

        [ObservableProperty]
        private string _confirmPassword;

        [ObservableProperty]
        private string _email;

        [ObservableProperty]
        private string _firstName;

        [ObservableProperty]
        private string _lastName;

        [ObservableProperty]
        private string _errorMessage;

        [ObservableProperty]
        private bool _isErrorVisible;

        public RegisterViewModel(IAuthService authService, INavigationService navigationService)
        {
            Title = "Register";
            _authService = authService;
            _navigationService = navigationService;
        }

        [RelayCommand]
        private async Task Register()
        {
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password) ||
                string.IsNullOrWhiteSpace(ConfirmPassword) || string.IsNullOrWhiteSpace(Email))
            {
                ErrorMessage = "Please fill in all required fields";
                IsErrorVisible = true;
                return;
            }

            if (Password != ConfirmPassword)
            {
                ErrorMessage = "Passwords do not match";
                IsErrorVisible = true;
                return;
            }

            await ExecuteAsync(RegisterAsync());
        }

        private async Task RegisterAsync()
        {
            try
            {
                IsErrorVisible = false;
                Console.WriteLine("Calling auth service for registration");

                bool isRegistered = await _authService.RegisterUserAsync(
                    Username, Password, Email, FirstName, LastName);

                Console.WriteLine($"Registration result: {isRegistered}");

                if (isRegistered)
                {
                    Console.WriteLine("Registration successful, navigating to LoginPage");
                    // Fix the navigation URI format
                    await _navigationService.NavigateToAsync("///LoginPage");
                }
                else
                {
                    ErrorMessage = "Registration failed. Username may already exist.";
                    IsErrorVisible = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in RegisterAsync: {ex.Message}");
                ErrorMessage = $"Registration failed: {ex.Message}";
                IsErrorVisible = true;
            }
        }

        [RelayCommand]
        private async Task GoToLogin()
        {
            Console.WriteLine("GoToLogin command invoked");
            await _navigationService.NavigateToAsync("///LoginPage");
        }
    }
}