using Mauiapp1.Models;
using Mauiapp1.Services;

namespace Mauiapp1.Views
{
    public partial class ListingDetailPage : ContentPage
    {
        private Listing _listing;
        private readonly IDatabaseService _databaseService;

        // Updated constructor to accept database service
        public ListingDetailPage(Listing listing, IDatabaseService databaseService = null)
        {
            InitializeComponent();
            _listing = listing;
            _databaseService = databaseService;
            BindingContext = listing;
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
    }
}