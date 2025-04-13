using System.Collections.ObjectModel;
using Mauiapp1.Models;
using Mauiapp1.Services;

namespace Mauiapp1.Views
{
    public partial class SearchResultsPage : ContentPage
    {
        private readonly IDatabaseService _databaseService;
        public ObservableCollection<Listing> SearchResults { get; private set; }

        public SearchResultsPage(IDatabaseService databaseService, string searchQuery)
        {
            InitializeComponent();
            _databaseService = databaseService;

            SearchResults = new ObservableCollection<Listing>();
            ResultsCollectionView.ItemsSource = SearchResults;

            // Set the search bar text to the query
            SearchBar.Text = searchQuery;

            // Perform initial search
            PerformSearch(searchQuery);
        }

        private void PerformSearch(string searchQuery)
        {
            // Clear previous results
            SearchResults.Clear();

            if (string.IsNullOrWhiteSpace(searchQuery))
                return;

            // For demo purposes, we'll add some mock data based on the search query
            // In a real app, you would query your database or API
            var mockData = GetMockListings().Where(l =>
                (l.Title?.Contains(searchQuery, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (l.Category?.Contains(searchQuery, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (l.Description?.Contains(searchQuery, StringComparison.OrdinalIgnoreCase) ?? false)
            ).ToList();

            // In a real app, you would use your database service:
            // var results = await _databaseService.SearchListingsAsync(searchQuery);

            foreach (var item in mockData)
            {
                SearchResults.Add(item);
            }
        }

        private List<Listing> GetMockListings()
        {
            // Mock data for testing
            return new List<Listing>
            {
                new Listing { Id = 1, Title = "Rolex Submariner", Description = "Luxury dive watch in excellent condition", Category = "Watches", Price = 8500, ImageUrl = "rolex.jpg", SellerName = "LuxuryWatchSeller" },
                new Listing { Id = 2, Title = "Seiko 5", Description = "Affordable automatic watch", Category = "Watches", Price = 120, ImageUrl = "seiko.jpg", SellerName = "WatchEnthusiast" },
                new Listing { Id = 3, Title = "Tesla Model 3", Description = "Electric sedan, low mileage", Category = "Cars", Price = 35000, ImageUrl = "tesla.jpg", SellerName = "EVLover" },
                new Listing { Id = 4, Title = "Toyota Camry", Description = "Reliable sedan with good fuel economy", Category = "Cars", Price = 15000, ImageUrl = "toyota.jpg", SellerName = "CarDealer" },
                new Listing { Id = 5, Title = "iPhone 13", Description = "Slightly used, great condition", Category = "Electronics", Price = 650, ImageUrl = "iphone.jpg", SellerName = "GadgetGuru" },
                new Listing { Id = 6, Title = "Samsung TV", Description = "55-inch QLED smart TV", Category = "Electronics", Price = 800, ImageUrl = "samsung.jpg", SellerName = "ElectronicsShop" },
                new Listing { Id = 7, Title = "Classic Case Studies in Psychology", Price = 66, Description = "FClassic Case Studies in Psychology", Condition = "Good-reading", Category = "Books", ImageUrl = "classicbook.jpg", SellerName = "Book shop" },
                new Listing { Id = 8, Title = "TENSSENX", Price = 129, Description = "Terrain Off Road Monster RC Truck, IPX5 Electric Vehicle Toys Gifts for Kids and Adults", Condition = "Kids - Excellent", Category = "Toys", ImageUrl = "toy.jpg", SellerName = "Toyshop" },
                new Listing { Id = 9, Title = "Philips Air Purifier", Price = 91, Description = "With just one push of a button, the air purifier filters the invisible viruses,", Condition = "Used - Excellent", Category = "Home Appliances", ImageUrl = "purifier.jpg", SellerName = "Hardware Shop" },
                new Listing { Id = 10, Title = "The Tiger Who Came to Tea", Price = 46, Description = "The classic picture book story of Sophie and her extraordinary teatime guest has been loved by millions of children since it was first published more than fifty years ago.", Condition = "Fun - Words", Category = "Books", ImageUrl = "tigerbook.jpg", SellerName = "Book Shop" },
            };
        }

        private void OnSearchButtonPressed(object sender, EventArgs e)
        {
            string query = SearchBar.Text;
            PerformSearch(query);
        }

        private async void OnItemSelected(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is Listing selectedItem)
            {
                // Reset selection
                ResultsCollectionView.SelectedItem = null;

                // Navigate to listing detail page
                // In a real app, you would navigate to a detail page:
                // await Navigation.PushAsync(new ListingDetailPage(selectedItem));

                // For now, just show an alert
                await DisplayAlert("Item Selected", $"You selected: {selectedItem.Title}", "OK");
            }
        }
    }
}