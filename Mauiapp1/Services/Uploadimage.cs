using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UploadImageApp.Model;
using Microsoft.Maui.Storage;

namespace UploadImageApp.Services
{
    public class UploadImage
    {
        private readonly Page _page;

        public UploadImage(Page page)
        {
            _page = page;
        }

        public async Task<FileResult> OpenMediaPickerAsync()
        {
            try
            {
                // Configure FilePicker with more comprehensive options
                var customFileType = new FilePickerFileType(
                    new Dictionary<DevicePlatform, IEnumerable<string>>
                    {
                        { DevicePlatform.iOS, new[] { "public.image" } },
                        { DevicePlatform.Android, new[] { "image/*" } },
                        { DevicePlatform.WinUI, new[] { ".jpg", ".jpeg", ".png" } },
                        { DevicePlatform.macOS, new[] { "jpg", "jpeg", "png" } }
                    });

                var options = new PickOptions
                {
                    PickerTitle = "Please select a photo",
                    FileTypes = customFileType,
                };

                // This is critical for Windows - ensure permissions are set correctly
#if WINDOWS
                await Permissions.RequestAsync<Permissions.StorageRead>();
                await Permissions.RequestAsync<Permissions.StorageWrite>();
#endif

                // Use FilePicker with proper options
                var result = await FilePicker.Default.PickAsync(options);

                if (result != null)
                {
                    Console.WriteLine($"File picked: {result.FileName}, Type: {result.ContentType}");

                    // More permissive file type checking
                    if (result.ContentType != null &&
                        (result.ContentType.StartsWith("image/") ||
                         result.FileName.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                         result.FileName.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                         result.FileName.EndsWith(".png", StringComparison.OrdinalIgnoreCase)))
                    {
                        return result;
                    }
                    else
                    {
                        await _page.DisplayAlert("Error Type Image",
                            $"Please choose a supported image format (PNG, JPEG, JPG). Selected: {result.ContentType}",
                            "Ok");
                    }
                }
                else
                {
                    Console.WriteLine("No file was picked or dialog was canceled");
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"File picker error: {ex.Message}");
                await _page.DisplayAlert("Error", $"Failed to pick image: {ex.Message}", "OK");
                return null;
            }
        }

        public async Task<Stream> FileResultToStream(FileResult fileResult)
        {
            if (fileResult == null)
                return null;

            try
            {
                return await fileResult.OpenReadAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error opening file stream: {ex.Message}");
                await _page.DisplayAlert("Error", $"Failed to open file: {ex.Message}", "OK");
                return null;
            }
        }

        public Stream ByteArrayToStream(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
                return null;

            return new MemoryStream(bytes);
        }

        public string ByteBase64ToString(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
                return string.Empty;

            return Convert.ToBase64String(bytes);
        }

        public byte[] StringToByteBase64(string text)
        {
            if (string.IsNullOrEmpty(text))
                return Array.Empty<byte>();

            return Convert.FromBase64String(text);
        }

        public async Task<ImageFile> Upload(FileResult fileResult)
        {
            if (fileResult == null)
            {
                Console.WriteLine("Upload called with null fileResult");
                return null;
            }

            byte[] bytes;

            try
            {
                Console.WriteLine($"Beginning upload of file: {fileResult.FileName}");

                using (var ms = new MemoryStream())
                {
                    var stream = await FileResultToStream(fileResult);
                    if (stream == null)
                    {
                        Console.WriteLine("Failed to get stream from file result");
                        return null;
                    }

                    await stream.CopyToAsync(ms);
                    bytes = ms.ToArray();

                    Console.WriteLine($"Successfully read {bytes.Length} bytes from file");
                }

                if (bytes.Length == 0)
                {
                    Console.WriteLine("Warning: File has zero bytes");
                    await _page.DisplayAlert("Warning", "The selected file appears to be empty", "OK");
                }

                var result = new ImageFile
                {
                    byteBase64 = ByteBase64ToString(bytes),
                    ContentType = fileResult.ContentType ?? "image/unknown",
                    FileName = fileResult.FileName
                };

                Console.WriteLine("Image file object created successfully");
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Upload error: {ex.Message}");
                await _page.DisplayAlert("Error", $"Failed to process image: {ex.Message}", "OK");
                return null;
            }
        }
    }
}