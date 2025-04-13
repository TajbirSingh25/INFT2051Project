using System.Collections.ObjectModel;
using Mauiapp1.Models;
using Mauiapp1.Services;

namespace Mauiapp1.Views
{
    public partial class ListingDetailPage : ContentPage
    {
        private Listing _listing;
        private readonly IDatabaseService _databaseService;

        // Properties for binding
        public ObservableCollection<Listing> MainListings { get; set; }
        public ObservableCollection<Listing> SearchResults { get; set; }
        public bool HasSearchResults => SearchResults?.Count > 0;
        private bool _isFromSearch = false;

        // Updated constructor for when coming from MainPage
        public ListingDetailPage(Listing listing, IDatabaseService databaseService = null)
        {
            InitializeComponent();
            _listing = listing;
            _databaseService = databaseService;

            // Initialize the collections
            MainListings = new ObservableCollection<Listing>();
            SearchResults = new ObservableCollection<Listing>();

            // Load MainPage listings if database service is available
            if (_databaseService != null)
            {
                LoadMainListings();
            }

            // Set up the binding context with both the current listing and the collections
            SetupBindingContext();
        }

        // Constructor for when coming from SearchResultsPage
        public ListingDetailPage(Listing listing, ObservableCollection<Listing> searchResults, IDatabaseService databaseService = null)
        {
            InitializeComponent();
            _listing = listing;
            _databaseService = databaseService;
            _isFromSearch = true;

            // Initialize the collections
            MainListings = new ObservableCollection<Listing>();
            SearchResults = new ObservableCollection<Listing>(searchResults.Where(l => l.Id != listing.Id));

            // Load MainPage listings if database service is available
            if (_databaseService != null)
            {
                LoadMainListings();
            }

            // Set up the binding context with both the current listing and the collections
            SetupBindingContext();
        }

        private void SetupBindingContext()
        {
            // Create a dynamic object to hold all binding properties
            var bindingContext = new
            {
                // Properties from the selected listing
                _listing.Id,
                _listing.Title,
                _listing.Price,
                _listing.Description,
                _listing.Condition,
                _listing.Category,
                _listing.ImageUrl,
                _listing.SellerName,

                // The collections
                MainListings = this.MainListings,
                SearchResults = this.SearchResults,
                HasSearchResults = this.HasSearchResults
            };

            // Set the binding context
            BindingContext = bindingContext;
        }

        private void LoadMainListings()
        {
            // In a real app, you would get this from the database
            // For now, we'll use mock data similar to what's in MainPage
            var allListings = GetMockListings();

            // Don't include the current listing in the main listings
            var filteredListings = allListings.Where(l => l.Id != _listing.Id).ToList();

            // Take up to 5 listings to show
            foreach (var item in filteredListings.Take(5))
            {
                MainListings.Add(item);
            }
        }

        private List<Listing> GetMockListings()
        {
            // Mock data for testing, same as in MainPage
            return new List<Listing>
            {
                new Listing { Id = 1, Title = "Rolex Watch", Price = 8500, Description = "Luxury Rolex Submariner watch in excellent condition.", Condition = "Like New", Category = "Watches", ImageUrl = "rolex.jpg", SellerName = "LuxuryWatchSeller" },
                new Listing { Id = 2, Title = "Tesla Model 3", Price = 35000, Description = "2021 Tesla Model 3 with low mileage.", Condition = "Used - Excellent", Category = "Cars", ImageUrl = "tesla.jpg", SellerName = "EVLover" },
                new Listing { Id = 3, Title = "iPhone 13 Pro", Price = 650, Description = "iPhone 13 Pro in Sierra Blue with 256GB storage.", Condition = "Used - Good", Category = "Electronics", ImageUrl = "iphone.jpg", SellerName = "GadgetGuru" },
                new Listing { Id = 4, Title = "Samsung QLED TV", Price = 800, Description = "55-inch Samsung QLED 4K Smart TV with remote.", Condition = "Like New", Category = "Electronics", ImageUrl = "samsung.jpg", SellerName = "ElectronicsShop" },
                new Listing { Id = 5, Title = "Toyota Camry", Price = 15000, Description = "2018 Toyota Camry SE with 45,000 miles.", Condition = "Used - Good", Category = "Cars", ImageUrl = "toyota.jpg", SellerName = "CarDealer" },
                new Listing { Id = 6, Title = "Seiko Automatic Watch", Price = 250, Description = "Seiko 5 automatic watch with stainless steel bracelet.", Condition = "Used - Excellent", Category = "Watches", ImageUrl = "seiko.jpg", SellerName = "WatchEnthusiast" },
                new Listing { Id = 7, Title = "Classic Case Studies in Psychology", Price = 66, Description = "Classic Case Studies in Psychology", Condition = "Good-reading", Category = "Books", ImageUrl = "classicbook.jpg", SellerName = "Book shop" },
                new Listing { Id = 8, Title = "TENSSENX", Price = 129, Description = "Terrain Off Road Monster RC Truck, IPX5 Electric Vehicle Toys", Condition = "Kids - Excellent", Category = "Toys", ImageUrl = "toy.jpg", SellerName = "Toyshop" },
                new Listing { Id = 9, Title = "Philips Air Purifier", Price = 91, Description = "With just one push of a button, the air purifier filters invisible viruses.", Condition = "Used - Excellent", Category = "Home Appliances", ImageUrl = "purifier.jpg", SellerName = "Hardware Shop" },
                new Listing { Id = 10, Title = "The Tiger Who Came to Tea", Price = 46, Description = "The classic picture book story of Sophie and her extraordinary teatime guest.", Condition = "Fun - Words", Category = "Books", ImageUrl = "tigerbook.jpg", SellerName = "Book Shop" },
            };
        }

