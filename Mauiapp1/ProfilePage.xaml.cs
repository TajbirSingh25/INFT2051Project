using Microsoft.Maui.Storage;
using Mauiapp1.Services;
using Mauiapp1.Models;
using SQLite;
using System.ComponentModel;

namespace Mauiapp1;

public partial class ProfilePage : ContentPage
{
    private readonly IDatabaseService _databaseService;

    public ProfilePage(IDatabaseService databaseService)
    {
        InitializeComponent();
        _databaseService = databaseService;
    }

    private async void BackButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new MainPage(_databaseService));
    }

    private async void OnListItemClicked(object sender, EventArgs e)
    {
        // Now we can properly create ListItemPage with its dependency
        await Navigation.PushAsync(new ListItemPage(_databaseService));
    }
}