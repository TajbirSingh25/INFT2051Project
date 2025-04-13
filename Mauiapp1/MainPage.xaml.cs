using System.Collections.ObjectModel;
using System.Reflection;
using Mauiapp1.Models;
using Microsoft.Maui.Controls;
using Mauiapp1.Services;
using Mauiapp1.Views;

namespace Mauiapp1
{
    public partial class MainPage : ContentPage
    {
        private readonly IDatabaseService _databaseService;
        public ObservableCollection<Listing> Listings { get; set; }
        private bool _moreCategoriesVisible = false;

        public MainPage(IDatabaseService databaseService)
        {
            InitializeComponent();
            _databaseService = databaseService;

            // Initialize listings with more detailed data
            Listings = new ObservableCollection<Listing>
            {
                new Listing {
                    Id = 1,
                    Title = "Rolex Watch",
                    Price = 8500,
                    Description = "Luxury Rolex Submariner watch in excellent condition. Water-resistant to 300 meters with automatic movement.",
                    Condition = "Like New",
                    Category = "Watches",
                    ImageUrl = "rolex.jpg",
                    SellerName = "LuxuryWatchSeller"
                },
                new Listing {
                    Id = 2,
                    Title = "Tesla Model 3",
                    Price = 35000,
                    Description = "2021 Tesla Model 3 with low mileage. Autopilot features included. Blue exterior with white interior.",
                    Condition = "Used - Excellent",
                    Category = "Cars",
                    ImageUrl = "tesla.jpg",
                    SellerName = "EVLover"
                },
                new Listing {
                    Id = 3,
                    Title = "iPhone 13 Pro",
                    Price = 650,
                    Description = "iPhone 13 Pro in Sierra Blue with 256GB storage. Includes original box and charger.",
                    Condition = "Used - Good",
                    Category = "Electronics",
                    ImageUrl = "iphone.jpg",
                    SellerName = "GadgetGuru"
                },
                new Listing {
                    Id = 4,
                    Title = "Samsung QLED TV",
                    Price = 800,
                    Description = "55-inch Samsung QLED 4K Smart TV with remote. Perfect for gaming and streaming.",
                    Condition = "Like New",
                    Category = "Electronics",
                    ImageUrl = "samsung.jpg",
                    SellerName = "ElectronicsShop"
                },
                new Listing {
                    Id = 5,
                    Title = "Toyota Camry",
                    Price = 15000,
                    Description = "2018 Toyota Camry SE with 45,000 miles. Well maintained with full service history.",
                    Condition = "Used - Good",
                    Category = "Cars",
                    ImageUrl = "toyota.jpg",
                    SellerName = "CarDealer"
                },
                new Listing {
                    Id = 6,
                    Title = "Seiko Automatic Watch",
                    Price = 250,
                    Description = "Seiko 5 automatic watch with stainless steel bracelet. Displays day and date.",
                    Condition = "Used - Excellent",
                    Category = "Watches",
                    ImageUrl = "seiko.jpg",
                    SellerName = "WatchEnthusiast"
                },
                new Listing {
                    Id = 7,
                    Title = "Classic Case Studies in Psychology",
                    Price = 66,
                    Description = "FClassic Case Studies in Psychology",
                    Condition = "Good-reading",
                    Category = "Books",
                    ImageUrl = "classicbook.jpg",
                    SellerName = "Book shop"
                },
                new Listing {
                    Id = 8,
                    Title = "TENSSENX",
                    Price = 129,
                    Description = "Terrain Off Road Monster RC Truck, IPX5 Electric Vehicle Toys Gifts for Kids and Adults",
                    Condition = "Kids - Excellent",
                    Category = "Toys",
                    ImageUrl = "toy.jpg",
                    SellerName = "Toyshop"
                },
                new Listing {
                    Id = 9,
                    Title = "Philips Air Purifier",
                    Price = 91,
                    Description = "With just one push of a button, the air purifier filters the invisible viruses,",
                    Condition = "Used - Excellent",
                    Category = "Home Appliances",
                    ImageUrl = "purifier.jpg",
                    SellerName = "Hardware Shop"
                },
                     new Listing {
                    Id = 10,
                    Title = "The Tiger Who Came to Tea",
                    Price = 46,
                    Description = "The classic picture book story of Sophie and her extraordinary teatime guest has been loved by millions of children since it was first published more than fifty years ago.",
                    Condition = "Fun - Words",
                    Category = "Books",
                    ImageUrl = "tigerbook.jpg",
                    SellerName = "Book Shop"
                },
            };

            BindingContext = this;
        }

        // Event handlers
        private async void OnCategoryClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            string category = button.Text;

            // Navigate to search results with the category as the query
            await Navigation.PushAsync(new SearchResultsPage(_databaseService, category));
        }

        private void OnSeeMoreClicked(object sender, EventArgs e)
        {
            _moreCategoriesVisible = !_moreCategoriesVisible;
            SecondRowCategories.IsVisible = _moreCategoriesVisible;
            SeeMoreButton.Text = _moreCategoriesVisible ? "See Less" : "See More";
        }

        private void OnAddItemClicked(object sender, EventArgs e)
        {
            // Add your code here to handle the add item action
        }

        private async void OnProfileClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ProfilePage(_databaseService));
        }

        private async void OnSearchButtonPressed(object sender, EventArgs e)
        {
            string query = MainSearchBar.Text;

            if (!string.IsNullOrWhiteSpace(query))
            {
                await Navigation.PushAsync(new SearchResultsPage(_databaseService, query));
            }
        }

        // Add this method to ensure correct navigation
        private async void NavigateToDetailPage(Listing listing)
        {
            try
            {
                // Create the detail page and pass the listings collection
                var detailPage = new Views.ListingDetailPage(listing, _databaseService);

                // Navigate to it
                await Navigation.PushAsync(detailPage);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Navigation Error", $"Error navigating to details: {ex.Message}", "OK");
            }
        }

        // Update these methods to use the common navigation method
        private async void OnListingSelected(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is Listing selectedListing)
            {
                // Reset selection
                ((CollectionView)sender).SelectedItem = null;

                // Navigate to listing detail page
                NavigateToDetailPage(selectedListing);
            }
        }

        private async void OnListingTapped(object sender, TappedEventArgs e)
        {
            if (e.Parameter is Listing tappedListing)
            {
                // Navigate to listing detail page
                NavigateToDetailPage(tappedListing);
            }
        }
    }
}