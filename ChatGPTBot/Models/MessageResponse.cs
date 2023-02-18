using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatGPTBot.Models
{
    public class MessageResponse
    {
        public Guid Id { get; set; }
        public string Message { get; set; }
        public string Response { get; set; }
        public DateTime Recieved { get; set; }
        public DateTime Responsed { get; set; }

        public Chat Chat { get; set; }
        public Guid ChatId { get; set; }
    }
}
