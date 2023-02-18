using ChatGPTBot.Interfaces;
using OpenAI.GPT3.Managers;
using OpenAI.GPT3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenAI.GPT3;
using OpenAI.GPT3.Managers;
using OpenAI.GPT3.ObjectModels;
using OpenAI.GPT3.ObjectModels.RequestModels;

namespace ChatGPTBot.Services
{
    public class ChatGPTService : IMessageHandler
    {
        private readonly string _token;
        private readonly OpenAIService _gpt3;
        public ChatGPTService(string token)
        {
            _token = token;
            _gpt3 = new OpenAIService(new OpenAiOptions()
            {
                ApiKey = _token
            });
        }

        public async Task<string> Handle(string message)
        {
            var modelName = OpenAI.GPT3.ObjectModels.Models.TextDavinciV3;
            var completionResult = await _gpt3.Completions.CreateCompletion(new CompletionCreateRequest()
            {
                Prompt = message,
                Model = OpenAI.GPT3.ObjectModels.Models.TextDavinciV3,
                Temperature = 0.9F,
                MaxTokens = 2048,
                N = 1
            });

            if (completionResult.Successful)
            {
                var reply = $"{string.Join(".\n", completionResult.Choices.Select(completion => completion.Text)).Trim()}";
                return reply;
            }
            else
            {
                return "/Error";
            }
        }
    }
}
