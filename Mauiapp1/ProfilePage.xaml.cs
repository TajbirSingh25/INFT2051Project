using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using Mauiapp1.Services;
using Mauiapp1.Models;
using SQLite;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Media;
using System.IO;

namespace Mauiapp1
{
    public partial class ProfilePage : ContentPage
    {
        private readonly IDatabaseService _databaseService;
        private ObservableCollection<ProductViewModel> _userProducts;
        private FileResult _selectedProfileImageFile;
        private User _currentUser;

        private string _profileImagePath;

        public ProfilePage(IDatabaseService databaseService)
        {
            InitializeComponent();
            _databaseService = databaseService;
            _userProducts = new ObservableCollection<ProductViewModel>();
            ItemsCollectionView.ItemsSource = _userProducts;

            this.Appearing += async (s, e) => await LoadDataAsync();
        }

        private async void OnEditItemClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is int productId)
            {
                var product = await _databaseService.GetProductByIdAsync(productId);
                if (product != null)
                {
                    await Navigation.PushAsync(new ListItemPage(_databaseService, product));
                }
                else
                {
                    await DisplayAlert("Error", "Could not find the product to edit", "OK");
                }
            }
        }

        private async void OnDeleteItemClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is int productId)
            {
                bool confirm = await DisplayAlert("Confirm Delete",
                    "Are you sure you want to delete this product?",
                    "Delete", "Cancel");

                if (confirm)
                {
                    try
                    {
                        var productToDelete = await _databaseService.GetProductByIdAsync(productId);

                        if (productToDelete != null)
                        {
                            // Delete the product
                            var result = await _databaseService.DeleteProductAsync(productToDelete);

                            if (result > 0)
                            {
                                // Refresh the products list
                                await LoadUserProductsAsync();
                                await DisplayAlert("Success", "Product deleted successfully", "OK");
                            }
                            else
                            {
                                await DisplayAlert("Error", "Failed to delete the product", "OK");
                            }
                        }
                        else
                        {
                            await DisplayAlert("Error", "Product not found", "OK");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error deleting product: {ex.Message}");
                        await DisplayAlert("Error", "Failed to delete the product", "OK");
                    }
                }
            }
        }
        private async Task LoadDataAsync()
        {
            try
            {
                string currentUsername = await SecureStorage.GetAsync("current_user");

                if (string.IsNullOrEmpty(currentUsername))
                {
                    var users = await _databaseService.GetAllUsersAsync(); 
                    if (users != null && users.Count > 0)
                    {
                        _currentUser = users.FirstOrDefault();
                        currentUsername = _currentUser?.Username;
                    }
                    else
                    {
                        currentUsername = "Guest";
                    }
                }

                if (_currentUser == null && !string.IsNullOrEmpty(currentUsername))
                {
                    _currentUser = await _databaseService.GetUserAsync(currentUsername);
                }

                if (_currentUser != null)
                {
                    UsernameLabel.Text = _currentUser.Username;
                    string fullName = GetFullName(_currentUser);
                    FullNameLabel.Text = !string.IsNullOrWhiteSpace(fullName) ? fullName : "Member";
                    LoadProfileImage(currentUsername);
                }
                await LoadUserProductsAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading data: {ex.Message}");
            }
        }

        private string GetFullName(User user)
        {
            if (user == null) return string.Empty;

            if (!string.IsNullOrWhiteSpace(user.FirstName) && !string.IsNullOrWhiteSpace(user.LastName))
            {
                return $"{user.FirstName} {user.LastName}";
            }
            else if (!string.IsNullOrWhiteSpace(user.FirstName))
            {
                return user.FirstName;
            }
            else if (!string.IsNullOrWhiteSpace(user.LastName))
            {
                return user.LastName;
            }

            return string.Empty;
        }

        private void LoadProfileImage(string username)
        {
            try
            {
                string profileImagesDir = Path.Combine(FileSystem.AppDataDirectory, "ProfileImages");
                if (Directory.Exists(profileImagesDir))
                {
                    var files = Directory.GetFiles(profileImagesDir, $"profile_{username}*");
                    if (files.Length > 0)
                    {
                        var mostRecentFile = files.OrderByDescending(f => new FileInfo(f).CreationTime).First();
                        ProfileImage.Source = ImageSource.FromFile(mostRecentFile);
                        _profileImagePath = mostRecentFile;
                    }
                    else
                    {
                        string savedPath = Preferences.Get($"profile_image_{username}", null);
                        if (!string.IsNullOrEmpty(savedPath) && File.Exists(savedPath))
                        {
                            ProfileImage.Source = ImageSource.FromFile(savedPath);
                            _profileImagePath = savedPath;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading profile image: {ex.Message}");
               
            }
        }

        private async Task LoadUserProductsAsync()
        {
            try
            {
                _userProducts.Clear();

                // Get all products
                var products = await _databaseService.GetAllProductsAsync();
                if (products != null && products.Count > 0)
                {
                    foreach (var product in products)
                    {
                        _userProducts.Add(new ProductViewModel
                        {
                            Id = product.Id,
                            Name = product.Name,
                            Type = product.Type,
                            Price = product.Price,
                            Description = product.Description,
                            ImagePath = product.ImagePath,
                            ImageSource = GetImageSource(product.ImagePath)
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading products: {ex.Message}");
                await DisplayAlert("Error", "Failed to load products", "OK");
            }
        }

        private ImageSource GetImageSource(string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath) || !File.Exists(imagePath))
            {
                return "placeholder_product.png";
            }

            return ImageSource.FromFile(imagePath);
        }

        private async void BackButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MainPage(_databaseService));
        }

        private async void OnListItemClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ListItemPage(_databaseService));
        }

        private async void OnAddProfilePhotoClicked(object sender, EventArgs e)
        {
            var action = await DisplayActionSheet("Select Profile Photo", "Cancel", null,
                "Take Photo", "Choose from Gallery");

            switch (action)
            {
                case "Take Photo":
                    await TakeProfilePhotoAsync();
                    break;
                case "Choose from Gallery":
                    await PickProfilePhotoAsync();
                    break;
            }
        }

        private async Task TakeProfilePhotoAsync()
        {
            try
            {
                if (!MediaPicker.Default.IsCaptureSupported)
                {
                    await DisplayAlert("Error", "Camera not available", "OK");
                    return;
                }

#if ANDROID
                var status = await Permissions.RequestAsync<Permissions.Camera>();
                if (status != PermissionStatus.Granted)
                {
                    await DisplayAlert("Permission Required", "Camera access is required", "OK");
                    return;
                }
#endif

                var photo = await MediaPicker.Default.CapturePhotoAsync();
                if (photo != null)
                {
                    await SaveAndSetProfilePhotoAsync(photo);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to take photo: {ex.Message}", "OK");
            }
        }

        private async Task PickProfilePhotoAsync()
        {
            try
            {
                var photo = await MediaPicker.Default.PickPhotoAsync();
                if (photo != null)
                {
                    await SaveAndSetProfilePhotoAsync(photo);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to pick photo: {ex.Message}", "OK");
            }
        }

        private async Task SaveAndSetProfilePhotoAsync(FileResult photo)
        {
            try
            {
                _selectedProfileImageFile = photo;

                var stream = await photo.OpenReadAsync();
                ProfileImage.Source = ImageSource.FromStream(() => stream);

                // Save the image to app storage
                string imagePath = await SaveProfileImageAsync(photo);

                if (!string.IsNullOrEmpty(imagePath))
                {
                    _profileImagePath = imagePath;
                    if (_currentUser != null)
                    {
                        Preferences.Set($"profile_image_{_currentUser.Username}", imagePath);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving profile photo: {ex.Message}");
                await DisplayAlert("Error", "Failed to save profile photo", "OK");
            }
        }

        private async Task<string> SaveProfileImageAsync(FileResult photo)
        {
            if (photo == null) return null;

            try
            {
                string username = _currentUser?.Username ?? "unknown";
                var fileName = $"profile_{username}_{Guid.NewGuid()}{Path.GetExtension(photo.FileName)}";

                // Set destination path
                var destinationPath = Path.Combine(
                    FileSystem.AppDataDirectory,
                    "ProfileImages",
                    fileName
                );

                Directory.CreateDirectory(Path.GetDirectoryName(destinationPath));

                using var sourceStream = await photo.OpenReadAsync();
                using var destinationStream = File.Create(destinationPath);
                await sourceStream.CopyToAsync(destinationStream);

                return destinationPath;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to save image: {ex.Message}");
                return null;
            }
        }

        private async void OnItemSelected(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is ProductViewModel selectedProduct)
            {
                ((CollectionView)sender).SelectedItem = null;

                await DisplayAlert("Product Selected", $"You selected: {selectedProduct.Name}", "OK");

            }
        }
    }

    // ViewModel for displaying products
    public class ProductViewModel : INotifyPropertyChanged
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public decimal Price { get; set; }
        public string ImagePath { get; set; }
        public ImageSource ImageSource { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}