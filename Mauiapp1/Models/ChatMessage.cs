using System;
using SQLite;

namespace Mauiapp1.Models
{
    public class ChatMessage
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string SenderId { get; set; }
        public string SenderName { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }
        public bool IsFromCurrentUser { get; set; }
        public string AvatarUrl { get; set; }

        // For persistence
        public int ListingId { get; set; }
        public string ChatSessionId { get; set; }
    }
}