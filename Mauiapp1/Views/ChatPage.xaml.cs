using System;
using System.Collections.ObjectModel;
using Mauiapp1.Models;
using Mauiapp1.Services;

namespace Mauiapp1.Views
{
    public partial class ChatPage : ContentPage
    {
        private readonly Mauiapp1.Models.Listing _listingItem;
        private readonly string _currentUserId = "currentUser"; // In a real app, this would come from authentication
        private Random _random = new Random();
        private bool _isTypingResponseSimulated = false;
        private IntelligentChatbotService _chatbotService;
        private readonly ChatDatabaseService _databaseService = ChatDatabaseService.Instance;

        public ObservableCollection<ChatMessage> Messages { get; private set; }
        public string SellerName { get; set; }
        public string SellerAvatarUrl { get; set; }
        public string ItemTitle { get; set; }

        public ChatPage(Mauiapp1.Models.Listing listingItem)
        {
            InitializeComponent();

            _listingItem = listingItem;
            SellerName = listingItem.SellerName ?? "Seller";
            ItemTitle = listingItem.Title ?? "Item";
            SellerAvatarUrl = "profile_placeholder.png"; // Replace with actual seller avatar if available

            // Initialize the chatbot service
            _chatbotService = new IntelligentChatbotService(listingItem);

            // Initialize with empty collection in case loading fails
            Messages = new ObservableCollection<ChatMessage>();

            // Set binding context early to avoid null reference
            this.BindingContext = this;

            // Set up the chat
            InitializeChatAsync();

            // Add Clear Chat option to toolbar
            ToolbarItems.Add(new ToolbarItem
            {
                Text = "Clear Chat",
                Order = ToolbarItemOrder.Secondary,
                Command = new Command(async () => await ClearChatHistoryAsync())
            });
        }

        private async void InitializeChatAsync()
        {
            // Show loading indicator
            LoadingIndicator.IsVisible = true;
            ChatContainer.IsVisible = false;

            try
            {
                // Load existing messages
                var loadedMessages = await _databaseService.GetMessagesForListingAsync(_listingItem.Id, _currentUserId);

                if (loadedMessages != null && loadedMessages.Count > 0)
                {
                    Messages = loadedMessages;
                }
                else
                {
                    // If no messages found, add a welcome message
                    var welcomeMessage = new ChatMessage
                    {
                        SenderId = "seller",
                        SenderName = SellerName,
                        Message = $"Hi! Thanks for your interest in {ItemTitle}. How can I help you?",
                        Timestamp = DateTime.Now,
                        IsFromCurrentUser = false,
                        AvatarUrl = SellerAvatarUrl,
                        ListingId = _listingItem.Id,
                        ChatSessionId = $"{_currentUserId}_{_listingItem.Id}"
                    };

                    Messages.Add(welcomeMessage);
                    await _databaseService.SaveMessageAsync(welcomeMessage);
                }

                // Update binding context
                this.BindingContext = this;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"There was a problem loading the chat: {ex.Message}", "OK");

                // If loading fails, ensure we have a welcome message
                if (Messages.Count == 0)
                {
                    var welcomeMessage = new ChatMessage
                    {
                        SenderId = "seller",
                        SenderName = SellerName,
                        Message = $"Hi! Thanks for your interest in {ItemTitle}. How can I help you?",
                        Timestamp = DateTime.Now,
                        IsFromCurrentUser = false,
                        AvatarUrl = SellerAvatarUrl
                    };

                    Messages.Add(welcomeMessage);
                }
            }
            finally
            {
                // Hide loading indicator
                LoadingIndicator.IsVisible = false;
                ChatContainer.IsVisible = true;

                // Scroll to the bottom when the page appears
                this.Appearing += (sender, e) => {
                    if (Messages.Count > 0)
                    {
                        MessagesCollection.ScrollTo(Messages.Count - 1, animate: false);
                    }
                };
            }
        }

        private void OnMessageTextChanged(object sender, TextChangedEventArgs e)
        {
            // Enable/disable the send button based on whether there's text
            SendButton.IsEnabled = !string.IsNullOrWhiteSpace(e.NewTextValue);
        }

