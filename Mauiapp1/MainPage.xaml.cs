using System.Collections.ObjectModel;
using System.Reflection;
using Mauiapp1.Models;
using Microsoft.Maui.Controls;
using Mauiapp1.Services;

namespace Mauiapp1
{
    public partial class MainPage : ContentPage
    {
        private readonly IDatabaseService _databaseService;
        public ObservableCollection<Listing> Listings { get; set; }

        public MainPage(IDatabaseService databaseService)
        {
            InitializeComponent();
            _databaseService = databaseService;

            // Initialize listings
            Listings = new ObservableCollection<Listing>
            {
                new Listing { ImageUrl = "listing1.jpg" },
                new Listing { ImageUrl = "listing2.jpg" },
                new Listing { ImageUrl = "listing3.jpg" },
                new Listing { ImageUrl = "listing4.jpg" },
                new Listing { ImageUrl = "listing5.jpg" },
                new Listing { ImageUrl = "listing6.jpg" }
            };

            BindingContext = this;
        }

        // Event handlers
        private void OnCategoryClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            DisplayAlert("Category Clicked", $"You clicked {button.Text}", "OK");
        }

        private void OnSeeMoreClicked(object sender, EventArgs e)
        {
            DisplayAlert("See More", "Showing more categories...", "OK");
        }




        private void OnAddItemClicked(object sender, EventArgs e)
        {
            // Add your code here to handle the add item action
        }
        private async void OnProfileClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ProfilePage(_databaseService));
        }
    }
}