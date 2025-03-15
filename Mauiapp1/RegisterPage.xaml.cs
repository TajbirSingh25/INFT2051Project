using Microsoft.Maui.Controls;
using Mauiapp1.ViewModels;

namespace Mauiapp1
{
    public partial class RegisterPage : ContentPage
    {
        public RegisterPage(RegisterViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}