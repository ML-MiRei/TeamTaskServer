using Getaway.Application.CQRS.Messenger.Chat.Commands.AddUserInChat;
using Getaway.Application.CQRS.Messenger.Chat.Commands.CreateGroupChat;
using Getaway.Application.CQRS.Messenger.Chat.Commands.CreateGroupChatWithUsers;
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
using Getaway.Application.CQRS.Project.Queries.GetProject;
using Getaway.Application.CQRS.Project.Queries.GetUsersByProject;
using Getaway.Application.CQRS.Team.Queries.GetTeam;
using Getaway.Application.CQRS.Team.Queries.GetUsersByChhat;
using Getaway.Application.CQRS.Team.Queries.GetUsersByTeam;
using Getaway.Application.CQRS.User.Queries.GetUserById;
using Getaway.Application.CQRS.User.Queries.GetUserByTag;
using Getaway.Application.ReturnsModels;
using Getaway.Core.Entities;
using Getaway.Core.Enums;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace Getaway.Presentation.Hubs
{

    public class ChatHub : Hub
    {
        private const string GROUP_CHAT_PREFIX = "chat_";
        private const string USER_PREFIX = "user_";
        private const string NOTIFICATION_CHAT_PREFIX = "Chats";

        private static IMediator _mediator;
        private static ILogger<ChatHub> _logger;

        private static UserConnections _userConnections = new UserConnections();

        public ChatHub(ILogger<ChatHub> logger, IMediator mediator)
        {
            _mediator = mediator;
            _logger = logger;
        }


        public async Task ConnectUserWithChats(int userId, string userTag)
        {
            var chats = await _mediator.Send(new GetChatsQuery { UserId = userId });
            foreach (var chat in chats)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, GROUP_CHAT_PREFIX + chat.ID);

            }

            _userConnections.ListUserConnections.Add(new UserConnection { ConnectionId = Context.ConnectionId, Id = userId, Tag = userTag });
            await Groups.AddToGroupAsync(Context.ConnectionId, USER_PREFIX + userId);

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

                _logger.LogInformation("Send message");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }
        public async Task DeleteMessage(int chatId, int messageId)
        {
            try
            {

                await _mediator.Send(new DeleteMessageCommand { ChatID = chatId, MessageId = messageId });

                await Clients.Group(GROUP_CHAT_PREFIX + chatId).SendAsync("MessageIsDeleted", messageId);

                _logger.LogInformation("Delete message");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }
        public async Task UpdateMessage(int chatId, int messageId, string textMessage)
        {
            try
            {
                await _mediator.Send(new UpdateMessageCommand { ChatID = chatId, MessageId = messageId, TextMessage = textMessage });

                await Clients.Group(GROUP_CHAT_PREFIX + chatId).SendAsync("MessageUpdated", new MessageEntity { ChatId = chatId, ID = messageId, TextMessage = textMessage });

                _logger.LogInformation("Update message");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }



        public async Task CreateGroupChatByProject(int userId, int projectId)
        {
            try
            {
                var project = await _mediator.Send(new GetProjectQuery { ProjectId = projectId });
                var users = await _mediator.Send(new GetUsersByProjectQuery { ProjectId = projectId });
                var chat = await _mediator.Send(new CreateGroupChatWithUsersCommand { UsersId = users.Select(u => u.ID).ToArray(), AdminId = userId, Name = project.ProjectName });

                var notificationForAll = await _mediator.Send(new CreateNotificationCommand
                {
                    Details = "You have been added to a new chat",
                    Title = NOTIFICATION_CHAT_PREFIX,
                    UserId = users.Where(u => u.ID != userId).Select(u => u.ID).ToList()
                });

                var result = (new ChatModel()
                {
                    ChatId = chat.ID,
                    Image = chat.Image,
                    ChatName = chat.ChatName,
                    Messages = (await _mediator.Send(new GetMessagesQuery { ChatId = chat.ID }))
                                            .Select(m => new MessageModel { MessageId = m.ID, TextMessage = m.TextMessage, UserNameCreator = m.UserNameCreator })
                                            .ToList(),
                    Users = (await _mediator.Send(new GetUsersByChatQuery { ChatId = chat.ID })).Select(u => new UserModel
                    {
                        Email = u.Email,
                        LastName = u.LastName,
                        FirstName = u.FirstName,
                        SecondName = u.SecondName,
                        PhoneNumber = u.PhoneNumber,
                        UserTag = u.Tag
                    }).ToList(),
                    Type = chat.Type,
                    UserRole = (int)UserRole.LEAD,
                    ColorNumber = 3
                });

                foreach (var user in users)
                {
                    if (_userConnections.IsConnectedUser(user.ID))
                    {
                        await Groups.AddToGroupAsync(_userConnections.GetUserConnectionId(user.ID), GROUP_CHAT_PREFIX + chat.ID);
                    }
                }

                await Clients.OthersInGroup(GROUP_CHAT_PREFIX + chat.ID).SendAsync("NotificationReply", new NotificationModel
                {
                    Details = notificationForAll.Detail,
                    Title = notificationForAll.Title,
                    NotificationId = notificationForAll.ID
                });
                await Clients.Group(GROUP_CHAT_PREFIX + chat.ID).SendAsync("NewGroupChatReply", result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }
        public async Task CreateGroupChatByTeam(int userId, int teamId)
        {
            try
            {
                var team = await _mediator.Send(new GetTeamQuery { TeamId = teamId });
                var users = await _mediator.Send(new GetUsersByTeamQuery { TeamId = teamId });
                var chat = await _mediator.Send(new CreateGroupChatWithUsersCommand { UsersId = users.Select(u => u.ID).ToArray(), AdminId = userId, Name = team.Name });

                var notificationForAll = await _mediator.Send(new CreateNotificationCommand
                {
                    Details = "You have been added to a new chat",
                    Title = NOTIFICATION_CHAT_PREFIX,
                    UserId = users.Where(u => u.ID != userId).Select(u => u.ID).ToList()
                });

                var result = (new ChatModel()
                {
                    ChatId = chat.ID,
                    Image = chat.Image,
                    ChatName = chat.ChatName,
                    Messages = (await _mediator.Send(new GetMessagesQuery { ChatId = chat.ID }))
                                            .Select(m => new MessageModel { MessageId = m.ID, TextMessage = m.TextMessage, UserNameCreator = m.UserNameCreator })
                                            .ToList(),
                    Users = (await _mediator.Send(new GetUsersByChatQuery { ChatId = chat.ID })).Select(u => new UserModel
                    {
                        Email = u.Email,
                        LastName = u.LastName,
                        FirstName = u.FirstName,
                        SecondName = u.SecondName,
                        PhoneNumber = u.PhoneNumber,
                        UserTag = u.Tag
                    }).ToList(),
                    Type = chat.Type,
                    UserRole = (int)UserRole.LEAD,
                    ColorNumber = 3
                });

                foreach (var user in users)
                {
                    if (_userConnections.IsConnectedUser(user.ID))
                    {
                        await Groups.AddToGroupAsync(_userConnections.GetUserConnectionId(user.ID), GROUP_CHAT_PREFIX + chat.ID);
                    }
                }

                await Clients.OthersInGroup(GROUP_CHAT_PREFIX + chat.ID).SendAsync("NotificationReply", new NotificationModel
                {
                    Details = notificationForAll.Detail,
                    Title = notificationForAll.Title,
                    NotificationId = notificationForAll.ID
                });
                await Clients.Group(GROUP_CHAT_PREFIX + chat.ID).SendAsync("NewGroupChatReply", result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }
        public async Task CreateGroupChat(int userId, string name)
        {
            try
            {
                var chat = await _mediator.Send(new CreateGroupChatCommand { UserId = userId, Name = name });

                await Groups.AddToGroupAsync(Context.ConnectionId, GROUP_CHAT_PREFIX + chat.ID);

                var result = (new ChatModel()
                {
                    ChatId = chat.ID,
                    Image = chat.Image,
                    ChatName = chat.ChatName,
                    Messages = (await _mediator.Send(new GetMessagesQuery { ChatId = chat.ID }))
                                            .Select(m => new MessageModel { MessageId = m.ID, TextMessage = m.TextMessage, UserNameCreator = m.UserNameCreator })
                                            .ToList(),
                    Users = (await _mediator.Send(new GetUsersByChatQuery { ChatId = chat.ID })).Select(u => new UserModel
                    {
                        Email = u.Email,
                        LastName = u.LastName,
                        FirstName = u.FirstName,
                        SecondName = u.SecondName,
                        PhoneNumber = u.PhoneNumber,
                        UserTag = u.Tag
                    }).ToList(),
                    Type = chat.Type,
                    UserRole = (int)UserRole.LEAD,
                    ColorNumber = 3
                });

                await Clients.Group(USER_PREFIX + userId).SendAsync("NewGroupChatReply", result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
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
                    Type = chat.Type,
                    UserRole = (int)UserRole.EMPLOYEE,
                    ColorNumber = 2


                };

                var n = await _mediator.Send(new CreateNotificationCommand
                {
                    Details = "You have been added to a new chat",
                    Title = NOTIFICATION_CHAT_PREFIX,
                    UserId = new List<int> {
                            (await _mediator.Send(new GetUserByTagQuery{ UserTag = secondUserTag})).ID
                    }
                });


                await Groups.AddToGroupAsync(Context.ConnectionId, GROUP_CHAT_PREFIX + chat.ID);


                if (_userConnections.IsConnectedUser(secondUserTag))
                {
                    await Clients.Group(USER_PREFIX + _userConnections.GetUserId(secondUserTag)).SendAsync("NotificationReply", new NotificationModel
                    {
                        Details = n.Detail,
                        Title = n.Title,
                        NotificationId = n.ID
                    });
                    await Groups.AddToGroupAsync(_userConnections.GetUserConnectionId(secondUserTag), GROUP_CHAT_PREFIX + chat.ID);
                }

                await Clients.Group(GROUP_CHAT_PREFIX + chat.ID).SendAsync("NewChatReply", chatModel);


            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }

        }
        public async Task UpdateChat(ChatEntity chatEntity)
        {
            try
            {
                await _mediator.Send(new UpdateGroupChatCommand { ChatId = chatEntity.ID, Name = chatEntity.ChatName, AdminTag = chatEntity.AdminTag });

                var users = (await _mediator.Send(new GetUsersByChatQuery { ChatId = chatEntity.ID })).Select(u => u.ID).ToList();

                var notification = await _mediator.Send(new CreateNotificationCommand
                {
                    Details = "One of the chats has been updated",
                    Title = NOTIFICATION_CHAT_PREFIX,
                    UserId = users
                });

                await Clients.OthersInGroup(GROUP_CHAT_PREFIX + chatEntity.ID).SendAsync("NotificationReply", new NotificationModel
                {
                    Details = notification.Detail,
                    Title = notification.Title,
                    NotificationId = notification.ID
                });
                await Clients.Group(GROUP_CHAT_PREFIX + chatEntity.ID).SendAsync("UpdateChatReply", new ChatModel
                {
                    ChatName = chatEntity.ChatName,
                    ChatId = chatEntity.ID
                });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }
        public async Task AddUserInChat(int chatId, string userTag)
        {
            try
            {
                await _mediator.Send(new AddUserInChatCommand { ChatId = chatId, UserTag = userTag });

                var newUser = await _mediator.Send(new GetUserByTagQuery { UserTag = userTag });
                var chat = await _mediator.Send(new GetChatQuery { ChatId = chatId });

                var notificationForOne = await _mediator.Send(new CreateNotificationCommand
                {
                    Details = "You have been added to a new chat",
                    Title = NOTIFICATION_CHAT_PREFIX,
                    UserId = new List<int> { newUser.ID }
                });
                var notificationForAll = await _mediator.Send(new CreateNotificationCommand
                {
                    Details = "A new user has been added to the chat",
                    Title = NOTIFICATION_CHAT_PREFIX,
                    UserId = (await _mediator.Send(new GetUsersByChatQuery { ChatId = chatId })).Where(u => u.ID != newUser.ID)
                         .Select(u => u.ID)
                         .ToList()

                });


                await Clients.Group(GROUP_CHAT_PREFIX + chat.ID).SendAsync("AddUserInChatReply", chatId, new UserModel
                {
                    Email = newUser.Email,
                    SecondName = newUser.SecondName,
                    FirstName = newUser.FirstName,
                    LastName = newUser.FirstName,
                    PhoneNumber = newUser.PhoneNumber,
                    UserTag = userTag,
                    ColorNumber = 0
                });
                await Clients.OthersInGroup(GROUP_CHAT_PREFIX + chatId).SendAsync("NotificationReply", new NotificationModel
                {
                    Details = notificationForAll.Detail,
                    Title = notificationForAll.Title,
                    NotificationId = notificationForAll.ID
                });

                if (_userConnections.IsConnectedUser(userTag))
                {
                    await Groups.AddToGroupAsync(_userConnections.GetUserConnectionId(newUser.ID), GROUP_CHAT_PREFIX + chat.ID);
                    await Clients.Group(USER_PREFIX + _userConnections.GetUserId(userTag)).SendAsync("NotificationReply", new NotificationModel
                    {
                        Details = notificationForOne.Detail,
                        Title = notificationForOne.Title,
                        NotificationId = notificationForOne.ID
                    });
                    await Clients.Group(USER_PREFIX + newUser.ID).SendAsync("NewGroupChatReply", new ChatModel
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
                        Type = (int)ChatType.GROUP,
                        ColorNumber = 1,
                        UserRole = (int)UserRole.EMPLOYEE
                    });
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException.Message);
            }
        }
        public async Task DeleteUserFromChat(int chatId, string userTag)
        {
            try
            {
                await _mediator.Send(new DeleteUserFromChatCommand { ChatId = chatId, UserTag = userTag });

                var kickedUser = await _mediator.Send(new GetUserByTagQuery { UserTag = userTag });

                var notificationForOne = await _mediator.Send(new CreateNotificationCommand
                {
                    Details = "You were kicked out of the chat",
                    Title = NOTIFICATION_CHAT_PREFIX,
                    UserId = new List<int> { kickedUser.ID }
                });
                var notificationForAll = await _mediator.Send(new CreateNotificationCommand
                {
                    Details = $"User {kickedUser.FirstName} has left the chat",
                    Title = NOTIFICATION_CHAT_PREFIX,
                    UserId =
                        (await _mediator.Send(new GetUsersByChatQuery { ChatId = chatId })).Where(u => u.ID != kickedUser.ID)
                        .Select(u => u.ID)
                        .ToList()

                }); ;

                if (_userConnections.IsConnectedUser(userTag))
                {
                    await Clients.Group(USER_PREFIX + kickedUser.ID).SendAsync("DeleteChatReply", chatId);
                    await Groups.RemoveFromGroupAsync(_userConnections.GetUserConnectionId(userTag), GROUP_CHAT_PREFIX + chatId);
                    await Clients.Group(USER_PREFIX + kickedUser.ID).SendAsync("NotificationReply", new NotificationModel
                    {
                        Details = notificationForOne.Detail,
                        Title = notificationForOne.Title,
                        NotificationId = notificationForOne.ID
                    });
                }

                await Clients.Group(GROUP_CHAT_PREFIX + chatId).SendAsync("DeleteUserFromChatReply", chatId, userTag);
                await Clients.OthersInGroup(GROUP_CHAT_PREFIX + chatId).SendAsync("NotificationReply", new NotificationModel
                {
                    Details = notificationForAll.Detail,
                    Title = notificationForAll.Title,
                    NotificationId = notificationForAll.ID
                });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }
        public async Task LeaveChat(int chatId, int userId)
        {
            try
            {
                await _mediator.Send(new LeaveChatCommand { ChatId = chatId, UserId = userId });

                var kickedUser = await _mediator.Send(new GetUserByIdQuery { UserId = userId });
                var users = await _mediator.Send(new GetUsersByChatQuery { ChatId = chatId });

                if (users.Count > 0)
                {
                    var notification = await _mediator.Send(new CreateNotificationCommand
                    {
                        Details = $"User {kickedUser.FirstName} has left the chat",
                        Title = NOTIFICATION_CHAT_PREFIX,
                        UserId = users.Select(u => u.ID).ToList()
                    });

                    await Clients.Group(GROUP_CHAT_PREFIX + chatId).SendAsync("DeleteUserFromChatReply", chatId, kickedUser.Tag);
                    await Clients.OthersInGroup(GROUP_CHAT_PREFIX + chatId).SendAsync("NotificationReply", new NotificationModel
                    {
                        Details = notification.Detail,
                        Title = notification.Title,
                        NotificationId = notification.ID
                    });
                }

                await Clients.Group(USER_PREFIX + userId).SendAsync("DeleteChatReply", chatId);
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, GROUP_CHAT_PREFIX + chatId);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }

    }
}
