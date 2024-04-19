using Getaway.Application.CQRS.Messenger.Chat.Queries.GetChats;
using Getaway.Application.CQRS.Messenger.Message.Commands.CreateMessage;
using Getaway.Application.CQRS.Messenger.Message.Commands.DeleteMessage;
using Getaway.Application.CQRS.Messenger.Message.Commands.UpdateMessage;
using Getaway.Application.ReturnsModels;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.SignalR;

namespace Getaway.Presentation.Hubs
{
    public class ChatHub : Hub
    {
        private const string GROUP_CHAT_PREFIX = "chat_";


        private static IMediator _mediator;

        public ChatHub(IMediator mediator)
        {
            _mediator = mediator;
        }


        public async Task ConnectUserWithChats(int userId)
        {
            var chats = await _mediator.Send(new GetChatsQuery { UserId = userId });
            foreach (var chat in chats)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, GROUP_CHAT_PREFIX + chat.ID);
            }
        }


        public async Task SendMessage(int userId, int chatId, string textMessage)
        {
            try
            {

                var message = _mediator.Send(new CreateMessageCommand { ChatID = chatId, TextMessage = textMessage, UserId = userId }).Result;

                await Clients.Group(GROUP_CHAT_PREFIX + chatId).SendAsync("Receive", chatId, new MessageModel
                {
                    CreatorTag = message.CreatorTag,
                    MessageId = message.ID,
                    TextMessage = message.TextMessage,
                    UserNameCreator = message.UserNameCreator,
                    DateCreated = message.DateCreated
                });

                Console.WriteLine("Send message");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


        public async Task DeleteMessage(int chatId, int messageId)
        {
            try
            {

               await _mediator.Send(new DeleteMessageCommand { ChatID = chatId, MessageId = messageId });

                await Clients.Group(GROUP_CHAT_PREFIX + chatId).SendAsync("MessageIsDeleted", messageId);

                Console.WriteLine("Delete message");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


        public async Task UpdateMessage(int chatId, int messageId, string textMessage)
        {
            try
            {

                await _mediator.Send(new UpdateMessageCommand { ChatID = chatId, MessageId = messageId, TextMessage = textMessage });

                await Clients.Group(GROUP_CHAT_PREFIX + chatId).SendAsync("MessageIsUpdated", messageId);

                Console.WriteLine("Update message");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

    }
}
