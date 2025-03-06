
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mauiapp1;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MauiApp1.ViewModels
{
    public partial class LoginPageViewModel : BaseViewModel
    {
        [ObservableProperty]
        private string username;

        [ObservableProperty]
        private string password;

        public LoginPageViewModel()
        {
            Title = "Login";
        }

        [RelayCommand]
        private async Task LoginAsync()
        {
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                await App.Current.MainPage.DisplayAlert("Error", "Please enter both username and password.", "OK");
                return;
            }

            // Simulated authentication (replace with real authentication logic)
            if (Username == "admin" && Password == "password123")
            {
                await App.Current.MainPage.DisplayAlert("Success", "Login successful!", "OK");
                // Navigate to another page (update this with actual navigation logic)
            }
            else
            {
                await App.Current.MainPage.DisplayAlert("Error", "Invalid credentials. Try again.", "OK");
            }
        }

        public ICommand LoginCommand => new AsyncRelayCommand(LoginAsync);
    }
}
