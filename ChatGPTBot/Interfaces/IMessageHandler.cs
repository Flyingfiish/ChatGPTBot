using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatGPTBot.Interfaces
{
    public interface IMessageHandler
    {
        Task<string> Handle(string message);
    }
}