        private async void OnSendClicked(object sender, EventArgs e)
        {
            string messageText = MessageEntry.Text?.Trim();

            if (string.IsNullOrWhiteSpace(messageText))
                return;

            try
            {
                // Add the user's message
                var userMessage = new ChatMessage
                {
                    SenderId = _currentUserId,
                    SenderName = "Me",
                    Message = messageText,
                    Timestamp = DateTime.Now,
                    IsFromCurrentUser = true,
                    AvatarUrl = "profile.png",
                    ListingId = _listingItem.Id,
                    ChatSessionId = $"{_currentUserId}_{_listingItem.Id}"
                };

                Messages.Add(userMessage);

                // Save the user message to database
                await _databaseService.SaveMessageAsync(userMessage);

                // Clear the input field
                MessageEntry.Text = string.Empty;

                // Scroll to the bottom to show the latest message
                await Task.Delay(100);
                MessagesCollection.ScrollTo(Messages.Count - 1);

                // Simulate the seller typing and responding
                await SimulateSellerResponseAsync(messageText);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to send message: {ex.Message}", "OK");
            }
        }

        private async Task SimulateSellerResponseAsync(string userMessage)
        {
            if (_isTypingResponseSimulated)
                return;

            _isTypingResponseSimulated = true;

            try
            {
                // Show "typing" indicator
                var typingMessage = new ChatMessage
                {
                    SenderId = "system",
                    SenderName = "System",
                    Message = $"{SellerName} is typing...",
                    Timestamp = DateTime.Now,
                    IsFromCurrentUser = false,
                    AvatarUrl = SellerAvatarUrl
                };

                Messages.Add(typingMessage);
                await Task.Delay(100);
                MessagesCollection.ScrollTo(Messages.Count - 1);

                // Wait a random time (1-3 seconds) to simulate typing
                await Task.Delay(_random.Next(1000, 3000));

                // Remove the typing indicator
                Messages.Remove(typingMessage);

                // Get an intelligent response
                string response = _chatbotService.GetResponse(userMessage);

                // Add the seller's response
                var sellerMessage = new ChatMessage
                {
                    SenderId = "seller",
                    SenderName = SellerName,
                    Message = response,
                    Timestamp = DateTime.Now,
                    IsFromCurrentUser = false,
                    AvatarUrl = SellerAvatarUrl,
                    ListingId = _listingItem.Id,
                    ChatSessionId = $"{_currentUserId}_{_listingItem.Id}"
                };

                Messages.Add(sellerMessage);

                // Save the response to database
                await _databaseService.SaveMessageAsync(sellerMessage);

                // Scroll to show the latest message
                await Task.Delay(100);
                MessagesCollection.ScrollTo(Messages.Count - 1);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to get response: {ex.Message}", "OK");
            }
            finally
            {
                _isTypingResponseSimulated = false;
            }
        }

        private async Task ClearChatHistoryAsync()
        {
            bool confirm = await DisplayAlert("Clear Chat", "Are you sure you want to clear all chat history?", "Yes", "No");

            if (confirm)
            {
                try
                {
                    // Clear from database
                    await _databaseService.ClearChatHistoryAsync(_listingItem.Id, _currentUserId);

                    // Clear from collection
                    Messages.Clear();

                    // Add welcome message back
                    var welcomeMessage = new ChatMessage
                    {
                        SenderId = "seller",
                        SenderName = SellerName,
                        Message = $"Hi! Thanks for your interest in {ItemTitle}. How can I help you?",
                        Timestamp = DateTime.Now,
                        IsFromCurrentUser = false,
                        AvatarUrl = SellerAvatarUrl,
                        ListingId = _listingItem.Id,
                        ChatSessionId = $"{_currentUserId}_{_listingItem.Id}"
                    };

                    Messages.Add(welcomeMessage);
                    await _databaseService.SaveMessageAsync(welcomeMessage);
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", $"There was a problem clearing the chat: {ex.Message}", "OK");
                }
            }
        }
    }
}