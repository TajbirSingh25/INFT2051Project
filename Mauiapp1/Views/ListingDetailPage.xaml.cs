using Mauiapp1.Models;

namespace Mauiapp1.Views
{
    public partial class ListingDetailPage : ContentPage
    {
        private Listing _listing;

        public ListingDetailPage(Listing listing)
        {
            InitializeComponent();
            _listing = listing;
            BindingContext = listing;

            // Add a back button to the toolbar
            ToolbarItems.Add(new ToolbarItem
            {
                Text = "Back",
                IconImageSource = "back.png", // Use your back icon if available
                Order = ToolbarItemOrder.Primary,
                Command = new Command(async () => await Navigation.PopAsync())
            });
        }

        // Override the hardware back button behavior (for Android)
        protected override bool OnBackButtonPressed()
        {
            // Navigate back to the previous page
            Navigation.PopAsync();

            // Return true to indicate we've handled the back button
            return true;
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