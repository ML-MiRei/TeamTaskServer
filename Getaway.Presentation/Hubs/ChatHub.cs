using Getaway.Application.CQRS.Messenger.Chat.Commands.AddUserInChat;
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
using Getaway.Application.CQRS.User.Queries.GetUserById;
using Getaway.Application.CQRS.User.Queries.GetUserByTag;
using Getaway.Application.ReturnsModels;
using Getaway.Core.Entities;
using Getaway.Core.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;

namespace Getaway.Presentation.Hubs
{




    public class ChatHub : Hub
    {
        private const string GROUP_CHAT_PREFIX = "chat_";
        private static IMediator _mediator;
        private static UserConnections _userConnections = new UserConnections();

        public ChatHub(IMediator mediator)
        {
            _mediator = mediator;
        }


        public async Task ConnectUserWithChats(int userId, string userTag)
        {
            var chats = await _mediator.Send(new GetChatsQuery { UserId = userId });
            foreach (var chat in chats)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, GROUP_CHAT_PREFIX + chat.ID);

            }

            _userConnections.ListUserConnections.Add(new UserConnection { ConnectionId = Context.ConnectionId, Id = userId, Tag = userTag });

            await Console.Out.WriteLineAsync(Context.ConnectionId);
            await Groups.AddToGroupAsync(Context.ConnectionId, "user_" + userId);

        }


        public override Task OnDisconnectedAsync(Exception? exception)
        {
            _userConnections.RemoveUserConnectionId(Context.ConnectionId);
            return Task.CompletedTask;
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


        public async Task CreatePrivateChat(int userId, string secondUserTag)
        {
            try
            {
                var chat = await _mediator.Send(new CreatePrivateChatCommand { UserId = userId, SecondUserTag = secondUserTag });

                await Groups.AddToGroupAsync(Context.ConnectionId, GROUP_CHAT_PREFIX + chat.ID);


                var chatModel = new ChatModel()
                {
                    ChatId = chat.ID,
                    Image = chat.Image,
                    ChatName = chat.ChatName,
                    Messages = _mediator.Send(new GetMessagesQuery { ChatId = chat.ID }).Result
                                            .Select(m => new MessageModel { MessageId = m.ID, TextMessage = m.TextMessage, UserNameCreator = m.UserNameCreator })
                                            .ToList(),
                    Users = _mediator.Send(new GetUsersByChatQuery { ChatId = chat.ID }).Result.Select(u => new UserModel
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


                try
                {
                    var n = await _mediator.Send(new CreateNotificationCommand
                    {
                        Details = "You have been added to a new chat",
                        Title = "Chats",
                        UserId = new List<int> {
                            (await _mediator.Send(new GetUserByTagQuery{ UserTag = secondUserTag})).ID
                        }
                    });

                    await Console.Out.WriteLineAsync("Notif created");


                    await Groups.AddToGroupAsync(Context.ConnectionId, GROUP_CHAT_PREFIX + chat.ID);

                    await Console.Out.WriteLineAsync("user_" + userId);
                    await Clients.Group("user_" + userId).SendAsync("NewChatReply", chatModel);



                    if (_userConnections.IsConnectedUser(secondUserTag))
                    {

                        var user = await _mediator.Send(new GetUserByIdQuery { UserId = userId });
                        chatModel.ChatName = user.FirstName;

                        await Console.Out.WriteLineAsync("user_" + _userConnections.GetUserId(secondUserTag));

                        await Groups.AddToGroupAsync(_userConnections.GetUserConnectionId(secondUserTag), GROUP_CHAT_PREFIX + chat.ID);
                        await Clients.Group("user_" + _userConnections.GetUserId(secondUserTag)).SendAsync("NewChatReply", chatModel);
                    }


                    await Console.Out.WriteLineAsync("real-time");

                }
                catch (Exception ex)
                {
                    await Console.Out.WriteLineAsync(ex.Message);
                }



            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }



        public async Task AddUserInChat(int chatId, string userTag)
        {
            try
            {
                await _mediator.Send(new AddUserInChatCommand { ChatId = chatId, UserTag = userTag });

                var user = await _mediator.Send(new GetUserByTagQuery { UserTag = userTag });

                try
                {
                    var newUser = await _mediator.Send(new GetUserByTagQuery { UserTag = userTag });


                    await _mediator.Send(new CreateNotificationCommand
                    {
                        Details = "You have been added to a new chat",
                        Title = "Chats",
                        UserId = new List<int> { newUser.ID }
                    });



                    await _mediator.Send(new CreateNotificationCommand
                    {
                        Details = "A new user has been added to the chat",
                        Title = "Chats",
                        UserId = (await _mediator.Send(new GetUsersByChatQuery { ChatId = chatId })).Where(u => u.ID != newUser.ID)
                            .Select(u => u.ID)
                            .ToList()

                    });


                    var chat = await _mediator.Send(new GetChatQuery { ChatId = chatId });



                    Task task1 = Clients.Group("user_" + newUser.ID).SendAsync("NewChatReply",
                         new ChatModel
                         {
                             ChatId = chat.ID,
                             Image = chat.Image,
                             ChatName = chat.ChatName,
                             Messages = _mediator.Send(new GetMessagesQuery { ChatId = chat.ID }).Result
                                         .Select(m => new MessageModel { MessageId = m.ID, TextMessage = m.TextMessage, UserNameCreator = m.UserNameCreator, CreatorTag = m.CreatorTag, DateCreated = m.DateCreated })
                                         .OrderBy(m => m.MessageId)
                                         .ToList(),
                             Users = _mediator.Send(new GetUsersByChatQuery { ChatId = chat.ID }).Result.Select(u => new UserModel
                             {
                                 Email = u.Email,
                                 LastName = u.LastName,
                                 FirstName = u.FirstName,
                                 SecondName = u.SecondName,
                                 PhoneNumber = u.PhoneNumber,
                                 UserTag = u.Tag,
                                 ColorNumber = 2

                             }).ToList(),
                             Type = chat.Type,
                             ColorNumber = 1,
                             UserRole = (int)UserRole.EMPLOYEE
                         }
                         );


                    Task task2 = Clients.Group(GROUP_CHAT_PREFIX + chat.ID).SendAsync("AddUserInChatReply",
                                      chatId,
                                      new UserModel
                                      {
                                          Email = user.Email,
                                          SecondName = user.SecondName,
                                          FirstName = user.FirstName,
                                          LastName = user.FirstName,
                                          PhoneNumber = user.PhoneNumber,
                                          UserTag = userTag,
                                          ColorNumber = 0
                                      }
                                      );


                    if(_userConnections.IsConnectedUser(userTag))
                        await Groups.AddToGroupAsync(_userConnections.GetUserConnectionId(newUser.ID), GROUP_CHAT_PREFIX + chat.ID);


                    Task.WaitAll(task1, task2);

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.InnerException.Message);
                }

            }
            catch (Exception)
            {
                throw new Exception();
            }
        }


        public async Task UpdateChat(ChatEntity chatEntity)
        {
            try
            {
                await _mediator.Send(new UpdateGroupChatCommand { ChatId = chatEntity.ID, Name = chatEntity.ChatName, AdminTag = chatEntity.AdminTag });

                try
                {

                    var users = (await _mediator.Send(new GetUsersByChatQuery { ChatId = chatEntity.ID }))
                                .Select(u => u.ID)
                                .ToList();

                    await _mediator.Send(new CreateNotificationCommand
                    {
                        Details = "One of the chats has been updated",
                        Title = "Chats",
                        UserId = users
                    });



                    await Clients.Group(GROUP_CHAT_PREFIX + chatEntity.ID).SendAsync("UpdateChatReply",
                           new ChatModel
                           {
                               ChatName = chatEntity.ChatName,
                               ChatId = chatEntity.ID
                           });
                }
                catch (Exception)
                {

                }

            }
            catch (Exception)
            {
                throw new Exception();
            }
        }


        public async Task DeleteUserFromChat(int chatId, string userTag)
        {
            try
            {
                await _mediator.Send(new DeleteUserFromChatCommand { ChatId = chatId, UserTag = userTag });

                var user = await _mediator.Send(new GetUserByTagQuery { UserTag = userTag });

                try
                {

                    var kickedUser = await _mediator.Send(new GetUserByTagQuery { UserTag = userTag });


                    await _mediator.Send(new CreateNotificationCommand
                    {
                        Details = "You were kicked out of the chat",
                        Title = "Chats",
                        UserId = new List<int> { kickedUser.ID }
                    });

                    await _mediator.Send(new CreateNotificationCommand
                    {
                        Details = $"User {kickedUser.FirstName} has left the chat",
                        Title = "Chats",
                        UserId =
                            (await _mediator.Send(new GetUsersByChatQuery { ChatId = chatId })).Where(u => u.ID != kickedUser.ID)
                            .Select(u => u.ID)
                            .ToList()

                    }); ;


                    var chat = await _mediator.Send(new GetChatQuery { ChatId = chatId });


                    Task task1 = Clients.Group("user_" + kickedUser.ID).SendAsync("DeleteChatReply", chatId);


                    if (_userConnections.IsConnectedUser(userTag))
                        await Groups.RemoveFromGroupAsync(_userConnections.GetUserConnectionId(kickedUser.ID), GROUP_CHAT_PREFIX + chat.ID);

                    Task task2 = Clients.Group(GROUP_CHAT_PREFIX + chatId).SendAsync("DeleteUserFromChatReply", chatId, userTag);


                    Task.WaitAll(task1, task2);




                }
                catch (Exception)
                {

                }

            }
            catch (Exception)
            {
                new Exception();
            }
        }



        public async Task LeaveChat(int chatId, int userId)
        {
            try
            {
                await _mediator.Send(new LeaveChatCommand { ChatId = chatId, UserId = userId });

                var user = await _mediator.Send(new GetUserByIdQuery { UserId = userId });


                try
                {

                    var kickedUser = await _mediator.Send(new GetUserByIdQuery { UserId = userId });


                    await _mediator.Send(new CreateNotificationCommand
                    {
                        Details = $"User {kickedUser.FirstName} has left the chat",
                        Title = "Chats",
                        UserId =
                            (await _mediator.Send(new GetUsersByChatQuery { ChatId = chatId })).Where(u => u.ID != kickedUser.ID)
                            .Select(u => u.ID)
                            .ToList()

                    });


                    var chat = await _mediator.Send(new GetChatQuery { ChatId = chatId });

                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, GROUP_CHAT_PREFIX + chat.ID);

                    await Clients.Group(GROUP_CHAT_PREFIX + chat.ID).SendAsync("DeleteUserFromChatReply", chatId, kickedUser.Tag);


                }
                catch (Exception)
                {

                }

            }
            catch (Exception)
            {
                throw new Exception();
            }
        }
    }
}
