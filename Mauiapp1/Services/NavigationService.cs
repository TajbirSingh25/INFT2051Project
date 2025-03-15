using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace Mauiapp1.Services
{
    public class NavigationService : INavigationService
    {
        public async Task NavigateToAsync(string route, IDictionary<string, object> parameters = null)
        {
            Console.WriteLine($"NavigateToAsync called with route: {route}");
            try
            {
                // Format the route correctly
                if (!route.StartsWith("///") && !route.StartsWith("//"))
                {
                    route = "///" + route;
                }

                Console.WriteLine($"Attempting to navigate to: {route}");

                if (parameters != null)
                {
                    await Shell.Current.GoToAsync(route, parameters);
                }
                else
                {
                    await Shell.Current.GoToAsync(route);
                }

                Console.WriteLine("Navigation completed successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Navigation error: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                // You could display an alert here if needed
            }
        }
    }
    }
