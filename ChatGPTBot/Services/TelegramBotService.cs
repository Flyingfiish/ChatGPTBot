using OpenAI.GPT3.ObjectModels.RequestModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using ChatGPTBot.Interfaces;
using ChatGPTBot.Db;
using ChatGPTBot.Models;
using Microsoft.Extensions.Hosting;
using System.Threading;

namespace ChatGPTBot.Services
{
    public class TelegramBotService : BackgroundService
    {
        private readonly IMessageHandler _messageHandler;
        private readonly ApplicationContext _dbContext;
        private readonly string _token;
        private readonly TelegramBotClient _botClient;

        public TelegramBotService(IMessageHandler messageHandler, ApplicationContext dbContext, string token)
        {
            _messageHandler = messageHandler;
            _dbContext = dbContext;
            _token = token;
            _botClient = new TelegramBotClient(_token);


        }

        private void StartRecieving()
        {
            using CancellationTokenSource cts = new();
            ReceiverOptions receiverOptions = new()
            {
                AllowedUpdates = Array.Empty<UpdateType>() // receive all update types
            };

            _botClient.StartReceiving(
                updateHandler: HandleUpdateAsync,
                pollingErrorHandler: HandlePollingErrorAsync,
                receiverOptions: receiverOptions,
                cancellationToken: cts.Token
            );

            Console.WriteLine($"Start listening");
        }

        async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            //await _botClient.SendTextMessageAsync($"@creatio_rbr_dev_bot", "test");
            // Only process Message updates: https://core.telegram.org/bots/api#message
            if (update.Message is not { } message)
                return;
            // Only process text messages
            if (message.Text is not { } messageText)
                return;

            var chatId = message.Chat.Id;
            var recieved = DateTime.UtcNow;
            Console.WriteLine(messageText);

            var response = await _messageHandler.Handle(messageText);

            Console.WriteLine(response);

            var dbChatId = CreateOrUpdateChat(message.Chat);
            CreateMessageResponse(new MessageResponse()
            {
                ChatId = dbChatId,
                Message = messageText,
                Response = response,
                Responsed = DateTime.UtcNow,
                Recieved = recieved
            });


            Message sentMessage = await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: response,
                cancellationToken: cancellationToken);
        }

        void CreateMessageResponse(MessageResponse messageResponse)
        {
            _dbContext.MessageResponses.Add(messageResponse);
            _dbContext.SaveChanges();
        }

        Guid CreateOrUpdateChat(Telegram.Bot.Types.Chat telegramChat)
        {
            var chat = _dbContext.Chats.FirstOrDefault(chat => chat.ChatId == telegramChat.Id);
            if (chat is null)
            {
                chat = new Models.Chat()
                {
                    Id = Guid.NewGuid(),
                    ChatId = telegramChat.Id,
                    FirstName = telegramChat.FirstName,
                    LastName = telegramChat.LastName,
                    Username = telegramChat.Username,
                    Bio = telegramChat.Bio,
                    UpdatedOnUtc = DateTime.UtcNow,
                    CreatedOnUtc = DateTime.UtcNow,
                };
                if (telegramChat.Photo != null)
                {
                    chat.BigPhotoId = telegramChat.Photo.BigFileId;
                    chat.BigPhotoUrl = $"https://api.telegram.org/bot{_token}/getFile?file_id={telegramChat.Photo.BigFileId}";
                }
                _dbContext.Chats.Add(chat);
            }
            else
            {
                chat.ChatId = telegramChat.Id;
                chat.FirstName = telegramChat.FirstName;
                chat.LastName = telegramChat.LastName;
                chat.Username = telegramChat.Username;
                chat.Bio = telegramChat.Bio;
                if (telegramChat.Photo != null)
                {
                    chat.BigPhotoId = telegramChat.Photo.BigFileId;
                    chat.BigPhotoUrl = $"https://api.telegram.org/bot{_token}/getFile?file_id={telegramChat.Photo.BigFileId}";
                }
                chat.UpdatedOnUtc = DateTime.UtcNow;
                _dbContext.Chats.Update(chat);
            }
            _dbContext.SaveChanges();
            return chat.Id;
        }

        Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //using CancellationTokenSource cts = new();
            ReceiverOptions receiverOptions = new()
            {
                AllowedUpdates = Array.Empty<UpdateType>() // receive all update types
            };

            _botClient.StartReceiving(
                updateHandler: HandleUpdateAsync,
                pollingErrorHandler: HandlePollingErrorAsync,
                receiverOptions: receiverOptions,
                cancellationToken: stoppingToken
            );

            Console.WriteLine($"Start listening");

            while (!stoppingToken.IsCancellationRequested)
            {
                //_logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
