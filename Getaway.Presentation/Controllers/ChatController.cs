
using Getaway.Application.CQRS.Messenger.Chat.Commands.AddUserInChat;
using Getaway.Application.CQRS.Messenger.Chat.Commands.CreateGroupChat;
using Getaway.Application.CQRS.Messenger.Chat.Commands.CreatePrivateChat;
using Getaway.Application.CQRS.Messenger.Chat.Commands.DeleteUserFromChat;
using Getaway.Application.CQRS.Messenger.Chat.Commands.LeaveChat;
using Getaway.Application.CQRS.Messenger.Chat.Commands.UpdateGroupChat;
using Getaway.Application.CQRS.Messenger.Chat.Queries.GetChats;
using Getaway.Application.CQRS.Messenger.Message.Commands.CreateMessage;
using Getaway.Application.CQRS.Messenger.Message.Commands.DeleteMessage;
using Getaway.Application.CQRS.Messenger.Message.Commands.UpdateMessage;
using Getaway.Application.CQRS.Messenger.Message.Queries.GetMessages;
using Getaway.Application.CQRS.Team.Queries.GetUsersByChhat;
using Getaway.Application.CQRS.User.Queries.GetUserById;
using Getaway.Application.CQRS.User.Queries.GetUserByTag;
using Getaway.Application.ReturnsModels;
using Getaway.Core.Entities;
using Getaway.Core.Enums;
using Getaway.Infrustructure;
using Getaway.Presentation.Hubs;
using MediatR;
using Microsoft.AspNetCore.Mvc;


namespace Getaway.Presentation.Controllers
{
    [Route("{userId}/api/[controller]")]
    [ApiController]
    public class ChatController(IMediator mediator) : ControllerBase
    {
        NotificationHub notificationHub = new NotificationHub(mediator);

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
                        UserTag = u.Tag
                    }).ToList(),
                    Type = chat.Type

                });

                Console.WriteLine(result.Last().ChatId);
            }



            return Ok(result);
        }



        [HttpPost("private")]
        public async Task<ActionResult<ChatModel>> CreatePrivateChat(int userId, [FromBody] string secondUserTag)
        {
            try
            {
                var chat = await mediator.Send(new CreatePrivateChatCommand { UserId = userId, SecondUserTag = secondUserTag });

                var chatModel = new ChatModel()
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

                };

                await notificationHub.ChatNotification(
                     NotificationAction.CREATE,
                     mediator.Send(new GetUserByTagQuery { UserTag = secondUserTag }).Result.ID,
                     chatModel
                     );


                return Ok(chatModel);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new EmptyResult();
            }

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


        [HttpPost("{chatId}/send-message")]
        public async Task<ActionResult> CreateMessage(int chatId, int userId, [FromBody] string textMessage)
        {
            try
            {
                var message = await mediator.Send(new CreateMessageCommand { UserId = userId, ChatID = chatId, TextMessage = textMessage });

                var result = (new MessageModel()
                {
                    MessageId = message.ID,
                    TextMessage = message.TextMessage,
                    UserNameCreator = message.UserNameCreator,
                    CreatorTag = message.CreatorTag,
                    DateCreated = DateTime.Now

                });

                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new EmptyResult();
            }
        }

        [HttpDelete("{chatId}/delete-message")]
        public async Task<ActionResult> DeleteMessage(int chatId, [FromBody] int messageId)
        {
            try
            {
                Console.WriteLine("start delete");
                await mediator.Send(new DeleteMessageCommand { ChatID = chatId, MessageId = messageId });

                return Ok();
            }
            catch (Exception)
            {
                return new EmptyResult();
            }
        }


        [HttpDelete("{chatId}/add-user")]
        public async Task<ActionResult> AddUserInChat(int chatId, [FromBody] string userTag)
        {
            try
            {
                await mediator.Send(new AddUserInChatCommand { ChatId = chatId, UserTag = userTag });

                var user = await mediator.Send(new GetUserByTagQuery { UserTag = userTag });



                await notificationHub.MembersChatNotification(
                    NotificationAction.ADD_USER,
                    chatId,
                    new UserModel
                    {
                        Email = user.Email,
                        SecondName = user.SecondName,
                        FirstName = user.FirstName,
                        LastName = user.FirstName,
                        PhoneNumber = user.PhoneNumber,
                        UserTag = userTag
                    }
                    );



                return Ok();
            }
            catch (Exception)
            {
                return new EmptyResult();
            }
        }




        [HttpPatch("{chatId}/update-message/{messageId}")]
        public async Task<ActionResult> UpdateMessage(int chatId, int messageId, [FromBody] string textMessage)
        {
            try
            {
                await mediator.Send(new UpdateMessageCommand { ChatID = chatId, MessageId = messageId, TextMessage = textMessage });

                return Ok();
            }
            catch (Exception)
            {
                return new EmptyResult();
            }
        }

        [HttpPatch("{chatId}/update")]
        public async Task<ActionResult> UpdateChat(int chatId, [FromBody] ChatEntity chatEntity)
        {
            try
            {
                await mediator.Send(new UpdateGroupChatCommand { ChatId = chatId, Name = chatEntity.ChatName, AdminTag = chatEntity.AdminTag });

                return Ok();
            }
            catch (Exception)
            {
                return new EmptyResult();
            }
        }



        [HttpDelete("{chatId}/delete-user/{userTag}")]
        public async Task<ActionResult> DeleteUserFromChat(int chatId, string userTag)
        {
            try
            {
                await mediator.Send(new DeleteUserFromChatCommand { ChatId = chatId, UserTag = userTag });

                var user = await mediator.Send(new GetUserByTagQuery { UserTag = userTag });



                await notificationHub.MembersChatNotification(
                    NotificationAction.DELETE_USER,
                    chatId,
                    new UserModel
                    {
                        Email = user.Email,
                        SecondName = user.SecondName,
                        FirstName = user.FirstName,
                        LastName = user.FirstName,
                        PhoneNumber = user.PhoneNumber,
                        UserTag = userTag
                    }
                    );

                return Ok();
            }
            catch (Exception)
            {
                return new EmptyResult();
            }
        }


        [HttpDelete("{chatId}/leave")]
        public async Task<ActionResult> LeaveChat(int chatId, int userId)
        {
            try
            {
                await mediator.Send(new LeaveChatCommand { ChatId = chatId, UserId = userId });

                var user = await mediator.Send(new GetUserByIdQuery { UserId = userId });



                await notificationHub.MembersChatNotification(
                    NotificationAction.DELETE_USER,
                    chatId,
                    new UserModel
                    {
                        Email = user.Email,
                        SecondName = user.SecondName,
                        FirstName = user.FirstName,
                        LastName = user.FirstName,
                        PhoneNumber = user.PhoneNumber,
                        UserTag = user.Tag
                    }
                    );

                return Ok();
            }
            catch (Exception)
            {
                return new EmptyResult();
            }
        }
    }
}
