
using Getaway.Application.CQRS.Messenger.Chat.Commands.AddUserInChat;
using Getaway.Application.CQRS.Messenger.Chat.Commands.CreateGroupChat;
using Getaway.Application.CQRS.Messenger.Chat.Commands.CreatePrivateChat;
using Getaway.Application.CQRS.Messenger.Chat.Commands.DeleteUserFromChat;
using Getaway.Application.CQRS.Messenger.Chat.Commands.LeaveChat;
using Getaway.Application.CQRS.Messenger.Chat.Commands.UpdateGroupChat;
using Getaway.Application.CQRS.Messenger.Chat.Queries.GetChat;
using Getaway.Application.CQRS.Messenger.Chat.Queries.GetChats;
using Getaway.Application.CQRS.Messenger.Message.Commands.CreateMessage;
using Getaway.Application.CQRS.Messenger.Message.Commands.DeleteMessage;
using Getaway.Application.CQRS.Messenger.Message.Commands.UpdateMessage;
using Getaway.Application.CQRS.Messenger.Message.Queries.GetMessages;
using Getaway.Application.CQRS.Notification.Commands.CreateNotification;
using Getaway.Application.CQRS.Team.Queries.GetUsersByChhat;
using Getaway.Application.CQRS.Team.Queries.GetUsersByTeam;
using Getaway.Application.CQRS.User.Queries.GetUserById;
using Getaway.Application.CQRS.User.Queries.GetUserByTag;
using Getaway.Application.ReturnsModels;
using Getaway.Core.Entities;
using Getaway.Core.Enums;
using Getaway.Infrustructure;
using Getaway.Presentation.Hubs;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR.Client;


namespace Getaway.Presentation.Controllers
{
    [Route("{userId}/api/[controller]")]
    [ApiController]
    public class ChatController(IMediator mediator) : ControllerBase
    {

        Random random = new Random();


        [HttpGet("list")]
        public async Task<ActionResult> GetChatsList(int userId)
        {


            var chats = await mediator.Send(new GetChatsQuery { UserId = userId });
            List<ChatModel> result = new List<ChatModel>();

            foreach (var chat in chats)
            {
                result.Add(new ChatModel()
                {
                    ChatId = chat.ID,
                    Image = chat.Image,
                    ChatName = chat.ChatName,
                    Messages = mediator.Send(new GetMessagesQuery { ChatId = chat.ID }).Result
                                        .Select(m => new MessageModel { MessageId = m.ID, TextMessage = m.TextMessage, UserNameCreator = m.UserNameCreator, CreatorTag = m.CreatorTag, DateCreated = m.DateCreated })
                                        .OrderBy(m => m.MessageId)
                                        .ToList(),
                    Users = mediator.Send(new GetUsersByChatQuery { ChatId = chat.ID }).Result.Select(u => new UserModel
                    {
                        Email = u.Email,
                        LastName = u.LastName,
                        FirstName = u.FirstName,
                        SecondName = u.SecondName,
                        PhoneNumber = u.PhoneNumber,
                        UserTag = u.Tag,
                        ColorNumber = random.Next(5)

                    }).ToList(),
                    Type = chat.Type,
                    ColorNumber = random.Next(5),
                    UserRole = chat.Type == (int)ChatType.GROUP && chat.AdminId == userId ? (int)UserRole.LEAD : (int)UserRole.EMPLOYEE

                });

                Console.WriteLine(result.Last().ChatId);
            }



            return Ok(result);
        }



        [HttpPost("group")]
        public async Task<ActionResult<ChatModel>> CreateGroupChat(int userId, [FromBody] string name)
        {
            try
            {
                var chat = await mediator.Send(new CreateGroupChatCommand { UserId = userId, Name = name });



                var result = (new ChatModel()
                {
                    ChatId = chat.ID,
                    Image = chat.Image,
                    ChatName = chat.ChatName,
                    Messages = mediator.Send(new GetMessagesQuery { ChatId = chat.ID }).Result
                                            .Select(m => new MessageModel { MessageId = m.ID, TextMessage = m.TextMessage, UserNameCreator = m.UserNameCreator })
                                            .ToList(),
                    Users = mediator.Send(new GetUsersByChatQuery { ChatId = chat.ID }).Result.Select(u => new UserModel
                    {
                        Email = u.Email,
                        LastName = u.LastName,
                        FirstName = u.FirstName,
                        SecondName = u.SecondName,
                        PhoneNumber = u.PhoneNumber,
                        UserTag = u.Tag
                    }).ToList(),
                    Type = chat.Type

                });

                return Ok(result);
            }
            catch (Exception)
            {
                return new EmptyResult();
            }
        }

    }
}
