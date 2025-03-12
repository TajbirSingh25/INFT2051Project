using Microsoft.Maui.Controls;
using Mauiapp1.ViewModels;

namespace Mauiapp1
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage(LoginViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}