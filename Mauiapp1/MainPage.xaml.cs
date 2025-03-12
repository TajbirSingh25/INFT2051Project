﻿using System.Collections.ObjectModel;
using System.Reflection;
using Mauiapp1.Models;
using Microsoft.Maui.Controls;

namespace Mauiapp1
{
    public partial class MainPage : ContentPage
    {
        public ObservableCollection<Listing> Listings { get; set; }

        public MainPage()
        {
            InitializeComponent();

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

        private void OnListItemClicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new ListItemPage());
        }

        private void OnProfileClicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new ProfilePage());
        }
        private void OnAddItemClicked(object sender, EventArgs e)
        {
            // Handle "Add Item" icon click
            DisplayAlert("Add Item", "Navigate to the Add Item page or show a dialog.", "OK");

            // Example: Navigate to a new page for adding an item
            // Navigation.PushAsync(new AddItemPage());
        }
    }
}