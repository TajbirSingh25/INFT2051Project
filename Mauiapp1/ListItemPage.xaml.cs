using Microsoft.Maui.Storage;
using Mauiapp1.Services;
using Mauiapp1.Models;
using SQLite;
using System.ComponentModel;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Media;
using UploadImageApp.Services;

namespace Mauiapp1;

public partial class ListItemPage : ContentPage
{
    UploadImage uploadImage { get; set; }
    private FileResult _selectedImageFile;
    private readonly IDatabaseService _databaseService;
    private string _imageBase64;

    public ListItemPage(IDatabaseService databaseService)
    {
        InitializeComponent();
        _databaseService = databaseService;
        uploadImage = new UploadImage(this); // Pass the page reference

        // Initialize Products table when the page loads
        InitializeProductsTable();
    }

    private async void InitializeProductsTable()
    {
        try
        {
            await _databaseService.CreateProductsTableAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error initializing products table: {ex.Message}");
        }
    }

    private async void BackButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ProfilePage(_databaseService));
    }

    private async void OnAddImageClicked(object sender, EventArgs e)
    {
        var action = await DisplayActionSheet("Select Image", "Cancel", null,
            "Take Photo", "Choose from Computer");

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
            // Check if camera is available (new way)
            if (DeviceInfo.Platform == DevicePlatform.Unknown || !MediaPicker.Default.IsCaptureSupported)
            {
                await DisplayAlert("Error", "Camera not available on this device", "OK");
                return;
            }

            // Request camera permission
#if ANDROID
            var status = await Permissions.RequestAsync<Permissions.Camera>();
            if (status != PermissionStatus.Granted)
            {
                await DisplayAlert("Permission Required", "Camera access is required to take photos", "OK");
                return;
            }
#endif

            // Launch camera and capture photo
            var photo = await MediaPicker.Default.CapturePhotoAsync();

            if (photo != null)
            {
                await LoadPhotoAsync(photo);
            }
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
            Console.WriteLine("Starting photo picker...");
            // Use the updated OpenMediaPickerAsync method that now uses FilePicker
            var img = await uploadImage.OpenMediaPickerAsync();

            if (img != null)
            {
                Console.WriteLine($"Image selected: {img.FileName}, Size: {(await img.OpenReadAsync()).Length} bytes");
                var imagefile = await uploadImage.Upload(img);

                if (imagefile != null)
                {
                    Console.WriteLine("Image processed successfully, updating UI");

                    // Create a memory stream from the base64 string
                    var imageBytes = uploadImage.StringToByteBase64(imagefile.byteBase64);
                    Console.WriteLine($"Image byte array length: {imageBytes.Length}");

                    // Store the base64 string for later use
                    _imageBase64 = imagefile.byteBase64;

                    // Set the image source
                    SelectedImage.Source = ImageSource.FromStream(() =>
                        uploadImage.ByteArrayToStream(imageBytes)
                    );

                    // Update UI state
                    _selectedImageFile = img;
                    AddImageButton.IsVisible = false;
                    ImageActionButtons.IsVisible = true;

                    Console.WriteLine("UI updated successfully");
                }
                else
                {
                    Console.WriteLine("Failed to process the image file");
                    await DisplayAlert("Error", "Failed to process the selected image", "OK");
                }
            }
            else
            {
                Console.WriteLine("No image was selected or picker was canceled");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in PickPhotoAsync: {ex}");
            await DisplayAlert("Error", $"Failed to pick image: {ex.Message}", "OK");
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
        OnAddImageClicked(sender, e);
    }

    private void OnDeleteImageClicked(object sender, EventArgs e)
    {
        SelectedImage.Source = null;
        _selectedImageFile = null;
        _imageBase64 = null;
        AddImageButton.IsVisible = true;
        ImageActionButtons.IsVisible = false;
    }

    private async void OnSubmitClicked(object sender, EventArgs e)
    {
        // Disable the button to prevent multiple submissions
        Button submitButton = sender as Button;
        if (submitButton != null)
        {
            submitButton.IsEnabled = false;
        }

        try
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

            if (!decimal.TryParse(PriceEntry.Text, out decimal price))
            {
                await DisplayAlert("Validation Error", "Please enter a valid price", "OK");
                return;
            }

            // Check if image is selected
            if (_selectedImageFile == null)
            {
                bool proceed = await DisplayAlert("No Image",
                    "You haven't added an image for this product. Do you want to continue without an image?",
                    "Continue", "Cancel");

                if (!proceed) return;
            }

            // Create product object
            var product = new Product
            {
                Name = ProductNameEntry.Text,
                Description = ProductDescriptionEditor.Text ?? "",
                Type = ProductTypePicker.SelectedItem?.ToString() ?? "",
                Price = price,
                ImagePath = await SaveImageAsync(),
                CreatedAt = DateTime.UtcNow
            };

            // Debug information
            Console.WriteLine($"Saving product: {product.Name}, {product.Type}, {product.Price:C}");

            // Explicitly create products table if it doesn't exist
            await _databaseService.CreateProductsTableAsync();

            // Save to database
            var result = await _databaseService.InsertProductAsync(product);
            Console.WriteLine($"Database insert result: {result}");

            if (result > 0)
            {
                await DisplayAlert("Success", "Product listed successfully!", "OK");

                // Clear the form
                ClearForm();

                // Use Device.BeginInvokeOnMainThread to ensure UI operations happen on main thread
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    try
                    {
                        // Try different navigation approaches based on your app structure
                        if (Navigation.NavigationStack.Count > 1)
                        {
                            await Navigation.PopAsync();
                        }
                        else
                        {
                            // If this is the root page, use Shell navigation if applicable
                            // Or navigate to another page
                            await Shell.Current.GoToAsync("..");
                        }
                    }
                    catch (Exception navEx)
                    {
                        Console.WriteLine($"Navigation error: {navEx.Message}");
                        // If navigation fails, just stay on the page
                    }
                });
            }
            else
            {
                await DisplayAlert("Error", "Failed to save product. Please try again.", "OK");
            }
        }
        catch (SQLiteException sqlEx)
        {
            // Handle specific database errors
            Console.WriteLine($"SQLite error: {sqlEx.Message}");
            await DisplayAlert("Database Error", $"Failed to save product: {sqlEx.Message}", "OK");
        }
        catch (Exception ex)
        {
            // Log the full exception details
            Console.WriteLine($"Exception during product submission: {ex}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            await DisplayAlert("Error", $"Failed to save product: {ex.Message}", "OK");
        }
        finally
        {
            // Re-enable the button
            if (submitButton != null)
            {
                submitButton.IsEnabled = true;
            }
        }
    }

    private void ClearForm()
    {
        // Clear all form fields
        ProductNameEntry.Text = string.Empty;
        ProductDescriptionEditor.Text = string.Empty;
        ProductTypePicker.SelectedIndex = -1;
        PriceEntry.Text = string.Empty;
        OnDeleteImageClicked(null, null); // Reset image state
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
            Console.WriteLine($"Failed to save image: {ex.Message}");
            await DisplayAlert("Error", $"Failed to save image: {ex.Message}", "OK");
            return null;
        }
    }
}