        // Override the hardware back button behavior (for Android)
        protected override bool OnBackButtonPressed()
        {
            // Use the same logic as the custom back button
            if (_databaseService != null)
            {
                Navigation.PushAsync(new MainPage(_databaseService));
            }
            else
            {
                Navigation.PopAsync();
            }

            // Return true to indicate we've handled the back button
            return true;
        }

        // Updated to match your request
        private async void BackButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MainPage(_databaseService));
        }

        private async void OnContactSellerClicked(object sender, EventArgs e)
        {
            // Navigate to the chat page with the current listing
            await Navigation.PushAsync(new ChatPage(_listing));
        }

        private async void OnMakeOfferClicked(object sender, EventArgs e)
        {
            string result = await DisplayPromptAsync("Make an Offer",
                $"Enter your offer for {_listing.Title}:",
                "Submit", "Cancel",
                "Enter amount",
                keyboard: Keyboard.Numeric);

            if (!string.IsNullOrEmpty(result))
            {
                if (decimal.TryParse(result, out decimal offerAmount))
                {
                    await DisplayAlert("Offer Submitted",
                        $"Your offer of ${offerAmount} has been sent to {_listing.SellerName}",
                        "OK");
                    // In a real app, you would save this offer in a database
                }
                else
                {
                    await DisplayAlert("Invalid Amount", "Please enter a valid number", "OK");
                }
            }
        }

        // Event handlers for the main listings collection
        private void OnMainListingSelected(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is Listing selectedListing)
            {
                // Reset selection
                MainListingsCollection.SelectedItem = null;

                // Navigate to the selected listing
                NavigateToNewListing(selectedListing);
            }
        }

        private void OnMainListingTapped(object sender, TappedEventArgs e)
        {
            if (e.Parameter is Listing tappedListing)
            {
                NavigateToNewListing(tappedListing);
            }
        }

        // Event handlers for the search results collection
        private void OnSearchResultSelected(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is Listing selectedListing)
            {
                // Reset selection
                SearchResultsCollection.SelectedItem = null;

                // Navigate to the selected listing
                NavigateToNewListing(selectedListing);
            }
        }

        private void OnSearchResultTapped(object sender, TappedEventArgs e)
        {
            if (e.Parameter is Listing tappedListing)
            {
                NavigateToNewListing(tappedListing);
            }
        }

        // Common method to navigate to a new listing
        private async void NavigateToNewListing(Listing listing)
        {
            try
            {
                // If the navigation came from a search, include search results
                if (_isFromSearch)
                {
                    var detailPage = new ListingDetailPage(listing, SearchResults, _databaseService);
                    await Navigation.PushAsync(detailPage);
                }
                else
                {
                    var detailPage = new ListingDetailPage(listing, _databaseService);
                    await Navigation.PushAsync(detailPage);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Navigation Error", $"Error navigating to details: {ex.Message}", "OK");
            }
        }
    }
}