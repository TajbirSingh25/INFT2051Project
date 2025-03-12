using Microsoft.Maui.Controls;

namespace Mauiapp1.Services
{
    public class NavigationService : INavigationService
    {
        public async Task NavigateToAsync(string route)
        {
            await Shell.Current.GoToAsync(route);
        }

        public async Task NavigateBackAsync()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}