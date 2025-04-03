using Microsoft.Maui.Storage;
using System.ComponentModel;

namespace Mauiapp1;

public partial class ListItemPage : ContentPage
{
    private FileResult _selectedImageFile;

    public ListItemPage()
    {
        InitializeComponent();
    }

    private void BackButton_Clicked(object sender, EventArgs e)
    {
        Navigation.PopAsync();
    }

    private async void OnAddImageClicked(object sender, EventArgs e)
    {
        var action = await DisplayActionSheet("Select Image", "Cancel", null,
            "Take Photo", "Choose from Gallery");

        switch (action)
        {
            case "Take Photo":
                await TakePhotoAsync();
                break;
            case "Choose from Gallery":
                await PickPhotoAsync();
                break;
        }
    }

    private async Task TakePhotoAsync()
    {
        try
        {
            var photo = await MediaPicker.CapturePhotoAsync();
            await LoadPhotoAsync(photo);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to take photo: {ex.Message}", "OK");
        }
    }

    private async Task PickPhotoAsync()
    {
        try
        {
            var photo = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
            {
                Title = "Select Product Image"
            });
            await LoadPhotoAsync(photo);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to pick photo: {ex.Message}", "OK");
        }
    }

    private async Task LoadPhotoAsync(FileResult photo)
    {
        if (photo == null)
            return;

        _selectedImageFile = photo;

        var stream = await photo.OpenReadAsync();
        SelectedImage.Source = ImageSource.FromStream(() => stream);

        AddImageButton.IsVisible = false;
        ImageActionButtons.IsVisible = true;
    }

    private void OnChangeImageClicked(object sender, EventArgs e)
    {
        // Changed from async void to just calling the method without await
        OnAddImageClicked(sender, e);
    }

    private void OnDeleteImageClicked(object sender, EventArgs e)
    {
        SelectedImage.Source = null;
        _selectedImageFile = null;
        AddImageButton.IsVisible = true;
        ImageActionButtons.IsVisible = false;
    }

    private async void OnSubmitClicked(object sender, EventArgs e)
    {
        // Validate inputs
        if (string.IsNullOrWhiteSpace(ProductNameEntry.Text))
        {
            await DisplayAlert("Validation Error", "Please enter a product name", "OK");
            return;
        }

        if (ProductTypePicker.SelectedIndex == -1)
        {
            await DisplayAlert("Validation Error", "Please select a product type", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(PriceEntry.Text))
        {
            await DisplayAlert("Validation Error", "Please enter a price", "OK");
            return;
        }

        // Validate price is a number
        if (!decimal.TryParse(PriceEntry.Text, out decimal price))
        {
            await DisplayAlert("Validation Error", "Please enter a valid price", "OK");
            return;
        }

        // Create product object
        var product = new Product
        {
            Name = ProductNameEntry.Text,
            Description = ProductDescriptionEditor.Text,
            Type = ProductTypePicker.SelectedItem.ToString(),
            Price = price,
            ImagePath = await SaveImageAsync()
        };

        // Save to database (you'll need to implement this)
        await SaveProductToDatabase(product);

        // Navigate back or show success message
        await DisplayAlert("Success", "Product listed successfully!", "OK");
        await Navigation.PopAsync();
    }

    private async Task<string> SaveImageAsync()
    {
        if (_selectedImageFile == null)
            return null;

        try
        {
            // Create a unique filename
            var fileName = $"{Guid.NewGuid()}_{_selectedImageFile.FileName}";

            // Get the app's local storage path
            var destinationPath = Path.Combine(
                FileSystem.AppDataDirectory,
                "ProductImages",
                fileName
            );

            // Ensure directory exists
            Directory.CreateDirectory(Path.GetDirectoryName(destinationPath));

            // Copy the file
            using var sourceStream = await _selectedImageFile.OpenReadAsync();
            using var destinationStream = File.Create(destinationPath);
            await sourceStream.CopyToAsync(destinationStream);

            return destinationPath;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to save image: {ex.Message}", "OK");
            return null;
        }
    }

    private async Task SaveProductToDatabase(Product product)
    {
        // Implement your database saving logic here
        // This is a placeholder - you'll need to use your specific database approach
        // For example, using SQLite:
        // await _database.InsertProductAsync(product);

        // Simulate database save
        await Task.Delay(500);
    }
}

// Product model class
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Type { get; set; }
    public decimal Price { get; set; }
    public string ImagePath { get; set; }
}