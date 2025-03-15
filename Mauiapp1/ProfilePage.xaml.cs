namespace Mauiapp1
{
    public partial class ProfilePage : ContentPage
    {
        public ProfilePage()
        {
            InitializeComponent();
        }
        private async void BackButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MainPage());
        }
        private void OnListItemClicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new ListItemPage());
        }
    }
}