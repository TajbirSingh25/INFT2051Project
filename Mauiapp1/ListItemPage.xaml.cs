using Microsoft.Maui.Controls;

namespace Mauiapp1
{
    public partial class ListItemPage : ContentPage
    {
        public ListItemPage()
        {
            InitializeComponent();
        }

        private void OnUploadImageClicked(object sender, EventArgs e)
        {
            DisplayAlert("Upload Image", "Image upload functionality goes here.", "OK");
        }

        private void OnSubmitClicked(object sender, EventArgs e)
        {
            DisplayAlert("Submit", "Listing submitted!", "OK");
            Navigation.PopAsync();
        }
        private async void BackButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ProfilePage());
        }
    }
}