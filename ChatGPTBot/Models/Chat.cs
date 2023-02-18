using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatGPTBot.Models
{
    public class Chat
    {
        public Guid Id { get; set; }
        public long ChatId { get; set; }
        public string? Username { get; set; } = string.Empty;
        public string? FirstName { get; set; } = string.Empty;
        public string? LastName { get; set; } = string.Empty;
        public string? Bio { get; set; } = string.Empty;
        public string? BigPhotoId { get; set; } = string.Empty;
        public string? BigPhotoUrl { get; set; } = string.Empty;
        public DateTime CreatedOnUtc { get; set; }
        public DateTime UpdatedOnUtc { get; set; }
    }
}
