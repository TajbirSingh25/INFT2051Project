using Mauiapp1.Views;
using Microsoft.Maui.Controls;

namespace Mauiapp1
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Register routes for navigation
            Routing.RegisterRoute("LoginPage", typeof(LoginPage));
            Routing.RegisterRoute("MainPage", typeof(MainPage));
            Routing.RegisterRoute("RegisterPage", typeof(RegisterPage));

            Routing.RegisterRoute("ProfilePage", typeof(ProfilePage));
            Routing.RegisterRoute(nameof(ListItemPage), typeof(ListItemPage));


            Routing.RegisterRoute(nameof(SearchResultsPage), typeof(SearchResultsPage));
            Routing.RegisterRoute(nameof(ListingDetailPage), typeof(ListingDetailPage));
            Routing.RegisterRoute(nameof(ChatPage), typeof(ChatPage));

        }
    }
}